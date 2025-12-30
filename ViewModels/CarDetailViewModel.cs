namespace CarBooking.ViewModels
{

    public class CarDetailVM
    {
        public int CarId { get; set; }
        public string CarName { get; set; } = null!;
        public string CarNumber { get; set; } = null!;
        public DateTime RegisterDate { get; set; }
        public DateTime PucexpireDate { get; set; }
        public DateTime InsuranceDate { get; set; }
        public decimal PricePerDay { get; set; }
        public IFormFile? RCImage { get; set; }
        public IFormFile? PUCImage { get; set; }
        public IFormFile? InsuranceImage { get; set; }
        public string? RCImagePath { get; set; }
        public string? PUCImagePath { get; set; }
        public string? InsuranceImagePath { get; set; }
        public List<IFormFile>? CarAllImages { get; set; } = new List<IFormFile>();
        public List<CarImageViewModel> SavedCarImages { get; set; } = new List<CarImageViewModel>();
    }
  
    public class CarImageViewModel
    {
        public int ImageId { get; set; }
        public string ImagePath { get; set; } = null!;
    }
    public class RequestPaginationViewModel
    {

        public int? Draw { get; set; }
        public int? Start { get; set; }
        public int? Length { get; set; }
        public Search? Search { get; set; }
        public IEnumerable<KColumn>? Columns { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }

    public class Search
    {
        public bool? IsRegex { get; set; }
        public string? Value { get; set; }
    }

    public class KColumn
    {
        public string? Field { get; set; }
        public object? Name { get; set; }
        public KSearch? Search { get; set; }
        public bool? IsSearchable { get; set; }
        public bool? IsSortable { get; set; }
        public KSort? Sort { get; set; }

    }

    public class KSort
    {
        public int? Order { get; set; }
        public KSortDirection? Direction { get; set; }
    }

    public enum KSortDirection
    {
        Ascending = 0,
        Descending = 1
    }

    public class KSearch
    {
        public string? Value { get; set; }
        public bool? IsRegex { get; set; }
    }

    public class ResponseList
    {
        public int? RecordsTotal { get; set; }
        public int? RecordsFiltered { get; set; }
        public object? Data { get; set; }
        public int? Draw { get; set; }
    }
}

