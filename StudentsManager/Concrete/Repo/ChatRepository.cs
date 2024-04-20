using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract.Service;
using StudentsManager.Context;
using StudentsManager.Domain.Models;

namespace StudentsManager.Concrete.Repo;

public class ChatRepository : IChatRepository
{
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddMessageAsync(string senderId, string receiverId, string content)
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
}