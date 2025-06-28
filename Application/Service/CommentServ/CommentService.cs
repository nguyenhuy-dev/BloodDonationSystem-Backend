using Application.DTO.CommentDTO;
using Domain.Entities;
using Infrastructure.Repository.CommentRepo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.CommentServ
{
    public class CommentService(ICommentRepository _commentRepository,
                                IHttpContextAccessor _contextAccessor) : ICommentService
    {
        public async Task<CommentRequestDTO?> AddCommentAsync(int blogId, CommentRequestDTO requestComment)
        {
            var user = _contextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(user) || !Guid.TryParse(user, out Guid userId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            var comment = new Comment
            { 
                Text = requestComment.Text,
                CreateAt = DateTime.Now,
                IsLegit = true,
                MemberId = userId,
                BlogId = blogId
            };

            var createdComment = await _commentRepository.CreateCommentAsync(comment);
            return new CommentRequestDTO
            {
                Text = createdComment.Text
            };
        }

        public Task<Comment?> DeleteCommentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CommentResponseDTO>> GetAllCommentsAsync(int blogId)
        {
            var comments = await _commentRepository.GetAllCommentsAsync(blogId);

            return comments.Select(c => new CommentResponseDTO
            {
                Id = c.Id,
                Text = c.Text,
                CreateAt = c.CreateAt,
                Member = c.Member.LastName + " " + c.Member.FirstName,
                UpdateAt = c.UpdateAt,
            }).ToList();
        }

        public async Task<CommentResponseDTO?> GetCommentByIdAsync(int id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);

            return new CommentResponseDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                CreateAt = comment.CreateAt,
                Member = comment.Member.LastName + " " + comment.Member.FirstName,
                UpdateAt = comment.UpdateAt
            };
        }

        public async Task<CommentResponseDTO> ModerateCommentAsync(int id)
        {
            var userId = _contextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            var existingComment = await _commentRepository.GetCommentByIdAsync(id);

            if (existingComment == null)
            {
                return null; // Comment not found
            }

            existingComment.IsLegit = false;
            existingComment.UpdateAt = DateTime.Now;
            existingComment.StaffId = parsedUserId;

            return new CommentResponseDTO
            {
                Id = existingComment.Id,
                Text = existingComment.Text,
                CreateAt = existingComment.CreateAt,
                Member = existingComment.Member.LastName + " " + existingComment.Member.FirstName,
                UpdateAt = existingComment.UpdateAt
            };
        }
    }
}
