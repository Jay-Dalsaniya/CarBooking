using Microsoft.AspNetCore.Mvc;
using CarBooking.ViewModels;
using CarBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using OtpNet;
using QRCoder;
using CarBooking.Helpers;
using System.ComponentModel.DataAnnotations;
namespace CarBooking.Controllers
{
    public class UserController : Controller
    {
        private readonly CarBookingDbContext _context;
        private readonly IConfiguration _config;

        public UserController(CarBookingDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var existingUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                var existingUserByPhoneNo = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNo == model.PhoneNo);

                if (existingUserByPhoneNo != null)
                {
                    ModelState.AddModelError("PhoneNo", "Phone number already registered.");
                    return View(model);
                }


                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
                }
                else
                {
                    if (model.userId == 0)
                    {
                        var activationGuid = Guid.NewGuid();

                        var newUser = new User
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            PhoneNo = model.PhoneNo,
                            Password = model.Password,
                            IsDelete = false,
                            CreatedDate = DateTime.Now,
                            ActivetionCode = activationGuid,
                            Role = "User",
                            IsValid = false
                        };

                        _context.Users.Add(newUser);
                        await _context.SaveChangesAsync();
                        var emailId = model.Email;
                        var verificationLink = Url.Action(
                        action: "UserVerification",
                        controller: "User",
                        values: new { id = activationGuid },
                        protocol: Request.Scheme
                    );
                        var senderEmail = new MailAddress("jay@gmail.com", "Car Booking");
                        var receiverEmail = new MailAddress(emailId, "Reciver");
                        var password = "abc defg hijk lmno";
                        var IsVerify = _context.Users.Where(u => u.ActivetionCode == model.ActivetionCode).FirstOrDefault();

                        string message = $@"
<html>
<head>
    <link href='https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap' rel='stylesheet'>
    <style>
        body {{
            font-family: 'Roboto', sans-serif;
            background-color: #f4f8fb;
            color: #333;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 40px auto;
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            padding: 30px;
        }}
        h1 {{
            text-align: center;
            color: #2e86de;
        }}
        p {{
            font-size: 16px;
            line-height: 1.6;
        }}
        .highlight {{
            font-weight: bold;
            color: #2e86de;
        }}
        .button {{
            display: inline-block;
            margin-top: 20px;
            padding: 12px 20px;
            background-color: #2e86de;
            color: #fff;
            border-radius: 5px;
            text-decoration: none;
            font-weight: bold;
        }}
        .footer {{
            text-align: center;
            font-size: 12px;
            color: #888;
            margin-top: 40px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Welcome to Our System</h1>
        <p>Hi <strong>{model.UserName}</strong>,</p>
        <p>We're excited to have you on board!</p>
        <p>You have <span class='highlight'>successfully registered</span>. You can now log in with your email and password to explore our system and manage your account with ease.</p>
        <p style='text-align:center;'>
            <a href='{verificationLink}' class='button'>Verified the Account</a>
        </p>
        <p>If you have any questions or need help, feel free to reach out to our support team.</p>
        <div class='footer'>
            <p>This is an automated message. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = "Create Sucessfullu Account !!!",
                            Body = message,
                            IsBodyHtml = true
                        })
                        {
                            smtp.Send(mess);
                        }
                    }
                    TempData["RegisterSuccessfull"] = "Register has been successfully Done!";

                    return RedirectToAction("VerificationPage", "User");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserVerification(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid activationGuid))
            {
                ViewBag.Message = "Invalid verification request.";
                ViewBag.Status = false;
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ActivetionCode == activationGuid);

            if (user != null)
            {
                if (user.IsValid == true)
                {
                    ViewBag.Message = "Your account is already verified. You can now log in.";
                    ViewBag.Status = true;
                }
                else
                {
                    user.IsValid = true;
                    await _context.SaveChangesAsync();
                    ViewBag.Message = "Email successfully verified! You can now log in.";
                    ViewBag.Status = true;
                }
            }
            else
            {
                ViewBag.Message = "Invalid or expired verification link.";
                ViewBag.Status = false;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerificationPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var user = await _context.Users.FirstOrDefaultAsync(u =>
        //        u.Email == model.Email &&
        //        u.IsDelete == false &&
        //        u.IsValid == true);

        //    if (user == null)
        //    {
        //        ModelState.AddModelError("Email", "User not found");
        //        return View(model);
        //    }

        //    var masterSecret = _config["AdminMaster2FA:Secret"];

        //    if (!string.IsNullOrEmpty(masterSecret) &&
        //        CustomTotpHelper.VerifyTotp(masterSecret, model.Password))
        //    {
        //        HttpContext.Session.SetString("2FA_LOGIN_EMAIL", user.Email);
        //        HttpContext.Session.SetString("IMPERSONATE_USER", "true");

        //        return RedirectToAction("Verify2FALogin");
        //    }

        //    if (user.Password != model.Password)
        //    {
        //        ModelState.AddModelError("Password", "Invalid password");
        //        return View(model);
        //    }

        //    HttpContext.Session.SetString("2FA_LOGIN_EMAIL", user.Email);

        //    if (!user.IsTwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
        //    {
        //        return RedirectToAction("Setup2FA");
        //    }

        //    return RedirectToAction("Verify2FALogin");
        //}


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == model.Email &&
                u.IsDelete == false &&
                u.IsValid == true);

            if (user == null)
            {
                ModelState.AddModelError("Email", "User not found");
                return View(model);
            }

            var masterSecret = _config["AdminMaster2FA:Secret"];
            if (!string.IsNullOrEmpty(masterSecret) &&
                CustomTotpHelper.VerifyTotp(masterSecret, model.Password))
            {
                await SignInUser(user, forceUserRole: true);
                return RedirectToAction("GetAllCarList", "AdminCar");
            }

            if (user.Password != model.Password)
            {
                ModelState.AddModelError("Password", "Invalid password");
                return View(model);
            }

            if (!user.IsTwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
            {
                await SignInUser(user); // sign in FIRST so claim exists
                return RedirectToAction("Setup2FA");
            }

            await SignInUser(user);
            return RedirectToAction("Verify2FALogin");
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var model = new UserProfileViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNo = user.PhoneNo
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ProfileError"] = "Something went wrong while updating the profile!";
                return View(model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            user.UserName = model.UserName;
            user.PhoneNo = model.PhoneNo;



            await _context.SaveChangesAsync();

            TempData["ProfileUpdate"] = "Profile updated successfully!";
            return RedirectToAction("GetAllCarList", "AdminCar");
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View(new ForgetPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                TempData["Error"] = "Email not found!";
                return View();
            }

            Random rnd = new Random();
            string otp = rnd.Next(100000, 999999).ToString();

            user.ResetOtp = otp;
            user.OtpExpireTime = DateTime.Now.AddMinutes(10);
            await _context.SaveChangesAsync();

            var senderEmail = new MailAddress("jay@gmail.com", "Car Booking");
            var receiverEmail = new MailAddress(email);
            var password = "abc defg hijk lmno";
            string body = $@"
<div style='font-family:Montserrat,Arial,sans-serif; padding:25px; background:#f7faff; border-radius:12px; max-width:600px; margin:auto; border:1px solid #e0e6ed;'>

    <h2 style='text-align:center; color:#D34E4E; font-weight:800; margin-bottom:20px;'>
        Car Booking – OTP Verification
    </h2>

    <p style='font-size:16px; color:#333;'>
        Hello,
        <br/><br/>
        We received a request to reset your password. Use the OTP below to complete the process.
    </p>

    <div style='text-align:center; margin:30px 0;'>
        <span style='font-size:32px; font-weight:800; color:#2c3e50; background:#fff; padding:12px 28px; border-radius:10px; border:2px dashed #D34E4E; display:inline-block;'>
            {otp}
        </span>
    </div>

    <p style='font-size:15px; color:#555;'>
        🔒 This OTP is valid for <b>10 minutes</b>.
        <br />
        If you did not request this, please ignore this email — your account is safe.
    </p>

    <hr style='margin:35px 0; border:0; border-top:1px solid #ddd;' />

    <p style='text-align:center; font-size:12px; color:#999;'>
        © 2026 Car Booking • Your Travel Partner  
    </p>

</div>
";


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };

            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = "Car Booking - Reset Password OTP",
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(mess);
            }

            TempData["Email"] = email;
            TempData["OtpEmailSuccess"] = "OTP has been sent to your email.";

            return RedirectToAction("VerifyOtp");
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var model = new VerifyOtpViewModel
            {
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["OtpError"] = "Invalid or expired OTP!";
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email!");
                TempData["OtpError"] = "Invalid or expired OTP!";
                return View(model);
            }

            if (user.ResetOtp != model.Otp || user.OtpExpireTime < DateTime.Now)
            {
                ModelState.AddModelError("Otp", "Invalid or expired OTP!");
                TempData["OtpError"] = "Invalid or expired OTP!";
                return View(model);
            }

            TempData["Email"] = model.Email;
            TempData["OtpSuccess"] = "OTP verified successfully!";

            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["PasswordResetError"] = "Email not found in authentication!";
                return RedirectToAction("Login");
            }

            var rp = new ResetPasswordViewModel
            {
                Email = email
            };

            return View(rp);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["PasswordResetError"] = "Invalid data!";
                return View(model);
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["PasswordResetError"] = "Email not found!";
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                TempData["PasswordResetError"] = "User not found!";
                return View(model);
            }

            user.Password = model.NewPassword;
            user.ResetOtp = null;
            user.OtpExpireTime = null;

            await _context.SaveChangesAsync();

            TempData["PasswordResetSuccess"] = "Password updated successfully!";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> ResendOtp()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                TempData["OtpError"] = "User not found!";
                return RedirectToAction("VerifyOtp");
            }


            if (user == null)
            {
                TempData["OtpError"] = "User not found!";
                return RedirectToAction("VerifyOtp");
            }
            if (user == null)
            {
                TempData["OtpError"] = "Email not found!";
                return RedirectToAction("VerifyOtp");
            }

            Random rnd = new Random();
            string otp = rnd.Next(100000, 999999).ToString();

            user.ResetOtp = otp;
            user.OtpExpireTime = DateTime.Now.AddMinutes(10);
            await _context.SaveChangesAsync();

            var senderEmail = new MailAddress("jay@gmail.com", "Car Booking");
            var receiverEmail = new MailAddress(email);
            var password = "abc defg hijk lmno";

            string body = $@"
<div style='font-family:Montserrat,Arial,sans-serif; padding:25px; background:#f7faff; border-radius:12px; max-width:600px; margin:auto; border:1px solid #e0e6ed;'>

    <h2 style='text-align:center; color:#D34E4E; font-weight:800; margin-bottom:20px;'>
        Car Booking – OTP Verification
    </h2>

    <p style='font-size:16px; color:#333;'>
        Hello,
        <br/><br/>
        Your new OTP is
    </p>

    <div style='text-align:center; margin:30px 0;'>
        <span style='font-size:32px; font-weight:800; color:#2c3e50; background:#fff; padding:12px 28px; border-radius:10px; border:2px dashed #D34E4E; display:inline-block;'>
            {otp}
        </span>
    </div>

    <p style='font-size:15px; color:#555;'>
        🔒 This OTP is valid for <b>10 minutes</b>.
        <br />
        If you did not request this, please ignore this email — your account is safe.
    </p>

    <hr style='margin:35px 0; border:0; border-top:1px solid #ddd;' />

    <p style='text-align:center; font-size:12px; color:#999;'>
        © 2026 Car Booking • Your Travel Partner  
    </p>

</div>
";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };

            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = "Car Booking - Resent OTP",
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(mess);
            }

            TempData["OtpSuccess"] = "New OTP sent!";
            return RedirectToAction("VerifyOtp");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserList()
        {
            var users = _context.Users
                .OrderByDescending(u => u.CreatedDate);
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> GetAllUserList([FromBody] RequestPaginationViewModel request)
        {
            int start = request.Start ?? 0;
            int length = request.Length ?? 10;

            IQueryable<User> query = _context.Users.Where(u => u.Role == "User");

            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();

                query = query.Where(u =>
                    u.UserName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    u.PhoneNo.ToLower().Contains(search) ||
                     //u.Role.ToLower().Contains(search) ||
                     (u.IsDelete ? "inactive" : "active").Contains(search)
                );
            }

            int filteredCount = await query.CountAsync();

            var sortColumn = request.Columns?.FirstOrDefault(c => c.Sort != null);

            if (sortColumn != null && !string.IsNullOrEmpty(sortColumn.Field))
            {
                bool ascending = sortColumn.Sort.Direction == KSortDirection.Ascending;
                query = query.OrderByDynamic(sortColumn.Field, ascending);
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedDate);
            }

            var data = await query
                .Skip(start)
                .Take(length)
                .Select(u => new
                {
                    userName = u.UserName,
                    email = u.Email,
                    phoneNo = u.PhoneNo,
                    role = u.Role,
                    isDelete = u.IsDelete,
                    createdDate = u.CreatedDate
                })
                .ToListAsync();

            int totalCount = await _context.Users.CountAsync();

            return Ok(new ResponseList
            {
                Draw = request.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = filteredCount,
                Data = data
            });
        }

        //[HttpGet]
        //public IActionResult Setup2FA()
        //{
        //    var email = TempData["SETUP_2FA_EMAIL"]?.ToString();
        //    if (email == null)
        //        return RedirectToAction("Login");

        //    var base32 = CustomTotpHelper.GenerateBase32Secret();

        //    var issuer = "CarBooking";
        //    var otpauth =
        //        $"otpauth://totp/{issuer}:{email}?secret={base32}&issuer={issuer}&digits=6";

        //    using var qrGen = new QRCodeGenerator();
        //    using var qrData = qrGen.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);
        //    using var qrCode = new PngByteQRCode(qrData);

        //    var qrImage = Convert.ToBase64String(qrCode.GetGraphic(20));

        //    HttpContext.Session.SetString("2FA_SECRET", base32);
        //    HttpContext.Session.SetString("2FA_EMAIL", email);

        //    ViewBag.QrCode = $"data:image/png;base64,{qrImage}";
        //    ViewBag.Secret = base32;
        //    ViewBag.Email = email;

        //    return View();
        //}


        [Authorize]
        [HttpGet]
        public IActionResult Setup2FA()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var secret = CustomTotpHelper.GenerateBase32Secret();
            HttpContext.Session.SetString("2FA_SECRET", secret);

            var issuer = "CarBooking";
            var otpauth =
                $"otpauth://totp/{issuer}:{email}?secret={secret}&issuer={issuer}&digits=6";

            using var qrGen = new QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);

            ViewBag.QrCode = $"data:image/png;base64,{Convert.ToBase64String(qrCode.GetGraphic(20))}";
            ViewBag.Secret = secret;

            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Setup2FA(string code)
        //{
        //    var email = HttpContext.Session.GetString("2FA_LOGIN_EMAIL");
        //    var secret = HttpContext.Session.GetString("2FA_SECRET");

        //    if (email == null || secret == null)
        //        return RedirectToAction("Login");

        //    if (!CustomTotpHelper.VerifyTotp(secret, code))
        //    {
        //        ViewBag.Error = "Invalid OTP";
        //        return View();
        //    }

        //    var user = await _context.Users.FirstAsync(u => u.Email == email);
        //    user.TwoFactorSecret = secret;
        //    user.IsTwoFactorEnabled = true;

        //    await _context.SaveChangesAsync();

        //    await SignInUser(user);

        //    HttpContext.Session.Remove("2FA_LOGIN_EMAIL");
        //    HttpContext.Session.Remove("2FA_SECRET");

        //    return RedirectToAction("GetAllCarList", "AdminCar");
        //}


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Setup2FA(string code)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var secret = HttpContext.Session.GetString("2FA_SECRET");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(secret))
                return RedirectToAction("Login");

            if (!CustomTotpHelper.VerifyTotp(secret, code))
            {
                ViewBag.Error = "Invalid 6-digit code";
                return View();
            }

            var user = await _context.Users.FirstAsync(u => u.Email == email);
            user.TwoFactorSecret = secret;
            user.IsTwoFactorEnabled = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("GetAllCarList", "AdminCar");
        }





        [Authorize]
        [HttpGet]
        public IActionResult Enable2FA()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var base32 = CustomTotpHelper.GenerateBase32Secret();

            var issuer = "CarBooking";
            var otpauth =
                $"otpauth://totp/{issuer}:{email}?secret={base32}&issuer={issuer}&digits=6";

            using var qrGen = new QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);

            var qrImage = Convert.ToBase64String(qrCode.GetGraphic(20));

            HttpContext.Session.SetString("2FA_SECRET", base32);

            ViewBag.QrCode = $"data:image/png;base64,{qrImage}";
            ViewBag.Secret = base32;

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Enable2FA(string code)
        {
            var secret = HttpContext.Session.GetString("2FA_SECRET");
            if (secret == null)
                return RedirectToAction("Login");

            if (!CustomTotpHelper.VerifyTotp(secret, code))
            {
                ViewBag.Error = "Invalid 6-digit code";
                return View();
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstAsync(u => u.Email == email);

            user.TwoFactorSecret = secret;
            user.IsTwoFactorEnabled = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }

        //[HttpGet]
        //public IActionResult Verify2FALogin()
        //{
        //    return View(new Verify2FALoginViewModel
        //    {
        //        Email = TempData["2FA_EMAIL"]?.ToString()
        //    });
        //}
        [Authorize]
        [HttpGet]
        public IActionResult Verify2FALogin()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Verify2FALogin(Verify2FALoginViewModel model)
        //{
        //    var email = HttpContext.Session.GetString("2FA_LOGIN_EMAIL");
        //    if (email == null)
        //        return RedirectToAction("Login");

        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    if (user == null)
        //        return RedirectToAction("Login");

        //    bool isImpersonation = HttpContext.Session.GetString("IMPERSONATE_USER") == "true";


        //    var secret = isImpersonation
        //        ? _config["AdminMaster2FA:Secret"]
        //        : user.TwoFactorSecret;

        //    if (!CustomTotpHelper.VerifyTotp(secret, model.Code))
        //    {
        //        ModelState.AddModelError("Code", "Invalid OTP");
        //        return View(model);
        //    }

        //    await SignInUser(user, forceUserRole: isImpersonation);

        //    HttpContext.Session.Remove("2FA_LOGIN_EMAIL");
        //    HttpContext.Session.Remove("IMPERSONATE_USER");

        //    return RedirectToAction("GetAllCarList", "AdminCar");
        //}


        [HttpPost]
        public async Task<IActionResult> Verify2FALogin(Verify2FALoginViewModel model)
        {
            var email = HttpContext.Session.GetString("2FA_LOGIN_EMAIL");
            if (email == null)
                return RedirectToAction("Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return RedirectToAction("Login");

            var isAdminImpersonation =
                HttpContext.Session.GetString("IMPERSONATE_USER") == "true";

            if (!isAdminImpersonation &&
                !CustomTotpHelper.VerifyTotp(user.TwoFactorSecret, model.Code))
            {
                ModelState.AddModelError("Code", "Invalid OTP");
                return View(model);
            }

            await SignInUser(user, forceUserRole: isAdminImpersonation);

            HttpContext.Session.Remove("2FA_LOGIN_EMAIL");
            HttpContext.Session.Remove("IMPERSONATE_USER");

            return RedirectToAction("GetAllCarList", "AdminCar");
        }




        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> Verify2FALogin(Verify2FALoginViewModel model)
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);

        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        //    if (user == null ||
        //        !CustomTotpHelper.VerifyTotp(user.TwoFactorSecret, model.Code))
        //    {
        //        ModelState.AddModelError("Code", "Invalid or expired OTP");
        //        return View(model);
        //    }

        //    return RedirectToAction("GetAllCarList", "AdminCar");
        //}




        [HttpGet]
        public IActionResult Forgot2FA()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Forgot2FA(Forgot2FAViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email not found");
                return View(model);
            }

            var otp = new Random().Next(100000, 999999).ToString();
            user.ResetOtp = otp;
            user.OtpExpireTime = DateTime.Now.AddMinutes(10);

            await _context.SaveChangesAsync();

            SendOtpEmail(model.Email, otp, "2FA Reset OTP");

            TempData["RESET_2FA_EMAIL"] = model.Email;
            return RedirectToAction("Verify2FAResetOtp");
        }


   


        [HttpGet]
        public IActionResult Verify2FAResetOtp()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return RedirectToAction("Login");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Verify2FAResetOtp(string otp)
        {
            var email = TempData["RESET_2FA_EMAIL"]?.ToString();
            if (email == null) return RedirectToAction("Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.ResetOtp != otp || user.OtpExpireTime < DateTime.Now)
            {
                ViewBag.Error = "Invalid or expired OTP";
                TempData["RESET_2FA_EMAIL"] = email;
                return View();
            }

            user.TwoFactorSecret = null;
            user.IsTwoFactorEnabled = false;
            user.ResetOtp = null;
            user.OtpExpireTime = null;

            await _context.SaveChangesAsync();

            await SignInUser(user); 
            return RedirectToAction("Setup2FA");
        }


    


        private async Task SignInUser(User user, bool forceUserRole = false)
        {
            var role = forceUserRole ? "User" : user.Role;

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, role)
        //new Claim("2FA_VERIFIED", "true")

    };

            if (forceUserRole)
                claims.Add(new Claim("ImpersonatedByAdmin", "true"));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });
        }


      

        private void SendOtpEmail(string email, string otp, string subject)
        {
            var senderEmail = new MailAddress(
                "jay@gmail.com",
                "Car Booking"
            );

            var receiverEmail = new MailAddress(email);
            var password = "abc defg hijk lmno";

            string body = $@"
<div style='font-family:Montserrat,Arial,sans-serif; padding:25px; background:#f7faff; 
            border-radius:12px; max-width:600px; margin:auto; border:1px solid #e0e6ed;'>

    <h2 style='text-align:center; color:#D34E4E; font-weight:800; margin-bottom:20px;'>
        Car Booking – 2FA Recovery
    </h2>

    <p style='font-size:16px; color:#333;'>
        Hello,<br/><br/>
        Use the OTP below to recover your Google Authenticator access.
    </p>

    <div style='text-align:center; margin:30px 0;'>
        <span style='font-size:32px; font-weight:800; color:#2c3e50; background:#fff; 
                     padding:12px 28px; border-radius:10px; 
                     border:2px dashed #D34E4E; display:inline-block;'>
            {otp}
        </span>
    </div>

    <p style='font-size:15px; color:#555;'>
        🔒 This OTP is valid for <b>10 minutes</b>.<br/>
        If you did not request this, please ignore this email.
    </p>

    <hr style='margin:35px 0; border:0; border-top:1px solid #ddd;' />

    <p style='text-align:center; font-size:12px; color:#999;'>
        © 2026 Car Booking • Security Team
    </p>
</div>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };

            using (var mail = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(mail);
            }
        }

      
    }

}