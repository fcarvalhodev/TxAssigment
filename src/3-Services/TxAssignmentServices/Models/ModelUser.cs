namespace TxAssignmentServices.Models
{
    public class ModelUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; } = null;
    }
}
