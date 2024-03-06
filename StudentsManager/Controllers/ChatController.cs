using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Models;

namespace StudentsManager.Controllers;

public class ChatController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly Context.Context _context;

    public ChatController(UserManager<ApplicationUser> userManager, Context.Context context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Json(users.ToList());
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string receiverId, string content)
    {
        var senderId = _userManager.GetUserId(User);
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    Timestamp = DateTime.UtcNow
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetUnreadMessages(string receiverId)
    {
        var userId = _userManager.GetUserId(User);
        var messages = await _context.Messages
            .Where(m => m.ReceiverId == userId && !m.IsRead)
            .ToListAsync();

        return Json(messages);
    }

    public async Task<IActionResult> GetMessages(string contactId)
    {
        var userId = _userManager.GetUserId(User);
        var messages = await _context.Messages
            .Where(m => (m.SenderId == userId && m.ReceiverId == contactId) ||
                        (m.SenderId == contactId && m.ReceiverId == userId))
            .Select(m => new
            {
                m.Id,
                m.Content,
                m.Timestamp,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                SenderFirstName = m.Sender.FirstName
            })
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        return Json(messages);
    }

    [HttpPost]
    public async Task<IActionResult> MarkMessagesAsRead([FromBody] List<int> messageIds)
    {
        var messages = await _context.Messages.Where(m => messageIds.Contains(m.Id)).ToListAsync();
        foreach (var message in messages) message.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok();
    }
}