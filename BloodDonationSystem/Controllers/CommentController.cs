using Application.DTO.CommentDTO;
using Application.Service.CommentServ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [ApiController]

    public class CommentController(ICommentService _commentService) : ControllerBase
    {
        [Authorize]
        [HttpPost("api/{blogId}/comments")]
        public async Task<IActionResult> CreateCommentAsync(int blogId, [FromBody] CommentRequestDTO commentRequest)
        {
            var comment = await _commentService.AddCommentAsync(blogId, commentRequest);
            if (comment == null)
            {
                return BadRequest(new 
                { 
                    IsSuccess = false, 
                    Message = "Failed to create comment." 
                });
            }
            return Ok(new 
            { 
                IsSuccess = true, 
                Message = "Comment created successfully.", 
                Data = comment 
            });
        }

        [HttpGet("api/{blogId}/comments")]
        public async Task<IActionResult> GetAllCommentsAsync(int blogId)
        {
            var comments = await _commentService.GetAllCommentsAsync(blogId);
            if (comments == null || !comments.Any())
            {
                return NotFound(new 
                { 
                    IsSuccess = false, 
                    Message = "No comments found." 
                });
            }
            return Ok(new 
            { 
                IsSuccess = true, 
                Message = "Comments retrieved successfully.",
                Data = comments 
            });
        }

        [HttpGet("api/comments/{id}")]
        public async Task<IActionResult> GetCommentByIdAsync(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound(new 
                { 
                    IsSuccess = false, 
                    Message = "Comment not found." 
                });
            }
            return Ok(new 
            { 
                IsSuccess = true, 
                Message = "Comment retrieved successfully.", 
                Data = comment 
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("api/comments/{id}/moderate")]
        public async Task<IActionResult> ModerateCommentAsync(int id)
        {
            var result = await _commentService.ModerateCommentAsync(id);
            if (result == null)
            {
                return NotFound(new 
                { 
                    IsSuccess = false, 
                    Message = "Comment not found or already moderated." 
                });
            }
            return Ok(new 
            { 
                IsSuccess = true, 
                Message = "Comment moderated successfully." 
            });
        }
    }
}
