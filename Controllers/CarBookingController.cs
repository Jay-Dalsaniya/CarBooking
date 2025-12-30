using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarBooking.Models;
using CarBooking.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CarBooking.Controllers
{
    [Authorize]
    public class CarBookingController : Controller
    {
        private readonly CarBookingDbContext _context;

        public CarBookingController(CarBookingDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            HttpContext.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            HttpContext.Response.Headers["Pragma"] = "no-cache";
            HttpContext.Response.Headers["Expires"] = "0";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingChartData(DateTime? startDate, DateTime? endDate)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.Trim();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var carBookingDetails = _context.CarBookingDetails
                .Where(x => !x.IsDelete);
            if (role == "User")
            {
                carBookingDetails = carBookingDetails.Where(c => c.UserId == Convert.ToInt32(userId));
            }

            if (startDate.HasValue)
            {
                carBookingDetails = carBookingDetails.Where(x => x.StartDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                carBookingDetails = carBookingDetails.Where(x => x.StartDate.Date <= endDate.Value.Date);
            }

            var data = await carBookingDetails
                .GroupBy(x => x.StartDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            CarBookingChartVM vm = new CarBookingChartVM
            {
                Dates = data.Select(d => d.Date.ToString("dd/MM/yyyy")).ToList(),
                BookingCounts = data.Select(d => d.Count).ToList()
            };

            return Ok(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingChartDataPie(DateTime? startDate, DateTime? endDate)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.Trim();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var carBookingDetails = _context.CarBookingDetails
                .Where(x => !x.IsDelete);

            if (role == "User")
            {
                carBookingDetails = carBookingDetails.Where(c => c.UserId == Convert.ToInt32(userId));
            }

            if (startDate.HasValue)
            {
                carBookingDetails = carBookingDetails.Where(x => x.StartDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                carBookingDetails = carBookingDetails.Where(x => x.StartDate.Date <= endDate.Value.Date);
            }

            var data = await carBookingDetails
                .GroupBy(x => new { x.StartDate.Year, x.StartDate.Month })
                .Select(s => new
                {
                    Year = s.Key.Year,
                    Month = s.Key.Month,
                    Count = s.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            CarBookingChartpie pieDetail = new CarBookingChartpie
            {
                Dates = data.Select(d => new DateTime(d.Year, d.Month, 1).ToString("MMMM yyyy")).ToList(),
                BookingCounts = data.Select(d => d.Count).ToList()
            };

            return Ok(pieDetail);
        }

        [HttpGet]
        public async Task<IActionResult> GetCarList()
        {
            var cars = _context.CarDetails
                .Where(a => a.IsDelete == false)
                .Select(c => new { id = c.CarId, name = c.CarName })
                .ToList();

            return Ok(cars);
        }

        public async Task<IActionResult> GetCarDetails(int carId)
        {
            var car = await _context.CarDetails
                .Where(c => c.CarId == carId && !c.IsDelete)
                .Select(c => new
                {
                    c.CarName,
                    c.CarNumber,
                    c.PricePerDay,
                    c.PucexpireDate,
                    c.InsuranceDate,
                    c.RegisterDate,
                    Images = c.CarImages.Select(i => new { i.ImagePath }).ToList()
                })
                .FirstOrDefaultAsync();

            return Json(car);
        }

        public async Task<IActionResult> GetCarPrice(int carId)
        {
            var car = await _context.CarDetails
                .Where(c => c.CarId == carId)
                .Select(c => c.PricePerDay)
                .FirstOrDefaultAsync();
            return Json(car);
        }

        public async Task<IActionResult> CheckCarAvailability(int carId, DateTime startDate, DateTime endDate, int bookingId = 0)
        {
            var existingBooking = await _context.CarBookingDetails
                .Where(b => b.CarId == carId &&
                            b.StartDate <= endDate &&
                            b.EndDate >= startDate &&
                            b.CarBookingId != bookingId &&
                            !b.IsDelete)
                .FirstOrDefaultAsync();

            if (existingBooking != null)
            {
                return Json(new { isAvailable = false, errorMessage = "This car is already booked for selected dates." });
            }

            return Json(new { isAvailable = true });
        }

        public async Task<IActionResult> GetAllCarBookingList()
        {
            var bookings = _context.CarBookingDetails.Where(a => a.IsDelete == false)
               .Include(x => x.Car)
               .Include(x => x.User)
               .ToList();

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllCarBookingList([FromBody] RequestPaginationViewModel request)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.Trim();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int start = request.Start ?? 0;
            int length = request.Length ?? 10;

            IQueryable<CarBookingDetail> query = _context.CarBookingDetails
                .Where(c => !c.IsDelete)
                .Include(c => c.Car)
                .Include(c => c.User);

            if (role == "User" && !string.IsNullOrEmpty(userId))
            {
                query = query.Where(c => c.UserId == Convert.ToInt32(userId));
            }

            if (!string.IsNullOrEmpty(request.StartDate))
            {
                DateTime sDate = DateTime.Parse(request.StartDate);
                query = query.Where(x => x.StartDate >= sDate);
            }

            if (!string.IsNullOrEmpty(request.EndDate))
            {
                DateTime eDate = DateTime.Parse(request.EndDate);
                query = query.Where(x => x.EndDate <= eDate);
            }

            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();

                query = query.Where(x =>
                    x.Car.CarName.ToLower().Contains(search) ||
                    x.User.UserName.ToLower().Contains(search) ||
                    x.TotalAmount.ToString().Contains(search) ||
                    x.StartDate.ToString().Contains(search) ||
                    x.EndDate.ToString().Contains(search)
                );
            }
            int filteredCount = await query.CountAsync();

            var sort = request.Columns?.FirstOrDefault(c => c.Sort != null);

            if (sort != null && !string.IsNullOrEmpty(sort.Field))
            {
                bool asc = sort.Sort.Direction == KSortDirection.Ascending;

                query = sort.Field switch
                {
                    "carName" => asc ? query.OrderBy(x => x.Car.CarName)
                                       : query.OrderByDescending(x => x.Car.CarName),

                    "userName" => asc ? query.OrderBy(x => x.User.UserName)
                                       : query.OrderByDescending(x => x.User.UserName),

                    "startDate" => asc ? query.OrderBy(x => x.StartDate)
                                       : query.OrderByDescending(x => x.StartDate),

                    "endDate" => asc ? query.OrderBy(x => x.EndDate)
                                       : query.OrderByDescending(x => x.EndDate),

                    "totalAmount" => asc ? query.OrderBy(x => x.TotalAmount)
                                         : query.OrderByDescending(x => x.TotalAmount),

                    _ => query.OrderByDescending(x => x.CreatedDate)
                };
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedDate);
            }


            var data = await query
                .Skip(start)
                .Take(length)
                .Select(x => new
                {
                    carBookingId = x.CarBookingId,
                    carName = x.Car.CarName,
                    userName = x.User.UserName,
                    startDate = x.StartDate.ToString("yyyy-MM-dd"),
                    endDate = x.EndDate.ToString("yyyy-MM-dd"),
                    totalAmount = x.TotalAmount
                })
                .ToListAsync();

            int totalCount = await _context.CarBookingDetails.CountAsync(c => !c.IsDelete);

            return Ok(new ResponseList
            {
                Draw = request.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = filteredCount,
                Data = data
            });
        }

        //public async Task<IActionResult> CarBooking(int id)
        //{
        //    //var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        //    //if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
        //    //{
        //    //    return RedirectToAction("Login", "User");
        //    //} 
        //    if (id == 0)
        //        return View(new CarBookingVM
        //        {
        //            StartDate = DateTime.Now,
        //            EndDate = DateTime.Now,
        //            AvailableCars = await _context.CarDetails.Where(c => !c.IsDelete).ToListAsync()
        //        });


        //    var booking = await _context.CarBookingDetails
        //        .Include(b => b.Car)
        //        .FirstOrDefaultAsync(b => b.CarBookingId == id);

        //    if (booking == null)
        //    {
        //        return NotFound();
        //    }

        //    var model = new CarBookingVM
        //    {
        //        CarBookingId = booking.CarBookingId,
        //        CarId = booking.CarId,
        //        UserId = booking.UserId,
        //        StartDate = booking.StartDate,
        //        EndDate = booking.EndDate,
        //        TotalAmount = booking.TotalAmount,
        //        TotalDays = booking.TotalDays,
        //        AvailableCars = await _context.CarDetails.Where(c => !c.IsDelete).ToListAsync()
        //    };

        //    foreach (var car in model.AvailableCars)
        //    {
        //        car.CarImages = await _context.CarImages.Where(ci => ci.CarId == car.CarId).ToListAsync();
        //    }

        //    return View(model);
        //}

        public async Task<IActionResult> CarBooking(int id, int? carId)
        {

            if (id == 0)
            {
                var model = new CarBookingVM
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    AvailableCars = await _context.CarDetails.Where(c => !c.IsDelete).ToListAsync(),
                    CarId = carId ?? 0
                };

                return View(model);
            }

            var booking = await _context.CarBookingDetails
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.CarBookingId == id);

            if (booking == null)
                return NotFound();

            var editModel = new CarBookingVM
            {
                CarBookingId = booking.CarBookingId,
                CarId = booking.CarId,
                UserId = booking.UserId,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalAmount = booking.TotalAmount,
                TotalDays = booking.TotalDays,
                AvailableCars = await _context.CarDetails.Where(c => !c.IsDelete).ToListAsync()
            };
            foreach (var car in editModel.AvailableCars)
            {
                car.CarImages = await _context.CarImages.Where(ci => ci.CarId == car.CarId).ToListAsync();
            }
            return View(editModel);
        }


        [HttpPost]
        public async Task<IActionResult> CarBooking(CarBookingVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "User");

                int totalDays = (model.EndDate - model.StartDate).Days + 1;
                var selectedCar = await _context.CarDetails.FindAsync(model.CarId);
                if (selectedCar == null)
                {
                    TempData["Error"] = "Selected car not found.";
                    return View(model);
                }

                decimal price = selectedCar.PricePerDay;
                decimal totalAmount = price * totalDays;

                if (userRole == "Admin")
                {
                    price = model.Price;
                    totalAmount = model.TotalAmount;
                }

                if (model.CarBookingId == 0 || model.CarBookingId == null)
                {
                    var addBooking = new CarBookingDetail
                    {
                        CarId = model.CarId,
                        UserId = Convert.ToInt32(userId),
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        TotalDays = totalDays,
                        TotalAmount = totalAmount,
                        CreatedDate = DateTime.Now,
                        IsDelete = false
                    };

                    _context.CarBookingDetails.Add(addBooking);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Car booking created successfully!";
                }
                else
                {
                    var booking = await _context.CarBookingDetails.FindAsync(model.CarBookingId);
                    if (booking != null)
                    {
                        booking.CarId = model.CarId;
                        //booking.UserId = Convert.ToInt32(userId);
                        booking.StartDate = model.StartDate;
                        booking.EndDate = model.EndDate;
                        booking.TotalDays = totalDays;
                        booking.TotalAmount = totalAmount;
                        booking.UpdatedDate = DateTime.Now;
                        booking.UpdateBy = Convert.ToInt32(userId);

                        //if (userRole == "Admin")
                        //            booking.UserId = Convert.ToInt32(userId);
                        _context.Update(booking);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Car booking updated successfully!";
                    }
                }

                return RedirectToAction("GetAllCarBookingList");
            }

            return View(model);
        }

        public async Task<IActionResult> ViewBookingDetails(int bookingId)
        {
            var booking = await _context.CarBookingDetails
                .Include(b => b.Car)
                .ThenInclude(c => c.CarImages)
                .FirstOrDefaultAsync(b => b.CarBookingId == bookingId && !b.IsDelete);

            if (booking == null)
                return NotFound();

            var model = new CarBookingVM
            {
                CarBookingId = booking.CarBookingId,
                CarId = booking.CarId,
                CarName = booking.Car.CarName,
                CarNumber = booking.Car.CarNumber,
                Price = booking.Car.PricePerDay,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalDays = booking.TotalDays,
                TotalAmount = booking.TotalAmount,
                AvailableCars = new List<CarDetail> { booking.Car }
            };

            return View("ViewBookingDetails", model);
        }

        public async Task<IActionResult> DeleteCarBooking(int? id)
        {
            var deleteCarBooking = await _context.CarBookingDetails
              .Include(b => b.Car)
              .FirstOrDefaultAsync(b => b.CarBookingId == id);

            if (deleteCarBooking == null) return NotFound("car booking is not found");

            if (deleteCarBooking != null)
            {
                deleteCarBooking.IsDelete = true;
                _context.Update(deleteCarBooking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(GetAllCarBookingList));
        }
    }
}