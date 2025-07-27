namespace CamCare.Models
{
    public class ServiceResponse<TData>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public TData? Data { get; set; }
        public Exception? Exception { get; set; }


        public ServiceResponse(TData data)
        {
            Success = true;
            Data = data;
        }

        public ServiceResponse(string message, Exception? exception = null)
        {
            Success = false;
            Data = default;
            Exception = exception;
            Message = message;
        }
        public ServiceResponse(bool success, string message, TData? data, Exception exception)
        {
            Success = success;
            Message = message;
            Data = data;
            Exception = exception;
        }
    }
    public static class ServiceResponse
    {
        public static ServiceResponse<TData> Success<TData>(TData data)
            => new(data);
        public static ServiceResponse<Paginated<T>> Success<T>(Paginated<T> paginatedData)
            => new(paginatedData);
        public static ServiceResponse<TData> Failure<TData>(string message, Exception? exception = null)
            => new(message, exception);
    }
}
