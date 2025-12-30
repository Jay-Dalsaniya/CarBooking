using System;
using System.Collections.Generic;

namespace CarBooking.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNo { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsDelete { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsValid { get; set; }

    public string Role { get; set; } = null!;

    public Guid? ActivetionCode { get; set; }

    public string? ResetOtp { get; set; }

    public DateTime? OtpExpireTime { get; set; }

    public bool IsTwoFactorEnabled { get; set; }

    public string? TwoFactorSecret { get; set; }

    public virtual ICollection<CarBookingDetail> CarBookingDetails { get; set; } = new List<CarBookingDetail>();
}
