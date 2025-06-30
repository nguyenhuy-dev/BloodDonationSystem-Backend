using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.CommentRepo
{
    public interface ICommentRepository
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment> GetCommentByIdAsync(int id);
        Task<List<Comment>> GetAllCommentsAsync(int blogId);
        Task<Comment> DeleteCommentAsync(int id);
        Task<int> ModerateCommentAsync(int id);
    }
}
