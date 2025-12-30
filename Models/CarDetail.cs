using System;
using System.Collections.Generic;

namespace CarBooking.Models;

public partial class CarDetail
{
    public int CarId { get; set; }

    public string CarName { get; set; } = null!;

    public string CarNumber { get; set; } = null!;

    public DateTime RegisterDate { get; set; }

    public DateTime PucexpireDate { get; set; }

    public DateTime InsuranceDate { get; set; }

    public decimal PricePerDay { get; set; }

    public string? RcimageOrignalFileName { get; set; }

    public string? RcimagePath { get; set; }

    public string? PucimageOrignalFileName { get; set; }

    public string? PucimagePath { get; set; }

    public string? InsuranceImageOrignalFileName { get; set; }

    public string? InsuranceImagePath { get; set; }

    public string? CarImageOrignalFileName { get; set; }

    public string? CarImagePath { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<CarBookingDetail> CarBookingDetails { get; set; } = new List<CarBookingDetail>();

    public virtual ICollection<CarImage> CarImages { get; set; } = new List<CarImage>();
}
