using Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class BookingViewModel
{
    public Guid HousingUnitId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public HousingUnit? HousingUnit { get; set; }
}
