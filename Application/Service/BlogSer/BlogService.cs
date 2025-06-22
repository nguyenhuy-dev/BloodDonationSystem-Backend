using Application.DTO.BlogDTO;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.BlogRepo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var totalItems = await _blogRepository.CountAllAsync();
            var blogs = await _blogRepository.GetAllBlogAsync(pageNumber, pageSize);

            var items = blogs.Select(b => new BlogResponseDTO
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                CreateAt = b.CreateAt,
                LastUpdate = b.LastUpdate,
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
            var blog = await _blogRepository.GetBlogByIdAsync(id);
            if (blog == null)
            {
                return null;
            }

            return new BlogResponseDTO{
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                CreateAt = blog.CreateAt,
                LastUpdate = blog.LastUpdate,
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
