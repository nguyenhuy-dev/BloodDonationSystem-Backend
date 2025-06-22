using Domain.Entities;
using Infrastructure.Helper;

namespace Infrastructure.Repository.BlogRepo
{
    public interface IBlogRepository
    {
        Task<Blog> CreateBlogAsync(Blog blog);

        Task<Blog> GetBlogByIdAsync(int id);

        Task<int> CountAllAsync();
        Task<List<Blog>> GetAllBlogAsync(int pageNumber, int pageSize);

        Task<Blog> UpdateBlogAsync(Blog blog);

        Task<bool> DeleteBlogAsync(int id);
    }
}