using System;
using System.Collections.Generic;

namespace CarBooking.Models;

public partial class CarBookingDetail
{
    public int CarBookingId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int TotalDays { get; set; }

    public int UserId { get; set; }

    public int CarId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDelete { get; set; }

    public int? UpdateBy { get; set; }

    public virtual CarDetail Car { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
