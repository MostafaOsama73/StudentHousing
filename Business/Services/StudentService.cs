using AutoMapper;
using Business.Models.Helpers;
using Business.Models.Requests;
using Business.Models.Responses;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services;

public interface IStudentService
{
    Task<StudentResponse> Update(StudentUpdateRequest request);
    Task<StudentResponse> GetById(Guid id);
    Task<IEnumerable<StudentResponse>> GetAll();
    Task<StudentIndexedResponse> GetAllFilterd(StudentFilterRequest filter);
    Task<bool> ChangePassword(ChangePasswordRequest request, string userId);
    Task<StudentResponse> SetDeletion(StudentDeleteRequest request);
}

public class StudentService : IStudentService
{
    private readonly IStudentRepository studentRepository;
    private readonly UserManager<User> userManager;
    private readonly IMapper mapper;
    public StudentService(IStudentRepository studentRepository, IMapper mapper, UserManager<User> userManager)
    {
        this.studentRepository = studentRepository;
        this.userManager = userManager;
        this.mapper = mapper;
    }

    public async Task<StudentResponse> Update(StudentUpdateRequest request)
    {
        var student = await studentRepository.GetAsync(request.StudentId);

        if (student == null)        
            throw new Exception(ErrorMessageHelper.StudentNotFound);

        student.UpdatedAt = DateTime.UtcNow;

        var updatedStudent = mapper.Map(request, student);

        await studentRepository.Update(updatedStudent);

        await studentRepository.CommitAsync();

        var response = mapper.Map<StudentResponse>(updatedStudent);

        return response;
    }

    public async Task<StudentResponse> GetById(Guid id)
    {
        var student = await studentRepository.GetAsync(id);

        if (student == null)
            throw new Exception(ErrorMessageHelper.StudentNotFound);

        var response = mapper.Map<StudentResponse>(student);

        return response;
    }

    public async Task<IEnumerable<StudentResponse>> GetAll()
    {
        var students = await studentRepository.GetAll().ToListAsync();

        var response = mapper.Map<IEnumerable<StudentResponse>>(students);

        return response;
    }

    public async Task<StudentIndexedResponse> GetAllFilterd(StudentFilterRequest filter)
    {
        var students = studentRepository.GetAll(true).Where(a => (filter.City == null || a.City == filter.City)
                                                                && (filter.PreferredArea == null || a.PreferredArea == filter.PreferredArea)
                                                                && (filter.Gender == null || a.Gender == filter.Gender)
                                                                && (filter.DateOfBirthFrom == null || a.DateOfBirth >= filter.DateOfBirthFrom)
                                                                && (filter.DateOfBirthTo == null || a.DateOfBirth <= filter.DateOfBirthTo));
                                                     

        var totalRecords = await students.CountAsync();

        students = students.Skip(filter.PageNumber * filter.PageSize)
                           .Take(filter.PageSize);

        var data = mapper.Map<List<StudentResponse>>(await students.ToListAsync());

        return new StudentIndexedResponse
        {
            PageSize = filter.PageSize,
            PageIndex = filter.PageNumber,
            TotalRecords = totalRecords,
            Records = data
        };
    }


    public async Task<bool> ChangePassword(ChangePasswordRequest request, string userId)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || 
            string.IsNullOrWhiteSpace(request.NewPassword))
            throw new ApplicationException(ErrorMessageHelper.CurrentPasswordAndNewPasswordAreRequired);

        if (request.NewPassword != request.ConfirmPassword)
            throw new ApplicationException(ErrorMessageHelper.PasswordsDoNotMatch);

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new ApplicationException(ErrorMessageHelper.UserNotFound);

        // Verify current password
        var passwordValid = await userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!passwordValid)
            throw new ApplicationException(ErrorMessageHelper.CurrentPasswordIncorrect);

        // Change password using UserManager (which handles validation and hashing)
        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Failed to change password: {errors}");
        }

        return true;
    }

    public async Task<StudentResponse> SetDeletion(StudentDeleteRequest request)
    {
        var student = await studentRepository.GetAll().Include(u => u.User).FirstOrDefaultAsync(u => u.StudentId == request.StudentId);
        
        if (student == null)
            throw new ApplicationException(ErrorMessageHelper.StudentNotFound);

        if (student.User == null)
            throw new ApplicationException(ErrorMessageHelper.UserNotFound);

        student.User.IsDeleted = true;

        await studentRepository.Update(student);

        await studentRepository.CommitAsync();

        var response = mapper.Map<StudentResponse>(student);

        return response;
    }
}
