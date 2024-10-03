using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StudentsManager.Concrete.Service;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.Abstract.Service;
using StudentsManager.Concrete.Service.Secrets;
using Xunit;
using Assert = Xunit.Assert;

namespace StudentsManager.Tests.Services;

public class ChatServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ChatService _chatService;
    private readonly Mock<IChatRepository> _chatRepositoryMock;
    private readonly ServiceProvider _serviceProvider;

    public ChatServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var projectId = configuration["ProjectSettings:ProjectId"];
        var secretManager = new SecretManagerService();
        var connectionString = secretManager.GetSecretAsync("conn_string", projectId).Result;

        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts => opts.UseSqlServer(connectionString));
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        _context.Database.EnsureCreated();

        _userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _chatRepositoryMock = new Mock<IChatRepository>();

        _chatService = new ChatService(_chatRepositoryMock.Object, _userManager);

        SetupTestData().Wait();
    }

    private async Task SetupTestData()
    {
        var user1 = new ApplicationUser
        {
            UserName = "user1@example.com",
            FirstName = "User1",
            LastName = "Test",
            Email = "user1@example.com"
        };
        await _userManager.CreateAsync(user1, "Password123!");

        var user2 = new ApplicationUser
        {
            UserName = "user2@example.com",
            FirstName = "User2",
            LastName = "Test",
            Email = "user2@example.com"
        };
        await _userManager.CreateAsync(user2, "Password123!");

        var user3 = new ApplicationUser
        {
            UserName = "user3@example.com",
            FirstName = "User3",
            LastName = "Test",
            Email = "user3@example.com"
        };
        await _userManager.CreateAsync(user3, "Password123!");
    }

    [Fact]
    public async Task GetAllUsersExceptCurrent_ShouldReturnAllUsersExceptCurrent()
    {
        var currentUser = await _userManager.FindByEmailAsync("user1@example.com");
        var result = await _chatService.GetAllUsersExceptCurrent(currentUser.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count()); // Expecting user2 and user3
        Assert.DoesNotContain(result, u => u.Id == currentUser.Id);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldCallAddMessageAsyncInRepository()
    {
        var sender = await _userManager.FindByEmailAsync("user1@example.com");
        var receiver = await _userManager.FindByEmailAsync("user2@example.com");
        var content = "Hello, how are you?";

        await _chatService.SendMessageAsync(sender.Id, receiver.Id, content);

        _chatRepositoryMock.Verify(repo => repo.AddMessageAsync(sender.Id, receiver.Id, content), Times.Once);
    }

    [Fact]
    public async Task GetMessagesWithContactAsync_ShouldReturnMessagesBetweenTwoUsers()
    {
        var user = await _userManager.FindByEmailAsync("user1@example.com");
        var contact = await _userManager.FindByEmailAsync("user2@example.com");

        var mockMessages = new List<dynamic>
        {
            new { SenderId = user.Id, ReceiverId = contact.Id, Content = "Hello!" },
            new { SenderId = contact.Id, ReceiverId = user.Id, Content = "Hi, how are you?" }
        };

        _chatRepositoryMock
            .Setup(repo => repo.GetMessagesWithContactAsync(user.Id, contact.Id))
            .ReturnsAsync(mockMessages);

        var result = await _chatService.GetMessagesWithContactAsync(user.Id, contact.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Content == "Hello!");
        Assert.Contains(result, m => m.Content == "Hi, how are you?");
    }

    public void Dispose()
    {
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
        _context.Dispose();
    }
}