using System;
using EZRide_Project.Data;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OwnerDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetOwnerDashboardSummary()
        {
            //  Token se OwnerId nikalo
            var ownerIdClaim = User.FindFirst("UserId");
            if (ownerIdClaim == null)
                return Unauthorized("Invalid token");

            int ownerId = int.Parse(ownerIdClaim.Value);

            //  Total Vehicles
            int totalVehicles = await _context.Vehicles
                .CountAsync(v => v.UserId == ownerId);

            //  Approved Vehicles
            int approvedVehicles = await _context.Vehicles
                .CountAsync(v => v.UserId == ownerId && v.IsApproved == true);

            //  Pending Vehicles
            int pendingVehicles = await _context.Vehicles
                .CountAsync(v => v.UserId == ownerId && v.IsApproved == false);

            //  Total Earnings
            decimal totalEarnings = await _context.OwnerPayments
                .Where(op => op.UserId == ownerId)
                .SumAsync(op => (decimal?)op.Amount) ?? 0;

            //Re-rent vehice count
            int reRentCount = await _context.OwnerPayments
       .CountAsync(op =>
           op.UserId == ownerId &&
           op.Status == OwnerPayment.PaymentStatus.ReRent
       );

            var result = new OwnerDashboardDto
            {
                TotalVehicles = totalVehicles,
                ApprovedVehicles = approvedVehicles,
                PendingVehicles = pendingVehicles,
                TotalEarnings = totalEarnings,
                ReRentCount = reRentCount
            };

            return Ok(result);
        }
    }
}

