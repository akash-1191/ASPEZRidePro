using EZRide_Project.DTO;
using EZRide_Project.Repositories;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Drawing;
using System.Xml.Linq;

namespace EZRide_Project.Services
{
    public class PaymentReceiptService : IPaymentReceiptService
    {
        private readonly IPaymentReceiptRepository _repository;
        private readonly EmailService _emailService;
        public PaymentReceiptService(IPaymentReceiptRepository repository, EmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public async Task<PaymentReceiptDto?> GetPaymentReceiptAsync(int userId, int bookingId)
        {
            return await _repository.GetPaymentReceiptByUserIdAndBookingIdAsync(userId, bookingId);
        }

        public byte[] GeneratePdf(PaymentReceiptDto dto)
        {
            using var stream = new MemoryStream();
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // Fonts
            var titleFont = new XFont("Verdana", 20, XFontStyle.Bold);
            var subtitleFont = new XFont("Verdana", 14, XFontStyle.Bold);
            var font = new XFont("Verdana", 12, XFontStyle.Regular);
            var footerFont = new XFont("Verdana", 10, XFontStyle.Italic);

            // Draw EZRide Title (Top Center)
            gfx.DrawString("EZRide", titleFont, XBrushes.DarkBlue, new XRect(0, 30, page.Width, 0), XStringFormats.TopCenter);

            // Subtitle
            gfx.DrawString("Payment Receipt", subtitleFont, XBrushes.Black, new XRect(0, 60, page.Width, 0), XStringFormats.TopCenter);

            int y = 100;
            int lineSpacing = 25;

            void DrawLine(string label, string value)
            {
                gfx.DrawString($"{label}:", font, XBrushes.Black, 40, y);
                gfx.DrawString($"{value}", font, XBrushes.Black, 180, y);
                y += lineSpacing;
            }

            // Draw receipt details
            DrawLine("Full Name", dto.FullName);
            DrawLine("Email", dto.Email);
            DrawLine("Phone", dto.PhoneNumber);
            DrawLine("Vehicle", dto.VehicleName);
            DrawLine("Vehicle Type", dto.VehicleType);
            DrawLine("Fuel Type", dto.FuelType);
            DrawLine("Registration No", dto.RegistrationNumber);
            DrawLine("Start Date & Time", dto.StartDateTime.ToString("f"));
            DrawLine("Drop Date & Time", dto.DropDateTime.ToString("f"));
            DrawLine("Booking Created At", dto.BookingCreatedAt.ToString("f"));
            DrawLine("Booking Status", dto.BookingStatus);
            DrawLine("Payment Method", dto.PaymentMethod);
            DrawLine("Transaction ID", dto.TransactionId);
            DrawLine("Payment Status", dto.PaymentStatus);
            DrawLine("Total Payment", $"₹{dto.TotalPayment:F2}");
            DrawLine("Security Deposit", $"₹{dto.SecurityDepositAmount:F2}");

            string appreciationMessage = "★ Thank you for booking with EZRide. Have a great day ahead! ★";
            var appreciationFont = new XFont("Verdana", 11, XFontStyle.BoldItalic);

            gfx.DrawString(
                appreciationMessage,
                appreciationFont,
                new XSolidBrush(XColor.FromArgb(0, 102, 51)), // dark green color
                new XRect(0, page.Height - 80, page.Width, 0),
                XStringFormats.TopCenter
            );
            // Save document
            document.Save(stream);
            return stream.ToArray();
        }

        //pdf send to the email 
        public async Task SendReceiptEmailAsync(string toEmail, int userId, int bookingId)
        {
            var dto = await GetPaymentReceiptAsync(userId, bookingId);
            if (dto == null) return;

            var pdfBytes = GeneratePdf(dto);

            string subject = "Your EZRide Payment Receipt";
            string body = $"Dear {dto.FullName},<br/><br/>Thank you for booking with EZRide. Please find your payment receipt attached.<br/><br/>Regards,<br/>EZRide Team";

            await _emailService.SendEmailWithAttachmentAsync(toEmail, subject, body, pdfBytes, $"EZRide_Receipt_{dto.FullName}.pdf");
        }


    }
}
