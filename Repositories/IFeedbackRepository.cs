using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IFeedbackRepository
    {
        Task<Feedback> AddFeedbackAsync(Feedback feedback);
        Task<IEnumerable<Feedback>> GetAllFeedbacksAsync();
    }
}
