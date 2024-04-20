namespace StudentsManager.Abstract.Service;

public interface IChatRepository
{
    Task AddMessageAsync(string senderId, string receiverId, string content);
    Task<IEnumerable<dynamic>> GetMessagesWithContactAsync(string userId, string contactId);
}