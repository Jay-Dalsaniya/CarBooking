using CarBooking.Models;

namespace CarBooking.ViewModels
{
   
    public class CarBookingVM
    {
        public int CarBookingId { get; set; }
        public int CarId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Price { get; set; }
        public int TotalDays { get; set; }
        public string? CarName { get; set; }   
        public string? CarNumber { get; set; } 
        public List<CarDetail>? AvailableCars { get; set; }
        public List<CarImageViewModel>? CarImages { get; set; } = new List<CarImageViewModel>();

    }
    public class CarBookingChartVM
    {
        public List<string> Dates { get; set; } = new List<string>();
        public List<int> BookingCounts { get; set; } = new List<int>();
    }
    public class CarBookingLineChartViewModel
    {
        public List<string> Dates { get; set; } = new List<string>();
        public List<int> StartDateBookingCounts { get; set; } = new List<int>();
        public List<int> EndDateBookingCounts { get; set; } = new List<int>();
    }
    
    public class CarBookingChartpie
    {
        public List<string> Dates { get; set; } = new List<string>();
        public List<int> BookingCounts { get; set; } = new List<int>();
    }
}
