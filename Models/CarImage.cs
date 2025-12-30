using System;
using System.Collections.Generic;

namespace CarBooking.Models;

public partial class CarImage
{
    public int ImageId { get; set; }

    public int CarId { get; set; }

    public string ImagePath { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual CarDetail Car { get; set; } = null!;
}
