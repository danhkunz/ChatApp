using ChatApp.ChatHubs;
using ChatApp.DBContext;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Yêu cầu xác thực JWT
    [AllowAnonymous]
    public class messagesController : ControllerBase
    {
        private readonly ChatContext _context;
        private readonly IHubContext<ChatHub> _chatHub;

        public messagesController(ChatContext context, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _chatHub = chatHub;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            message.SentAt = DateTime.UtcNow;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await _chatHub.Clients.User(message.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", message);

            return Ok(message);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMessages(string userId)
        {
            var currentUserId = User.Identity.Name;

            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                    (m.SenderId == userId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }
    }

}