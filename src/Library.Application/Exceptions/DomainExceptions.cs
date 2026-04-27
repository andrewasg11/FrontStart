namespace Library.Application.Exceptions
{
    // Handled by Middleware ------

    // 404: Not Found
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    // 404: Bad Request
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }

    // 409: Data Conflict
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
