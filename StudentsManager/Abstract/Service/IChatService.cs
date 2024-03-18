using StudentsManager.Domain.Models;

namespace StudentsManager.Abstract.Service;

public interface IChatService
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersExceptCurrent(string currentUserId);
    Task SendMessageAsync(string senderId, string receiverId, string content);
    Task<IEnumerable<Message>> GetUnreadMessagesAsync(string receiverId);
    Task<IEnumerable<dynamic>> GetMessagesWithContactAsync(string userId, string contactId);
    Task MarkMessagesAsReadAsync(List<int> messageIds);
}