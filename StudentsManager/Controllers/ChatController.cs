using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentsManager.Abstract.Service;
using StudentsManager.Domain.Models;

namespace StudentsManager.Controllers;

public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(IChatService chatService, UserManager<ApplicationUser> userManager)
    {
        _chatService = chatService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var currentUserId = _userManager.GetUserId(User);
        var users = await _chatService.GetAllUsersExceptCurrent(currentUserId);
        return Json(users);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string receiverId, string content)
    {
        var senderId = _userManager.GetUserId(User);
        await _chatService.SendMessageAsync(senderId, receiverId, content);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetUnreadMessages(string receiverId)
    {
        var messages = await _chatService.GetUnreadMessagesAsync(receiverId);
        return Json(messages);
    }

    public async Task<IActionResult> GetMessages(string contactId)
    {
        var userId = _userManager.GetUserId(User);
        var messages = await _chatService.GetMessagesWithContactAsync(userId, contactId);
        return Json(messages);
    }

    [HttpPost]
    public async Task<IActionResult> MarkMessagesAsRead([FromBody] List<int> messageIds)
    {
        await _chatService.MarkMessagesAsReadAsync(messageIds);
        return Ok();
    }
}