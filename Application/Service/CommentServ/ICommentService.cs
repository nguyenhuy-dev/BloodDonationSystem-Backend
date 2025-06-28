using Application.DTO.CommentDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.CommentServ
{
    public interface ICommentService
    {
        Task<CommentRequestDTO?> AddCommentAsync(int blogId, CommentRequestDTO comment);
        Task<CommentResponseDTO?> GetCommentByIdAsync(int id);
        Task<List<CommentResponseDTO>> GetAllCommentsAsync(int blogId);
        Task<Comment?> DeleteCommentAsync(int id);
        Task<CommentResponseDTO> ModerateCommentAsync(int id);
    }
}
