namespace TxAssignmentServices.Services
{
    public class ServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }
    }

    public class ServiceResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
