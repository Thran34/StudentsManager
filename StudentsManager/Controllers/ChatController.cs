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

    [HttpGet]
    public async Task<IActionResult> GetMessages(string contactId)
    {
        var userId = _userManager.GetUserId(User);
        var messages = await _chatService.GetMessagesWithContactAsync(userId, contactId);
        return Json(messages);
    }
}