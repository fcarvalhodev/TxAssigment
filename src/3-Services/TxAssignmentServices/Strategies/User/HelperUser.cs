using TxAssignmentServices.Models;

namespace TxAssignmentServices.Strategies.User
{
    internal static class HelperUser
    {
        internal static (bool isValid, string message) ModelIsValid(ModelUser user)
        {

            if (user == null)
                return (false, "The user can not be null");

            if(string.IsNullOrEmpty(user.Email))
                return (false, "The user email is mandatory");

            if (!IsValidEmail(user.Email))
                return (false, "The user email format is invalid");


            if (string.IsNullOrEmpty(user.Password))
                return (false, "The user password is mandatory");

            if (!IsPasswordStrong(user.Password))
                return (false, "The password is not strong enough, password must have at least 8 characters, number, digits, uppercase letter, lowercase letter and a non-alphanumeric character.");

            return (true, "");
        }

        private static bool IsPasswordStrong(string password)
        {
            if (password.Length < 8)
                return false;

            if (!password.Any(char.IsUpper))
                return false;

            if (!password.Any(char.IsLower))
                return false;

            if (!password.Any(char.IsDigit))
                return false;

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return false;

            return true;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
