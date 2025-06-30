using Application.DTO.BlogDTO;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.BlogRepo;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Service.BlogSer
{
    public class BlogService(IBlogRepository _blogRepository,
                             IHttpContextAccessor _contextAccessor) : IBlogService
    {
        public async Task<BlogRequestDTO> CreateBlogAsync(BlogRequestDTO blogDTO)
        {
            var user = _contextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(user) || !Guid.TryParse(user, out Guid userId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }
            var blog = new Blog
            {
                Title = blogDTO.Title,
                Content = blogDTO.Content,
                AuthorId = userId,
                IsActived = true,
                CreateAt = DateTime.Now
            };

            var created = await _blogRepository.CreateBlogAsync(blog);
            return new BlogRequestDTO
            {
                Title = created.Title,
                Content = created.Content,
            };
        }

        public async Task<bool> DeleteBlogAsync(int id)
        {
            var deleted = await _blogRepository.DeleteBlogAsync(id);
            if (!deleted)
            {
                throw new KeyNotFoundException("Blog not found or already deleted");
            }
            return true;
        }

        public async Task<PaginatedResult<BlogResponseDTO>> GetAllBlogAsync(int pageNumber, int pageSize)
        {
            var userRole = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            int totalItems;
            IEnumerable<Blog> blogs;

            if (userRole == "Staff")
            {
                totalItems = await _blogRepository.CountAllAsync();
                blogs = await _blogRepository.GetAllBlogAsync(pageNumber, pageSize);
            }
            else
            {
                totalItems = await _blogRepository.CountAllActiveBlogAsync();
                blogs = await _blogRepository.GetAllActiveBlogAsync(pageNumber, pageSize);
            }

            var items = blogs.Select(b => new BlogResponseDTO
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                CreateAt = b.CreateAt,
                LastUpdate = b.LastUpdate,
                IsActived = b.IsActived,
                Author = b.Author.LastName + " " + b.Author.FirstName,
            }).ToList();

            return new PaginatedResult<BlogResponseDTO>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<BlogResponseDTO> GetBlogByIdAsync(int id)
        {
            var userRole = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            Blog blog;

            if (userRole == "Staff")
            {
                blog = await _blogRepository.GetBlogByIdAsync(id);
            }
            else
            {
                blog = await _blogRepository.GetActiveBlogByIdAsync(id);
            }

            if (blog == null)
            {
                return null;
            }

            return new BlogResponseDTO
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                CreateAt = blog.CreateAt,
                LastUpdate = blog.LastUpdate,
                IsActived = blog.IsActived,
                Author = blog.Author.LastName + " " + blog.Author.FirstName,
            };
        }

        public async Task<BlogResponseDTO> UpdateBlogAsync(int id, BlogRequestDTO blogDTO)
        {
            var existingBlog = await _blogRepository.GetBlogByIdAsync(id);
            if (existingBlog == null)
            {
                throw new KeyNotFoundException("Blog not found");
            }

            existingBlog.Title = blogDTO.Title;
            existingBlog.Content = blogDTO.Content;
            existingBlog.IsActived = true;
            existingBlog.LastUpdate = DateTime.Now;

            var updated = await _blogRepository.UpdateBlogAsync(existingBlog);
            return new BlogResponseDTO
            {
                Id = id,
                Title = updated.Title,
                Content = updated.Content,
                Author = existingBlog.Author.LastName + " " + existingBlog.Author.FirstName,
                CreateAt = existingBlog.CreateAt,
                LastUpdate = existingBlog.LastUpdate,
            };
        }
    }
}