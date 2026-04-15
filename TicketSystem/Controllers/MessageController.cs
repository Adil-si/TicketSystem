using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketSystem.Application.Services;

namespace TicketSystem.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public MessageController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var groupedMessages = await _messageService.GetGroupedMessagesAsync(userId);
            var unreadCount = await _messageService.GetUnreadCountAsync(userId);
            var users = await _messageService.GetUsersForMessagingAsync(userId);

            ViewBag.UnreadCount = unreadCount;
            ViewBag.Users = users;

            return View(groupedMessages);
        }

        public async Task<IActionResult> Conversation(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var messages = await _messageService.GetConversationAsync(currentUserId, userId);

            // Als gelesen markieren
            foreach (var message in messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead))
            {
                await _messageService.MarkAsReadAsync(message.Id);
            }

            var otherUser = await _userService.GetUserByIdAsync(userId);
            ViewBag.OtherUser = otherUser;
            ViewBag.OtherUserId = userId;

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string content)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            if (!string.IsNullOrWhiteSpace(content))
            {
                await _messageService.SendMessageAsync(senderId, receiverId, content);
            }

            return RedirectToAction("Conversation", new { userId = receiverId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            await _messageService.DeleteMessageAsync(id);
            return RedirectToAction("Index");
        }
    }
}