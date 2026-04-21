using Business.Models.Requests;
using Business.Models.Responses;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace StudentHousingAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class StudentController : BaseController
{
    private readonly IStudentService studentService;

    public StudentController(IStudentService studentService)
    {
        this.studentService = studentService;
    }

    [HttpPost("update")]
    [ProducesResponseType(typeof(StudentResponse),StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(StudentUpdateRequest request)
    {
        var response = await studentService.Update(request);

        return Ok(response);
    }

    [HttpGet("GetStudent/{id}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await studentService.GetById(id);

        return Ok(response);
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(IEnumerable<StudentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await studentService.GetAll();
        return Ok(response);
    }

    [HttpPost("GetAllFilterd")]
    [ProducesResponseType(typeof(StudentIndexedResponse), StatusCodes.Status200OK)]

    public async Task<IActionResult> GetAllFilterd(StudentFilterRequest filter)
    {
        var response = await studentService.GetAllFilterd(filter);

        return Ok(response);
    }

    [HttpPost("ChangePassword")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = GetUserId();

        var response = await studentService.ChangePassword(request, userId);

        return Ok(response);
    }

    [HttpPost("SetDeletion/{id}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetDeletion(StudentDeleteRequest request)
    {
        var response = await studentService.SetDeletion(request);

        return Ok(response);
    }


}
