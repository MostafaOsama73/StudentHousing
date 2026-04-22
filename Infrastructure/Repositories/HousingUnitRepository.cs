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

public class HousingUnitRepository : BaseRepository<HousingUnit>, IHousingUnitRepository
{
    public HousingUnitRepository(StudentHousingDBContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<HousingUnit> HousingUnits, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        string? city = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        bool ascending = true)
    {
        var query = GetAll(asNoTracking: true);

        // Apply search/filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(h =>
                h.Title.Contains(search) ||
                h.Description.Contains(search) ||
                h.Area.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(h => h.City == city);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(h => h.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(h => h.Price <= maxPrice.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "title":
                    query = ascending ? query.OrderBy(h => h.Title) : query.OrderByDescending(h => h.Title);
                    break;
                case "price":
                    query = ascending ? query.OrderBy(h => h.Price) : query.OrderByDescending(h => h.Price);
                    break;
                case "city":
                    query = ascending ? query.OrderBy(h => h.City) : query.OrderByDescending(h => h.City);
                    break;
                case "area":
                    query = ascending ? query.OrderBy(h => h.Area) : query.OrderByDescending(h => h.Area);
                    break;
                default:
                    query = ascending ? query.OrderBy(h => h.HousingUnitId) : query.OrderByDescending(h => h.HousingUnitId);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(h => h.HousingUnitId);
        }

        // Apply pagination
        var housingUnits = await query
            .Include(h => h.LandLord)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (housingUnits, totalCount);
    }
}
