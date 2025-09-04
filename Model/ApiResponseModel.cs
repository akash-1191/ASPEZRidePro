namespace EZRide_Project.Model
{
    public class ApiResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public object Data { get; set; }

    }
}
