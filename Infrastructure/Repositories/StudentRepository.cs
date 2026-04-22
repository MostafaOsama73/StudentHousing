using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class StudentRepository : BaseRepository<Student>, IStudentRepository
{
    public StudentRepository(StudentHousingDBContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        string? sortBy = null,
        bool ascending = true)
    {
        var query = GetAll(asNoTracking: true);

        // Apply search/filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.Address.Contains(search) ||
                s.City.Contains(search) ||
                s.PreferredArea.Contains(search) ||
                s.NationalId.Contains(search));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "address":
                    query = ascending ? query.OrderBy(s => s.Address) : query.OrderByDescending(s => s.Address);
                    break;
                case "city":
                    query = ascending ? query.OrderBy(s => s.City) : query.OrderByDescending(s => s.City);
                    break;
                case "preferredarea":
                    query = ascending ? query.OrderBy(s => s.PreferredArea) : query.OrderByDescending(s => s.PreferredArea);
                    break;
                case "nationalid":
                    query = ascending ? query.OrderBy(s => s.NationalId) : query.OrderByDescending(s => s.NationalId);
                    break;
                case "dateofbirth":
                    query = ascending ? query.OrderBy(s => s.DateOfBirth) : query.OrderByDescending(s => s.DateOfBirth);
                    break;
                default:
                    query = ascending ? query.OrderBy(s => s.StudentId) : query.OrderByDescending(s => s.StudentId);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(s => s.StudentId);
        }

        // Apply pagination
        var students = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }
}
