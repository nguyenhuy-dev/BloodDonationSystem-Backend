using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.CommentRepo
{
    public class CommentRepository(BloodDonationSystemContext _context) : ICommentRepository
    {
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public Task<Comment> DeleteCommentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Comment>> GetAllCommentsAsync(int blogId)
        {
            return await _context.Comments
                .Include(c => c.Member) // Include related Member entity if 
                .Where(c => c.BlogId == blogId && c.IsLegit == true)
                .OrderByDescending(c => c.CreateAt)
                .ToListAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.Member)
                .Include(c => c.Staff)
                .Where(c => c.IsLegit == true)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> ModerateCommentAsync(int id)
        {
            return await _context.Comments
                .Where(c => c.Id == id && c.IsLegit == true)
                .ExecuteUpdateAsync(c => c.SetProperty(c => c.IsLegit, false));
        }
    }
}