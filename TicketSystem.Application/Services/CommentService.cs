using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByTicketIdAsync(int ticketId)
        {
            return await _commentRepository.GetByTicketIdAsync(ticketId);
        }

        public async Task<Comment> AddCommentAsync(int ticketId, string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Kommentar darf nicht leer sein.");

            var comment = new Comment
            {
                Content = content,
                TicketId = ticketId,
                ApplicationUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            return await _commentRepository.CreateAsync(comment);
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            await _commentRepository.DeleteAsync(commentId);
        }
    }
}