namespace StackBook.Services
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? Token { get; set; } // optional
        public int StatusCode { get; set; } = 200; // thêm HTTP code nếu muốn
        public Dictionary<string, string[]>? Errors { get; set; } // optional

        public static ServiceResponse<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static ServiceResponse<T> Fail(string message, int statusCode = 400) =>
            new() { Success = false, Message = message, StatusCode = statusCode };
    }
}