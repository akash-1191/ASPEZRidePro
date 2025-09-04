using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
        public async Task<FeedbackDTO> AddFeedbackAsync(FeedbackDTO feedbackDto)
        {
            var feedback = new Feedback
            {
                UserId = feedbackDto.UserId ?? 0,
                FeedbackType = Enum.Parse<Feedback.Feedbacktype>(feedbackDto.FeedbackType ?? "Suggestion", true),
                Message = feedbackDto.Message ?? string.Empty,
                Status = Feedback.FeedbackStatus.Pending,   // default value
                CreatedAt = DateTime.UtcNow
            };

            var addedFeedback = await _feedbackRepository.AddFeedbackAsync(feedback);

            return new FeedbackDTO
            {
                FeedbackId = addedFeedback.FeedbackId,
                UserId = addedFeedback.UserId,
                FeedbackType = addedFeedback.FeedbackType.ToString(),
                Message = addedFeedback.Message,
                Status = addedFeedback.Status.ToString(),
                CreatedAt = addedFeedback.CreatedAt,
                FullName = $"{addedFeedback.User?.Firstname} {(string.IsNullOrWhiteSpace(addedFeedback.User?.Middlename) ? "" : addedFeedback.User.Middlename + " ")}{addedFeedback.User?.Lastname}".Trim(),
                MobileNumber = addedFeedback.User?.Phone,
                Email = addedFeedback.User?.Email
            };
        }

        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _feedbackRepository.GetAllFeedbacksAsync();

            return feedbacks.Select(f => new FeedbackDTO
            {
                FeedbackId = f.FeedbackId,
                UserId = f.UserId,
                FeedbackType = f.FeedbackType.ToString(),
                Message = f.Message,
                Status = f.Status.ToString(),
                CreatedAt = f.CreatedAt,
                FullName = $"{f.User?.Firstname} {(string.IsNullOrWhiteSpace(f.User?.Middlename) ? "" : f.User.Middlename + " ")}{f.User?.Lastname}".Trim(),
                MobileNumber = f.User?.Phone,
                Email = f.User?.Email
            });
        }
    }
}
