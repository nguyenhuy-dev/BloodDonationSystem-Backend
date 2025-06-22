using Application.DTO.BlogDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.BlogSer
{
    public interface IBlogService
    {
        Task<BlogRequestDTO> CreateBlogAsync(BlogRequestDTO blogDTO);

        Task<BlogResponseDTO> GetBlogByIdAsync(int id);

        Task<PaginatedResult<BlogResponseDTO>> GetAllBlogAsync(int pageNumber, int pageSize);

        Task<BlogResponseDTO> UpdateBlogAsync(int id, BlogRequestDTO blog);

        Task<bool> DeleteBlogAsync(int id);
    }
}