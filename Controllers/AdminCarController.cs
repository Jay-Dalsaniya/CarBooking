using CarBooking.Models;
using CarBooking.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CarBooking.Controllers
{
    [Authorize]
    public class AdminCarController : Controller
    {
        private readonly CarBookingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminCarController(CarBookingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
      
        public async Task<IActionResult> GetAllCarList()
        {
            var cars = _context.CarDetails
                        .Where(c => c.IsDelete == false).Include(o => o.CarImages)
                        .ToList();
            return View(cars);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllCarList([FromBody] RequestPaginationViewModel request)
        {
            int start = request.Start ?? 0;
            int length = request.Length ?? 10;

            IQueryable<CarDetail> query = _context.CarDetails
                    .Where(c => !c.IsDelete);

            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                query = query.Where(c => c.CarName.Contains(request.Search.Value) || c.CarNumber.Contains(request.Search.Value));
            }

            int filteredCount = await query.CountAsync();

            var sort = request.Columns?.FirstOrDefault(c => c.Sort != null);
            bool asc = sort?.Sort?.Direction == KSortDirection.Ascending;

            if (sort != null && !string.IsNullOrEmpty(sort.Field))
                query = query.OrderByDynamic(sort.Field, asc);
            else
                query = query.OrderByDescending(x => x.CreatedDate);

            var data = await query
                .Skip(start)
                .Take(length)
                .Select(x => new CarDetailVM
                {
                    CarId = x.CarId,
                    CarName = x.CarName,
                    CarNumber = x.CarNumber,
                    RegisterDate = x.RegisterDate,
                    PucexpireDate = x.PucexpireDate,
                    InsuranceDate = x.InsuranceDate,
                    PricePerDay = x.PricePerDay
                })
                .ToListAsync();

            int totalCount = await _context.CarDetails.CountAsync(c => !c.IsDelete);

            return Ok(new ResponseList
            {
                Draw = request.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = filteredCount,
                Data = data
            });
        }

        private string SaveImage(IFormFile file, string root)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(root, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return Path.Combine("/CarImages", fileName);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEditCar(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new CarDetailVM
                {
                    RegisterDate = DateTime.Now,
                    PucexpireDate = DateTime.Now,
                    InsuranceDate = DateTime.Now,
                    CarAllImages = new List<IFormFile>(),
                    SavedCarImages = new List<CarImageViewModel>()
                });
            }

            var car = await _context.CarDetails
                .Include(c => c.CarImages)
                .FirstOrDefaultAsync(c => c.CarId == id);

            var model = new CarDetailVM
            {
                CarId = car.CarId,
                CarName = car.CarName,
                CarNumber = car.CarNumber,
                RegisterDate = car.RegisterDate,
                PucexpireDate = car.PucexpireDate,
                InsuranceDate = car.InsuranceDate,
                PricePerDay = car.PricePerDay,
                RCImagePath = car.RcimagePath,
                PUCImagePath = car.PucimagePath,
                InsuranceImagePath = car.InsuranceImagePath,

                SavedCarImages = car.CarImages
                   .Where(x => x.IsDeleted == false)
                   .Select(x => new CarImageViewModel
                   {
                       ImageId = x.ImageId,
                       ImagePath = x.ImagePath
                   })
                   .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> CheckCarNumber(string carNumber, int? carId)
        {
            var existsCarNumber = await _context.CarDetails.FirstOrDefaultAsync(u => u.CarNumber == carNumber && u.CarId != carId);
            return Json(new { existsCarNumber });
        }

        [HttpPost]
        public async Task<IActionResult> AddEditCar(CarDetailVM model)
        {
            if (!ModelState.IsValid)
            {

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
                {
                    TempData["Error"] = "Something went wrong. Please try again.";
                    return RedirectToAction("Login", "User");
                }

                if (model.CarId > 0)
                {
                    var existingCar = await _context.CarDetails.Include(c => c.CarImages).FirstOrDefaultAsync(c => c.CarId == model.CarId);
                    if (existingCar != null)
                    {
                        model.RCImagePath = existingCar.RcimagePath;
                        model.PUCImagePath = existingCar.PucimagePath;
                        model.InsuranceImagePath = existingCar.InsuranceImagePath;
                        model.SavedCarImages = existingCar.CarImages.Select(c => new CarImageViewModel { ImageId = c.ImageId, ImagePath = c.ImagePath }).ToList();
                    }
                }
                return View(model);
            }
            var existingCarNumber = await _context.CarDetails
       .FirstOrDefaultAsync(c => c.CarNumber == model.CarNumber && c.CarId != model.CarId);

            CarDetail carEntity;
            string root = Path.Combine(_env.WebRootPath, "CarImages");

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            if (model.CarName == null)
            {
                ModelState.AddModelError("CarName", "Please add Car Name. This is required.");
                return View(model);
            }

            if (model.CarId == 0 || model.CarId == null)
            {
                carEntity = new CarDetail
                {
                    CarName = model.CarName,
                    CarNumber = model.CarNumber,
                    RegisterDate = model.RegisterDate,
                    PucexpireDate = model.PucexpireDate,
                    InsuranceDate = model.InsuranceDate,
                    PricePerDay = model.PricePerDay,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    IsDelete = false,
                };

                _context.CarDetails.Add(carEntity);
                await _context.SaveChangesAsync();
                TempData["CarCreated"] = "Car has been created successfully!";
            }
            else
            {
                carEntity = await _context.CarDetails
                                          .Include(c => c.CarImages)
                                          .FirstOrDefaultAsync(cd => cd.CarId == model.CarId);

                if (carEntity == null)
                {
                    return NotFound($"Car with ID {model.CarId} not found.");
                }

                carEntity.CarName = model.CarName;
                carEntity.CarNumber = model.CarNumber;
                carEntity.RegisterDate = model.RegisterDate;
                carEntity.PucexpireDate = model.PucexpireDate;
                carEntity.InsuranceDate = model.InsuranceDate;
                carEntity.PricePerDay = model.PricePerDay;
                carEntity.UpdatedDate = DateTime.Now;
                carEntity.IsDelete = false;

                await _context.SaveChangesAsync();
                TempData["CarUpdated"] = "Car Update successfully!";

            }

            if (model.RCImage != null)
            {
                var rcImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.RCImage.FileName);
                var rcImagePath = Path.Combine(root, rcImageFileName);
                using (var stream = new FileStream(rcImagePath, FileMode.Create))
                {
                    await model.RCImage.CopyToAsync(stream);
                }
                carEntity.RcimagePath = "/CarImages/" + rcImageFileName;
            }
            else if (!string.IsNullOrEmpty(model.RCImagePath))
            {
                carEntity.RcimagePath = model.RCImagePath;
            }

            if (model.PUCImage != null)
            {
                var pucImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.PUCImage.FileName);
                var pucImagePath = Path.Combine(root, pucImageFileName);
                using (var stream = new FileStream(pucImagePath, FileMode.Create))
                {
                    await model.PUCImage.CopyToAsync(stream);
                }
                carEntity.PucimagePath = "/CarImages/" + pucImageFileName;
            }
            else if (!string.IsNullOrEmpty(model.PUCImagePath))
            {
                carEntity.PucimagePath = model.PUCImagePath;
            }

            if (model.InsuranceImage != null)
            {
                var insuranceImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.InsuranceImage.FileName);
                var insuranceImagePath = Path.Combine(root, insuranceImageFileName);
                using (var stream = new FileStream(insuranceImagePath, FileMode.Create))
                {
                    await model.InsuranceImage.CopyToAsync(stream);
                }
                carEntity.InsuranceImagePath = "/CarImages/" + insuranceImageFileName;
            }
            else if (!string.IsNullOrEmpty(model.InsuranceImagePath))
            {
                carEntity.InsuranceImagePath = model.InsuranceImagePath;
            }

            if (model.CarAllImages != null && model.CarAllImages.Any())
            {
                foreach (var img in model.CarAllImages)
                {
                    if (img != null)
                    {
                        var carImage = new CarImage
                        {
                            CarId = carEntity.CarId,
                            OriginalFileName = img.FileName,
                            ImagePath = SaveImage(img, root)
                        };
                        _context.CarImages.Add(carImage);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("GetAllCarList");
        }

        public async Task<IActionResult> DeleteCarDocument(int carId, string documentType)
        {
            var car = await _context.CarDetails.FirstOrDefaultAsync(c => c.CarId == carId);

            if (car == null)
            {
                return NotFound();
            }

            string pathToDelete = string.Empty;

            switch (documentType.ToLower())
            {
                case "rc":
                    pathToDelete = car.RcimagePath;
                    car.RcimagePath = null;
                    break;
                case "puc":
                    pathToDelete = car.PucimagePath;
                    car.PucimagePath = null;
                    break;
                case "insurance":
                    pathToDelete = car.InsuranceImagePath;
                    car.InsuranceImagePath = null;
                    break;
                default:
                    return BadRequest("Invalid imageyou delete.");
            }

            if (!string.IsNullOrEmpty(pathToDelete))
            {
                var physicalPath = Path.Combine(_env.WebRootPath, pathToDelete.TrimStart('/'));             
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AddEditCar", new { id = car.CarId });
        }

        public IActionResult DeleteCarImage(int id)
        {
            var img = _context.CarImages.FirstOrDefault(x => x.ImageId == id);
            if (img == null) return NotFound();

            img.IsDeleted = true;
            _context.CarImages.Update(img);
            _context.SaveChanges();

            TempData["SuccessImgDet"] = "Image deleted successfully!";
            return RedirectToAction("AddEditCar", new { id = img.CarId });
        }

        public async Task<IActionResult> DeleteCar(int? id)
        {
            var deleteCar = await _context.CarDetails
              .Include(o => o.CarImages)
              .FirstOrDefaultAsync(o => o.CarId == id);
            if (deleteCar == null) return NotFound("car is not found");

            deleteCar.IsDelete = true;

            foreach (var carImage in deleteCar.CarImages)
            {
                carImage.IsDeleted = true;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("GetAllCarList", "AdminCar");
        }
    }
}