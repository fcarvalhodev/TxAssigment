namespace TxAssignmentInfra.Repositories
{
    public class RepositoryResponse<T> : RepositoryResponse
    {
        public T? Data { get; set; }
    }

    public class RepositoryResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
