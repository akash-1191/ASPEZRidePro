using System.Security.Cryptography;
using System.Text;
using EZRide_Project.Data;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace EZRide_Project.Services
{
    public class OwnerPaymentService : IOwnerPaymentService
    {
        private readonly IOwnerPaymentRepository _repo;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public OwnerPaymentService(IOwnerPaymentRepository repo,
                                   ApplicationDbContext context,
                                   IConfiguration config)
        {
            _repo = repo;
            _context = context;
            _config = config;
        }

        public async Task<bool> VerifyPaymentAsync(OwnerPaymentVerifyDto dto)
        {
            // Step 1: Verify Razorpay Signature
            if (!VerifySignature(dto))
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Step 2: Insert Payment
                var payment = new OwnerPayment
                {
                    UserId = dto.OwnerId,
                    VehicleId = dto.VehicleId,
                    Amount = dto.Amount,
                    PaymentType = OwnerPayment.paymentType.Online,
                    Status = OwnerPayment.PaymentStatus.Paid,
                    CreatedAt = DateTime.Now
                };

                await _repo.AddPaymentAsync(payment);

                // Step 3: Update Availability Status to Expired
                var updateResult = await _repo.UpdateVehicleAvailabilityStatusAsync(dto.VehicleId, dto.OwnerId);

                if (!updateResult)
                {
                    // Log warning but don't fail the payment
                    Console.WriteLine($"Warning: Could not update availability for VehicleId: {dto.VehicleId}, OwnerId: {dto.OwnerId}");
                }

                await _repo.ClearSecurityDepositAmountAsync(dto.VehicleId);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private bool VerifySignature(OwnerPaymentVerifyDto dto)
        {
            string keySecret = _config["Razorpay:Secret"];
            string payload = dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

            string generatedSignature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(keySecret)))
            {
                generatedSignature = BitConverter
                    .ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)))
                    .Replace("-", "").ToLower();
            }

            return generatedSignature == dto.RazorpaySignature;
        }

        public async Task<List<OwnerPaymentDto>> GetOwnerPaymentsAsync(int ownerId)
        {
            var payments = await _repo.GetOwnerPaymentsAsync(ownerId);
            var result = new List<OwnerPaymentDto>();

            foreach (var payment in payments)
            {
                var dto = new OwnerPaymentDto
                {
                    OwnerPaymentId = payment.OwnerPaymentId,
                    UserId = payment.UserId,
                    VehicleId = payment.VehicleId,
                    Amount = payment.Amount,
                    PaymentType = payment.PaymentType.ToString(),
                    Status = payment.Status.ToString(),
                    CreatedAt = payment.CreatedAt
                };

                // Get Vehicle Details
                var vehicle = await _repo.GetVehicleByIdAsync(payment.VehicleId);
                if (vehicle != null)
                {
                    dto.VehicleName = vehicle.Vehicletype == Vehicle.VehicleType.Car
                        ? vehicle.CarName
                        : vehicle.BikeName;
                    dto.RegistrationNo = vehicle.RegistrationNo;
                    dto.VehicleType = vehicle.Vehicletype.ToString();
                }

                // **CRITICAL FIX: Find availability active AT THE TIME OF PAYMENT**
                var availability = await _repo.GetAvailabilityAtDateAsync(payment.VehicleId, payment.CreatedAt);

                if (availability != null)
                {
                    dto.AvailableDays = availability.AvailableDays;
                    dto.EffectiveFrom = availability.EffectiveFrom;
                    dto.EffectiveTo = availability.EffectiveTo;
                    dto.VehicleAmountPerDay = availability.vehicleAmountPerDay;
                    dto.AvailabilityStatus = availability.Status.ToString();

                    // Calculate total rent amount
                    if (availability.AvailableDays > 0 && availability.vehicleAmountPerDay > 0)
                    {
                        dto.TotalRentAmount = availability.AvailableDays * availability.vehicleAmountPerDay;
                    }

                    // Create formatted rental period
                    dto.RentalPeriod = $"{availability.EffectiveFrom:dd-MMM-yyyy} to {availability.EffectiveTo:dd-MMM-yyyy}";
                }
                else
                {
                    // If no availability found, try to get latest availability for this vehicle
                    var latestAvailability = await _repo.GetLatestAvailabilityAsync(payment.VehicleId);
                    if (latestAvailability != null)
                    {
                        dto.AvailableDays = latestAvailability.AvailableDays;
                        dto.EffectiveFrom = latestAvailability.EffectiveFrom;
                        dto.EffectiveTo = latestAvailability.EffectiveTo;
                        dto.VehicleAmountPerDay = latestAvailability.vehicleAmountPerDay;
                        dto.AvailabilityStatus = latestAvailability.Status.ToString();

                        if (latestAvailability.AvailableDays > 0 && latestAvailability.vehicleAmountPerDay > 0)
                        {
                            dto.TotalRentAmount = latestAvailability.AvailableDays * latestAvailability.vehicleAmountPerDay;
                        }

                        dto.RentalPeriod = $"{latestAvailability.EffectiveFrom:dd-MMM-yyyy} to {latestAvailability.EffectiveTo:dd-MMM-yyyy}";
                    }
                }

                result.Add(dto);
            }

            return result;
        }
    }
    
}


