using System.Security.Cryptography;
using System.Text;
using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
  
        public class PaymentService : IPaymentService
        {
            private readonly IPaymentRepository _repository;
        private readonly string _razorpaySecret;
        public PaymentService(IPaymentRepository repository, IConfiguration config)
            {
                _repository = repository;   
            _razorpaySecret = config["Razorpay:Secret"];
        }

        public async Task<bool> VerifyAndSavePaymentAsync(RazorpayVerificationDto dto)
        {
            string payload = dto.OrderId + "|" + dto.TransactionId;

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_razorpaySecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                //string generatedSignature = Convert.ToBase64String(hash);
                string generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                if (generatedSignature != dto.Signature)
                    return false;
            }

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = dto.Amount,
                TransactionId = dto.TransactionId,
                OrderId = dto.OrderId,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt
            };

            return await _repository.SavePaymentAsync(payment);
        }



    }



}
