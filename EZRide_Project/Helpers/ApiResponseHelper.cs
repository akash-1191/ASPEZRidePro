using EZRide_Project.Model;

namespace EZRide_Project.Helpers
{
    public class ApiResponseHelper
    {


        public static ApiResponseModel Success(string message = "Success", int statusCode = 200)
        {
            return new ApiResponseModel
            {
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static ApiResponseModel Fail(string message = "Something went wrong", int statusCode = 400)
        {
            return new ApiResponseModel
            {
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode
            };
        }


        public static ApiResponseModel UserDataNull()
        {
            return Fail("User data cannot be null.", 400);
        }
        public static ApiResponseModel EmailAlreadyExists()
        {
            return Fail("Email already exists.", 409); 
        }

        public static ApiResponseModel NotFound(string entityName = "Data")
        {
            return Fail($"{entityName} not found.", 404);
        }

        public static ApiResponseModel Unauthorized(string message = "Unauthorized access.")
        {
            return Fail(message, 401);
        }

        public static ApiResponseModel Forbidden(string message = "Access denied.")
        {
            return Fail(message, 403);
        }

        public static ApiResponseModel ServerError(string message = "Internal server error occurred.")
        {
            return Fail(message, 500);
        }
        public static ApiResponseModel ValidationFailed(string message = "Validation failed.")
        {
            return Fail(message, 422);
        }
    }
}
