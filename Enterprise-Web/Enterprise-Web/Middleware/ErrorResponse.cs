namespace Enterprise_Web.Middleware
{
    public class ErrorResponse<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public static ErrorResponse<T> Fail(string errorMessage)
        {
            return new ErrorResponse<T> { Succeeded = false, Message = errorMessage };
        }
        public static ErrorResponse<T> Success(T data)
        {
            return new ErrorResponse<T> { Succeeded = true};
        }
    }
}
