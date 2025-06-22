using Application.DTO;
using Application.DTO.BlogDTO;
using Application.Service.BlogSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [ApiController]
    public class BlogController(IBlogService _blogService) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost("api/blogs/create")]
        public async Task<IActionResult> CreateBlog(BlogRequestDTO blogDTO)
        {
            var blog = await _blogService.CreateBlogAsync(blogDTO);
            return Ok(new ApiResponse<BlogRequestDTO>
            {
                IsSuccess = true,
                Message = "Blog created successfully",
                Data = blog
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("api/blogs/{id}")]
        public async Task<IActionResult> UpdateBlog(int id, BlogRequestDTO blogDTO)
        {
            var updatedBlog = await _blogService.UpdateBlogAsync(id, blogDTO);
            return Ok(new ApiResponse<BlogResponseDTO>
            {
                IsSuccess = true,
                Message = "Blog updated successfully",
                Data = updatedBlog
            });
        }

        [Authorize]
        [HttpPut("api/blogs/{id}/delete")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var deleteBlog = await _blogService.DeleteBlogAsync(id);
            return Ok(new
            {
                IsSuccess = true,
                Message = "Blog deleted successfully"
            });
        }

        [HttpGet("api/blogs")]
        public async Task<IActionResult> GetAllBlogAsync([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            var blogs = await _blogService.GetAllBlogAsync(pageNumber, pageSize);
            if (blogs == null)
            {
                return NotFound("No blog founded");
            }
            return Ok(blogs);
        }

        [HttpGet("api/blogs/{id}")]
        public async Task<IActionResult> GetBlogByIdAsync(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if(blog == null)
            {
                return NotFound("No blog found");
            }
            return Ok(blog);
        }
    }
}
