using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Services
{
    public interface IFeedbackService
    {
        Task<FeedbackDTO> AddFeedbackAsync(FeedbackDTO feedbackDto);
        Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync();
    }
}
