using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;

namespace StudentsManager;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(IChatService chatService, UserManager<ApplicationUser> userManager)
    {
        _chatService = chatService;
        _userManager = userManager;
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
        }
    }
}