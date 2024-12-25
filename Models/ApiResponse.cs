namespace UserManagementAPI.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        
        public static ApiResponse<T> CreateSuccess(T data, string message = "Operation successful")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }

        public static ApiResponse<T> CreateError(string message)
        {
            return new ApiResponse<T> { Success = false, Message = message };
        }
    }
}