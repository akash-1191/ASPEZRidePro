using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using EZRide_Project.DTO;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookings()
        {
            var bookings = await _context.Bookings
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    VehicleId = b.VehicleId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    TotalDistance = b.TotalDistance,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status.ToString(),
                    CreatedAt = b.CreatedAt
                }).ToListAsync();

            return Ok(bookings);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDTO>> GetBooking(int id)
        {
            var b = await _context.Bookings.FindAsync(id);

            if (b == null)
            {
                return NotFound();
            }

            var dto = new BookingDTO
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                VehicleId = b.VehicleId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                TotalDistance = b.TotalDistance,
                TotalAmount = b.TotalAmount,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDTO>> PostBooking(BookingDTO dto)
        {
            var booking = new Booking
            {
                UserId = dto.UserId,
                VehicleId = dto.VehicleId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TotalDistance = dto.TotalDistance,
                TotalAmount = dto.TotalAmount,
                Status = Enum.Parse<Booking.BookingStatus>(dto.Status),
                CreatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            dto.BookingId = booking.BookingId;
            dto.CreatedAt = booking.CreatedAt;

            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, dto);
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, BookingDTO dto)
        {
            if (id != dto.BookingId)
            {
                return BadRequest();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.StartTime = dto.StartTime;
            booking.EndTime = dto.EndTime;
            booking.TotalDistance = dto.TotalDistance;
            booking.TotalAmount = dto.TotalAmount;
            booking.Status = Enum.Parse<Booking.BookingStatus>(dto.Status);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Concurrency error occurred.");
            }

            return NoContent();
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
