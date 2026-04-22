using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces;

public interface IStudentRepository : IBaseRepository<Student>
{
    Task<(IEnumerable<Student> Students, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        string? sortBy = null,
        bool ascending = true);
}
