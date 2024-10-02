using AutoMapper;
using Moq;
using StudentsManager.Abstract.Repo;
using StudentsManager.Concrete.Service;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;
using Xunit;

namespace StudentsManager.Tests.Services;

[Collection("TestCollection3")]
public class LessonPlanServiceTests
{
    private readonly Mock<ILessonPlanRepository> _lessonPlanRepositoryMock;
    private readonly Mock<IClassGroupRepository> _classGroupRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly LessonPlanService _service;

    public LessonPlanServiceTests()
    {
        _lessonPlanRepositoryMock = new Mock<ILessonPlanRepository>();
        _classGroupRepositoryMock = new Mock<IClassGroupRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new LessonPlanService(_lessonPlanRepositoryMock.Object, _classGroupRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task CreateLessonPlanAsync_ShouldCallRepository()
    {
        var viewModel = new CreateLessonPlanViewModel();
        var lessonPlan = new LessonPlan();
        _mapperMock.Setup(m => m.Map<LessonPlan>(viewModel)).Returns(lessonPlan);

        await _service.CreateLessonPlanAsync(viewModel);

        _lessonPlanRepositoryMock.Verify(repo => repo.AddLessonPlanAsync(lessonPlan), Times.Once);
    }

    [Fact]
    public async Task DeleteLessonPlanAsync_ShouldCallRepository()
    {
        await _service.DeleteLessonPlanAsync(1);

        _lessonPlanRepositoryMock.Verify(repo => repo.DeleteLessonPlanAsync(1), Times.Once);
    }
}