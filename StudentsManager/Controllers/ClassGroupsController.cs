using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentsManager.Abstract.Service;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using StudentsManager.ViewModel;

namespace StudentsManager.Controllers;

[Authorize]
public class ClassGroupsController : Controller
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IClassGroupService _classGroupService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public ClassGroupsController(ApplicationDbContext applicationDbContext,
        IClassGroupService classGroupService, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _classGroupService = classGroupService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var user = await GetUserAsync();
        var userId = _userManager.GetUserId(User);
        var classGroups = await _classGroupService.GetClassGroupsForUserAsync(user, userId);
        return View(classGroups);
    }

    private async Task<ApplicationUser> GetUserAsync()
    {
        return await _userManager.GetUserAsync(User);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var classGroup = await _classGroupService.GetClassGroupDetailsAsync(id.Value);
        if (classGroup == null) return NotFound();

        return View(classGroup);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = await _classGroupService.PrepareCreateClassGroupViewModelAsync();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClassGroupViewModel viewModel)
    {
        if (!ValidateCreateClassGroupViewModel(viewModel))
        {
            viewModel = await _classGroupService.PrepareCreateClassGroupViewModelAsync();
            return View(viewModel);
        }

        var creationResult = await _classGroupService.TryCreateClassGroupAsync(viewModel);

        if (!creationResult)
        {
            viewModel = await _classGroupService.PrepareCreateClassGroupViewModelAsync();
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var success = await _classGroupService.TryDeleteClassGroupAsync(id.Value);

        if (!success) return NotFound();

        return RedirectToAction(nameof(Index));
    }


    private bool ValidateCreateClassGroupViewModel(CreateClassGroupViewModel viewModel)
    {
        var isValid = true;

        if (_applicationDbContext.ClassGroups.Any(x => x.Name == viewModel.Name))
        {
            ModelState.AddModelError("Name", "A class group with this name already exists.");
            isValid = false;
        }

        if (viewModel.SelectedStudentIds == null || !viewModel.SelectedStudentIds.Any())
        {
            ModelState.AddModelError("SelectedStudentIds", "Please select at least one student.");
            isValid = false;
        }

        if (viewModel.SelectedTeacherId == 0)
        {
            ModelState.AddModelError("SelectedTeacherId", "Please select a teacher.");
            isValid = false;
        }

        return isValid;
    }
}