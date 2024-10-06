using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;

namespace StudentsManager;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, UserManager<ApplicationUser> userManager, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SendMessage(string senderId, string receiverId, string content)
    {
        var sender = await _userManager.FindByIdAsync(senderId);
        if (sender != null)
        {
            await _chatService.SendMessageAsync(senderId, receiverId, content);
            await Clients.All.SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                SenderName = sender.FirstName,
                Content = content
            });
            _logger.LogInformation("Message has been sent");
        }
    }
}