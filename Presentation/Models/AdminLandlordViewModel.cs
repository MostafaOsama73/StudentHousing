using Domain.Entities;

namespace Presentation.Models;

public class AdminLandlordViewModel
{
    public IEnumerable<LandLord> Landlords { get; set; } = new List<LandLord>();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public string? Search { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
