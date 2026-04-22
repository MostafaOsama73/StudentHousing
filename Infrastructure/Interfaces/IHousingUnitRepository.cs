using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces;

public interface IHousingUnitRepository : IBaseRepository<HousingUnit>
{
    Task<(IEnumerable<HousingUnit> HousingUnits, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        string? city = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        bool ascending = true);
}
