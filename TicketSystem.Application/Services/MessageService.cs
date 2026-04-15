using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Message>> GetSentMessagesAsync(string userId)
        {
            return await _messageRepository.GetSentMessagesAsync(userId);
        }

        public async Task<IEnumerable<Message>> GetReceivedMessagesAsync(string userId)
        {
            return await _messageRepository.GetReceivedMessagesAsync(userId);
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2)
        {
            return await _messageRepository.GetConversationAsync(userId1, userId2);
        }

        public async Task<Message> SendMessageAsync(string senderId, string receiverId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Nachricht darf nicht leer sein.");

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            return await _messageRepository.CreateAsync(message);
        }

        public async Task MarkAsReadAsync(int messageId)
        {
            await _messageRepository.MarkAsReadAsync(messageId);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _messageRepository.GetUnreadCountAsync(userId);
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            await _messageRepository.DeleteAsync(messageId);
        }

        public async Task<List<ApplicationUser>> GetUsersForMessagingAsync(string currentUserId)
        {
            var allUsers = await _userRepository.GetAllAsync();
            return allUsers.Where(u => u.Id != currentUserId).ToList();
        }
        public async Task<Message> SendMessageAsync(Message message)
        {
            return await _messageRepository.CreateAsync(message);
        }

        public async Task<List<MessageGroupDto>> GetGroupedMessagesAsync(string userId)
        {
            var sentMessages = await _messageRepository.GetSentMessagesAsync(userId);
            var receivedMessages = await _messageRepository.GetReceivedMessagesAsync(userId);

            var allMessages = sentMessages.Concat(receivedMessages);

            var grouped = allMessages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new MessageGroupDto
                {
                    OtherUserId = g.Key ?? "",
                    OtherUserName = GetOtherUserName(g.First(), userId),
                    LastMessage = g.OrderByDescending(m => m.SentAt).First().Content,
                    LastMessageTime = g.Max(m => m.SentAt),
                    UnreadCount = receivedMessages.Count(m => m.SenderId == g.Key && !m.IsRead),
                    Messages = g.OrderBy(m => m.SentAt).ToList()
                })
                .OrderByDescending(g => g.LastMessageTime)
                .ToList();

            return grouped;
        }

        private string GetOtherUserName(Message message, string currentUserId)
        {
            if (message.SenderId == currentUserId)
            {
                if (message.Receiver != null)
                {
                    return !string.IsNullOrEmpty(message.Receiver.Name)
                        ? message.Receiver.Name
                        : message.Receiver.Email ?? "Unbekannt";
                }
                return "Unbekannt";
            }
            else
            {
                if (message.Sender != null)
                {
                    return !string.IsNullOrEmpty(message.Sender.Name)
                        ? message.Sender.Name
                        : message.Sender.Email ?? "Unbekannt";
                }
                return "Unbekannt";
            }


        }
    }
}