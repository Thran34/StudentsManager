using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract.Service;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.Models;

namespace StudentsManager.Concrete.Service;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersExceptCurrent(string currentUserId)
    {
        return await _userManager.Users.Where(u => u.Id != currentUserId).ToListAsync();
    }

    public async Task SendMessageAsync(string senderId, string receiverId, string content)
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
    }

    public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(string receiverId)
    {
        return await _context.Messages
            .Where(m => m.ReceiverId == receiverId && !m.IsRead)
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetMessagesWithContactAsync(string userId, string contactId)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == userId && m.ReceiverId == contactId) ||
                        (m.SenderId == contactId && m.ReceiverId == userId))
            .OrderBy(m => m.Timestamp)
            .Select(m => new
            {
                m.Id,
                m.Content,
                m.Timestamp,
                m.SenderId,
                m.ReceiverId,
                SenderFirstName = m.Sender.FirstName
            })
            .ToListAsync();
    }

    public async Task MarkMessagesAsReadAsync(List<int> messageIds)
    {
        var messages = await _context.Messages.Where(m => messageIds.Contains(m.Id)).ToListAsync();
        foreach (var message in messages) message.IsRead = true;
        await _context.SaveChangesAsync();
    }
}