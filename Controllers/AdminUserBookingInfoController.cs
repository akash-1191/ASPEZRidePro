using System.Text.RegularExpressions;
using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]

    public class AdminUserBookingInfoController : ControllerBase
    {
        private readonly IAdminUserBookingInfoService _service;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AdminUserBookingInfoController(IAdminUserBookingInfoService service, ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _service = service;
            _context = applicationDbContext;
            _configuration = configuration;
        }

        [HttpGet("user-booking-info")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserBookingInfo()
        {
            var data = await _service.GetUserBookingInfoAsync();
            return Ok(data);
        }


        [HttpPut("cancel-reason")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCancelReason([FromBody] BookingCancelReasonDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Cancelreasion))
                return BadRequest("Cancel reason cannot be empty.");

            var booking = await _context.Bookings.FindAsync(dto.BookingId);

            if (booking == null)
                return NotFound("Booking not found.");

            booking.Cancelreasion = dto.Cancelreasion;

            if (booking.Status != Booking.BookingStatus.Cancelled)
            {
                booking.Status = Booking.BookingStatus.Cancelled;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cancel reason updated successfully." });
        }





        [HttpPut("status/inprogress")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetBookingStatusToInProgress([FromBody] handOverTheVehicle dto)
        {
            var booking = await _context.Bookings.FindAsync(dto.BookingId);

            if (booking == null)
                return NotFound(ApiResponseHelper.NotFound("Booking"));

            if (booking.Status == Booking.BookingStatus.Cancelled)
                return BadRequest(ApiResponseHelper.Fail("Journey is already cancelled. Cannot set to InProgress."));

            if (booking.Status == Booking.BookingStatus.InProgress)
                return Ok(ApiResponseHelper.Success("Booking is already in InProgress state."));

            booking.Status = Booking.BookingStatus.InProgress;
            await _context.SaveChangesAsync();

            return Ok(ApiResponseHelper.Success("Booking status is InProgress."));
        }


        [HttpPut("status/completed")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetBookingStatusToCompleted([FromBody] BookingStatusUpdateDto dto)
        {
            var booking = await _context.Bookings.FindAsync(dto.BookingId);

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            if (booking.Status == Booking.BookingStatus.Cancelled)
                return BadRequest(new { message = "Booking is cancelled. Cannot set to Completed." });

            if (booking.Status == Booking.BookingStatus.Completed)
                return Ok(new { message = "Booking is already Completed." });

            booking.Status = Booking.BookingStatus.Completed;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking status set to Completed successfully." });
        }


        //user side to print data 
        // CancelledBookings by the admin to the user side show this data
        [HttpGet("CancelledBookings")]
        public IActionResult GetCancelledBookingsByUser()
        {

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var cancelledBookings = _context.Bookings
                .Where(b => b.Cancelreasion != null && b.UserId == userId)
                .Include(b => b.Vehicle)
                .ToList()
                .Select(b => new
                {
                    b.BookingId,
                    b.BookingType,
                    b.Cancelreasion,
                    b.CreatedAt,
                    b.StartTime,
                    b.EndTime,
                    b.TotalAmount,
                    b.Status,
                    CarName = b.Vehicle?.CarName,
                    BikeName = b.Vehicle?.BikeName,
                    VehicleType = b.Vehicle?.Vehicletype.ToString(),
                    RegistrationNo = b.Vehicle?.RegistrationNo
                })
                .ToList();

            return Ok(cancelledBookings);
        }


        //user side print data
        //user seen the your runing data

        [HttpGet("bookings/inprogressStatus")]
        [Authorize]
        public IActionResult GetInProgressBookingsForUser()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(ApiResponseHelper.Unauthorized("User ID not found in token."));
            }

            int userId = int.Parse(userIdClaim.Value);

            var inProgressBookings = _context.Bookings
                .Where(b => b.Status == Booking.BookingStatus.InProgress && b.UserId == userId)
                .Include(b => b.Vehicle)
                .ToList()
                .Select(b => new
                {
                    b.BookingId,
                    b.BookingType,
                    b.CreatedAt,
                    b.StartTime,
                    b.EndTime,
                    b.TotalAmount,
                    Status = b.Status.ToString(),
                    CarName = b.Vehicle?.CarName,
                    BikeName = b.Vehicle?.BikeName,
                    VehicleType = b.Vehicle?.Vehicletype.ToString(),
                    RegistrationNo = b.Vehicle?.RegistrationNo
                })
                .ToList();

            if (!inProgressBookings.Any())
            {
                return NotFound(ApiResponseHelper.NotFound("InProgress bookings"));
            }

            return Ok(ApiResponseHelper.Success("InProgress bookings retrieved successfully", inProgressBookings));
        }


        [HttpPost("Fuelogpostdata")]
        [Authorize]
        public async Task<IActionResult> CreateFuelLog([FromBody] FuelLogCreateDto dto)
        {
            var fuelLog = new FuelLog
            {
                BookingId = dto.BookingId,
                FuelGiven = dto.FuelGiven,
                FuelReturned = dto.FuelReturned,
                FuelCharge = dto.FuelCharge,
                CreatedAt = DateTime.UtcNow

            };

            _context.FuelLogs.Add(fuelLog);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Fuel log created successfully", fuelLogId = fuelLog.FuelLogId });
        }


        [HttpPost("DamageReportPostData")]
        public async Task<IActionResult> CreateDamageReport([FromBody] DamageReportCreateDto dto)
        {
            try
            {
                string imageFileName = null;

                // Check if image string exists
                if (!string.IsNullOrEmpty(dto.Image))
                {
                    // Remove data:image/...base64, prefix if present
                    var base64Data = Regex.Match(dto.Image, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;

                    if (!string.IsNullOrEmpty(base64Data))
                    {
                        byte[] imageBytes = Convert.FromBase64String(base64Data);
                        string extension = ".png"; // or infer from content-type
                        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "damage-report");

                        // Create directory if not exists
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        imageFileName = $"damage_{Guid.NewGuid()}{extension}";
                        string fullPath = Path.Combine(folderPath, imageFileName);

                        await System.IO.File.WriteAllBytesAsync(fullPath, imageBytes);
                    }
                }

                // Save only file name or relative path in DB
                var damageReport = new DamageReport
                {
                    BookingId = dto.BookingId,
                    Description = dto.Description,
                    RepairCost = dto.RepairCost,
                    Image = imageFileName, // Just file name or "damage-report/damage_xxx.png"
                    CreatedAt = DateTime.UtcNow
                };

                _context.DamageReports.Add(damageReport);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Damage report created successfully", damageId = damageReport.DamageId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to save damage report", error = ex.Message });
            }
        }



        [HttpGet("ReturnSecurityAmount")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCompletedBookings()
        {
            var completedBookings = await _context.Bookings
                .Include(b => b.User)
                    .ThenInclude(u => u.Role)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages)
                .Include(b => b.Payment)
                .Include(b => b.SecurityDeposit)
                .Where(b => b.Status == Booking.BookingStatus.Completed && b.Payment.Status == "Success")
                .Select(booking => new AdminUserBookingInfoDto
                {
                    // Booking info
                    BookingId = booking.BookingId,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    TotalAmount = booking.TotalAmount,
                    BookingStatus = booking.Status.ToString(),

                    BookingType = booking.BookingType,
                    TotalDays = booking.TotalDays,
                    TotalHours = booking.TotalHours,
                    PerKelomeater = booking.PerKelomeater,
                    BookingCreatedAt = booking.CreatedAt,

                    // Payment info
                    PaymentStatus = booking.Payment.Status,
                    PaymentAmount = booking.Payment.Amount,
                    PaymentMethod = booking.Payment.PaymentMethod,
                    TransactionId = booking.Payment.TransactionId,
                    OrderId = booking.Payment.OrderId,
                    PaymentCreatedAt = booking.Payment.CreatedAt,

                    // Security Deposit info
                    SecurityDepositStatus = booking.SecurityDeposit.Status.ToString(),
                    SecurityDepositAmount = booking.SecurityDeposit.Amount,
                    RefundedAt = booking.SecurityDeposit.RefundedAt,
                    SecurityDepositCreatedAt = booking.SecurityDeposit.CreatedAt,

                    // User info
                    UserId = booking.User.UserId,
                    FirstName = booking.User.Firstname,
                    MiddleName = booking.User.Middlename,
                    LastName = booking.User.Lastname,
                    Email = booking.User.Email,
                    Phone = booking.User.Phone,
                    Address = booking.User.Address,
                    City = booking.User.City,
                    State = booking.User.State,
                    Age = booking.User.Age,
                    Gender = booking.User.Gender,
                    UserImage = booking.User.Image,
                    RoleName = booking.User.Role.RoleName.ToString(),
                    UserCreatedAt = booking.User.CreatedAt,

                    // Vehicle info
                    VehicleId = booking.Vehicle.VehicleId,
                    VehicleType = booking.Vehicle.Vehicletype.ToString(),
                    RegistrationNo = booking.Vehicle.RegistrationNo,
                    FuelType = booking.Vehicle.FuelType.ToString(),
                    SeatingCapacity = booking.Vehicle.SeatingCapacity,
                    Mileage = booking.Vehicle.Mileage,
                    Color = booking.Vehicle.Color,
                    CarName = booking.Vehicle.CarName,
                    BikeName = booking.Vehicle.BikeName,
                    Availability = booking.Vehicle.Availability.ToString(),
                    InsuranceStatus = booking.Vehicle.InsuranceStatus.ToString(),
                    RcStatus = booking.Vehicle.RcStatus.ToString(),
                    AcAvailability = booking.Vehicle.AcAvailability.HasValue ? booking.Vehicle.AcAvailability.ToString() : null,
                    FuelTankCapacity = booking.Vehicle.FuelTankCapacity,
                    YearOfManufacture = booking.Vehicle.YearOfManufacture,
                    EngineCapacity = booking.Vehicle.EngineCapacity,
                    VehicleSecurityDepositAmount = booking.Vehicle.SecurityDepositAmount,
                    VehicleCreatedAt = booking.Vehicle.CreatedAt,

                    // Vehicle image
                    VehicleImage = booking.Vehicle.VehicleImages
                        .OrderByDescending(img => img.VehicleImageId)
                        .Select(img => img.ImagePath)
                        .FirstOrDefault(),

                    VehicleImages = booking.Vehicle.VehicleImages
                        .OrderBy(img => img.VehicleImageId)
                        .Select(img => img.ImagePath)
                        .ToList()
                })
                .ToListAsync();

            return Ok(completedBookings);
        }



        //[HttpPost("create-security-deposit-order")]
        //public IActionResult CreateSecurityDepositOrder([FromBody] decimal amount)
        //{
        //    try
        //    {
        //        string key = _configuration["Razorpay:Key"];
        //        string secret = _configuration["Razorpay:Secret"];


        //        RazorpayClient client = new RazorpayClient(key, secret);

        //        Dictionary<string, object> options = new Dictionary<string, object>();
        //        options.Add("amount", amount * 100); // Convert to paise
        //        options.Add("currency", "INR");
        //        options.Add("receipt", "rcptid_" + Guid.NewGuid().ToString().Substring(0, 8));
        //        options.Add("payment_capture", 1); // auto-capture

        //        Razorpay.Api.Order order = client.Order.Create(options);

        //        return Ok(new
        //        {
        //            orderId = order["id"].ToString(),
        //            amount = amount * 100,
        //            currency = "INR"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error creating Razorpay order", error = ex.Message });
        //    }
        //}

        [HttpPost("create-security-deposit-order")]
        public IActionResult CreateSecurityDepositOrder([FromBody] AmountRequest request)
        {
            try
            {
                if (request == null || request.Amount <= 0)
                {
                    return BadRequest(new { message = "Invalid amount" });
                }

                decimal amount = request.Amount;

                string key = _configuration["Razorpay:Key"];
                string secret = _configuration["Razorpay:Secret"];

                RazorpayClient client = new RazorpayClient(key, secret);

                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", amount * 100);
                options.Add("currency", "INR");
                options.Add("receipt", "rcptid_" + Guid.NewGuid().ToString().Substring(0, 8));
                options.Add("payment_capture", 1);

                Razorpay.Api.Order order = client.Order.Create(options);

                return Ok(new
                {
                    orderId = order["id"].ToString(),
                    amount = amount * 100,
                    currency = "INR"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error creating Razorpay order",
                    error = ex.Message
                });
            }
        }


        [HttpPut("refund-security-deposit-changestatus/{bookingId}")]
        public async Task<IActionResult> RefundSecurityDeposit(int bookingId)
        {
            var deposit = await _context.SecurityDeposits
                .FirstOrDefaultAsync(d => d.BookingId == bookingId);

            if (deposit == null)
            {
                return NotFound(new { message = "Security deposit not found for this booking." });
            }

            // Already refunded check
            if (deposit.Status == SecurityDeposit.DepositStatus.Refunded)
            {
                return Ok(new { message = "Deposit is already refunded." });
            }

            deposit.Status = SecurityDeposit.DepositStatus.Refunded;
            deposit.RefundedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Security deposit status updated to Refunded successfully." });
        }

        [HttpPost("save-security-deposit")]
        public async Task<IActionResult> SaveSecurityDeposit([FromBody] SaveSecurityDepositDto dto)
        {
            if (dto == null || dto.Amount <= 0)
                return BadRequest("Invalid security deposit data");

            var existing = await _context.SecurityDeposits
                .FirstOrDefaultAsync(x => x.BookingId == dto.BookingId);

            if (existing != null)
                return Ok(new { message = "Security deposit already exists" });

            var deposit = new SecurityDeposit
            {
                BookingId = dto.BookingId,
                Amount = dto.Amount,
                Status = SecurityDeposit.DepositStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.SecurityDeposits.Add(deposit);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Security deposit saved successfully" });
        }


        [HttpGet("ShowDamageChargi")]
        public async Task<IActionResult> GetCompletedBookingswithamounnt()
        {
            var filteredBookings = await _context.Bookings
          .Include(b => b.DamageReport)
          .Include(b => b.FuelLog)
          .Include(b => b.Payment)
          .Include(b => b.SecurityDeposit)
          .Include(b => b.User)
          .Include(b => b.Vehicle)
              .ThenInclude(v => v.VehicleImages)
          .Where(b => b.Status == Booking.BookingStatus.Completed && b.Payment.Status == "Success")
          .Where(b => b.SecurityDeposit.Status == SecurityDeposit.DepositStatus.Refunded
                   || b.SecurityDeposit.Status == SecurityDeposit.DepositStatus.Pending)
          .Where(b =>
              (b.DamageReport != null && b.DamageReport.Status == DamageReport.DamageStatus.Reported)
              || (b.FuelLog != null && b.FuelLog.Status == FuelLog.FuelLogStatus.Active)
          )
          .Select(booking => new AdminUserOverallBookingInfoDto
          {
              // Booking
              BookingId = booking.BookingId,
              StartTime = booking.StartTime,
              EndTime = booking.EndTime,
              TotalAmount = booking.TotalAmount,
              BookingStatus = booking.Status.ToString(),
              BookingType = booking.BookingType,
              TotalDays = booking.TotalDays,
              TotalHours = booking.TotalHours,
              PerKelomeater = booking.PerKelomeater,
              BookingCreatedAt = booking.CreatedAt,

              // Payment
              PaymentStatus = booking.Payment.Status,
              PaymentAmount = booking.Payment.Amount,
              PaymentMethod = booking.Payment.PaymentMethod,
              TransactionId = booking.Payment.TransactionId,
              OrderId = booking.Payment.OrderId,
              PaymentCreatedAt = booking.Payment.CreatedAt,

              // Security Deposit
              SecurityDepositStatus = booking.SecurityDeposit.Status.ToString(),
              SecurityDepositAmount = booking.SecurityDeposit.Amount,
              RefundedAt = booking.SecurityDeposit.RefundedAt,
              SecurityDepositCreatedAt = booking.SecurityDeposit.CreatedAt,

              // User
              UserId = booking.User.UserId,
              FirstName = booking.User.Firstname,
              MiddleName = booking.User.Middlename,
              LastName = booking.User.Lastname,
              Email = booking.User.Email,
              Phone = booking.User.Phone,
              Address = booking.User.Address,
              City = booking.User.City,
              State = booking.User.State,
              Age = booking.User.Age,
              Gender = booking.User.Gender,
              UserImage = booking.User.Image,
              RoleName = booking.User.Role.RoleName.ToString(),
              UserCreatedAt = booking.User.CreatedAt,

              // Vehicle
              VehicleId = booking.Vehicle.VehicleId,
              VehicleType = booking.Vehicle.Vehicletype.ToString(),
              RegistrationNo = booking.Vehicle.RegistrationNo,
              FuelType = booking.Vehicle.FuelType.ToString(),
              SeatingCapacity = booking.Vehicle.SeatingCapacity,
              Mileage = booking.Vehicle.Mileage,
              Color = booking.Vehicle.Color,
              CarName = booking.Vehicle.CarName,
              BikeName = booking.Vehicle.BikeName,
              Availability = booking.Vehicle.Availability.ToString(),
              InsuranceStatus = booking.Vehicle.InsuranceStatus.ToString(),
              RcStatus = booking.Vehicle.RcStatus.ToString(),
              AcAvailability = booking.Vehicle.AcAvailability.HasValue ? booking.Vehicle.AcAvailability.ToString() : null,
              FuelTankCapacity = booking.Vehicle.FuelTankCapacity,
              YearOfManufacture = booking.Vehicle.YearOfManufacture,
              EngineCapacity = booking.Vehicle.EngineCapacity,
              VehicleSecurityDepositAmount = booking.Vehicle.SecurityDepositAmount,
              VehicleCreatedAt = booking.Vehicle.CreatedAt,
              VehicleImage = booking.Vehicle.VehicleImages
                  .OrderByDescending(img => img.VehicleImageId)
                  .Select(img => img.ImagePath)
                  .FirstOrDefault(),
              VehicleImages = booking.Vehicle.VehicleImages
                  .OrderBy(img => img.VehicleImageId)
                  .Select(img => img.ImagePath)
                  .ToList(),

              // ======== Damage Report Fields =========
              DamageReportStatus = booking.DamageReport != null ? booking.DamageReport.Status.ToString() : null,
              DamageDescription = booking.DamageReport != null ? booking.DamageReport.Description : null,
              DamageCharge = booking.DamageReport != null ? booking.DamageReport.RepairCost : null,
              DamageImage = booking.DamageReport != null ? booking.DamageReport.Image : null,

              // ======== Fuel Log Fields =========
              FuelLogStatus = booking.FuelLog != null ? booking.FuelLog.Status.ToString() : null,
              FuelGiven = booking.FuelLog != null ? booking.FuelLog.FuelGiven : null,
              FuelReturned = booking.FuelLog != null ? booking.FuelLog.FuelReturned : null,
              FuelCharge = booking.FuelLog != null ? booking.FuelLog.FuelCharge : null
          })
          .ToListAsync();

            return Ok(filteredBookings);
        }

    }
}
