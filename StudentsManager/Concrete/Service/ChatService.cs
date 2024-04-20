using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;

namespace StudentsManager.Concrete.Service;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatService(IChatRepository chatRepository, UserManager<ApplicationUser> userManager)
    {
        _chatRepository = chatRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersExceptCurrent(string currentUserId)
    {
        return await _userManager.Users.Where(u => u.Id != currentUserId).ToListAsync();
    }

    public async Task SendMessageAsync(string senderId, string receiverId, string content)
    {
        await _chatRepository.AddMessageAsync(senderId, receiverId, content);
    }

    public async Task<IEnumerable<dynamic>> GetMessagesWithContactAsync(string userId, string contactId)
    {
        return await _chatRepository.GetMessagesWithContactAsync(userId, contactId);
    }
}