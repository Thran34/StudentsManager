using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentsManager.Concrete.Service.Secrets;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace StudentsManager.Tests.Context;

[Collection("TestCollection1")]
public class ApplicationDbContextTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var projectId = configuration["ProjectSettings:ProjectId"];
        var secretManager = new SecretManagerService();
        var connectionString = secretManager.GetSecretAsync("conn_string", projectId).Result;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }


    [Fact]
    public void CanAddStudentWithoutClassGroup()
    {
        var user = new ApplicationUser
        {
            UserName = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        var student = new Student
        {
            FirstName = "John",
            LastName = "Doe",
            ApplicationUserId = user.Id,
            PhoneNumber = "123"
        };
        _context.Students.Add(student);
        _context.SaveChanges();

        var savedStudent = _context.Students.Find(student.StudentId);
        Assert.NotNull(savedStudent);
        Assert.Null(savedStudent.ClassGroupId);
    }

    [Fact]
    public void CanAddMessageWithSenderAndReceiver()
    {
        var sender = new ApplicationUser { UserName = "sender@example.com", FirstName = "John", LastName = "Doe" };
        var receiver = new ApplicationUser { UserName = "receiver@example.com", FirstName = "John", LastName = "Doe" };
        _context.Users.AddRange(sender, receiver);
        _context.SaveChanges();

        var message = new Message { SenderId = sender.Id, ReceiverId = receiver.Id, Content = "Hello" };
        _context.Messages.Add(message);
        _context.SaveChanges();

        var savedMessage = _context.Messages.Find(message.Id);
        Assert.NotNull(savedMessage);
        Assert.Equal("Hello", savedMessage.Content);
        Assert.Equal(sender.Id, savedMessage.SenderId);
        Assert.Equal(receiver.Id, savedMessage.ReceiverId);
    }

    public void Dispose()
    {
        _context.Messages.RemoveRange(_context.Messages);
        _context.Students.RemoveRange(_context.Students);
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
        _context.Dispose();
    }
}