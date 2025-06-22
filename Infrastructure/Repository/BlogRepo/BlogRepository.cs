using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.BlogRepo
{
    public class BlogRepository(BloodDonationSystemContext _context) : IBlogRepository
    {
        public async Task<int> CountAllAsync()
        {
            var count = await _context.Blogs.Where(b => b.IsActived == true).CountAsync();
            return count;
        }

        public async Task<Blog> CreateBlogAsync(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<bool> DeleteBlogAsync(int id)
        {
            var blog = await GetBlogByIdAsync(id);
            if (blog == null)
            {
                return false; // Blog not found
            }
            blog.IsActived = false; // Soft delete
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
            return true; // Blog successfully deleted
        }

        public async Task<List<Blog>> GetAllBlogAsync(int pageNumber, int pageSize)
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .OrderByDescending(blog => blog.CreateAt)
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Blog> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);
            return blog;
        }

        public async Task<Blog> UpdateBlogAsync(Blog blog)
        {
            _context.Update(blog);
            await _context.SaveChangesAsync();
            return blog;
        }
    }
}