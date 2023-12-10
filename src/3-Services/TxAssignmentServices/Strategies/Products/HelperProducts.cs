using TxAssignmentServices.Models;

namespace TxAssignmentServices.Strategies.Products
{
    internal static class HelperProducts
    {
        internal static (bool isValid, string message) ModelIsValid(ModelProduct product)
        {

            if (product == null)
                return (false, "The product (SKU) can not be empty");

            var validateJanCode = ValidateJanCode(product.JanCode);

            if (!validateJanCode.isValid)
                return validateJanCode;

            if (string.IsNullOrEmpty(product.Name))
                return (false, "The name of the product is mandatory");

            if (product.Name.Length > 255)
                return (false, "The name of the product is too large, maximum of 255 characteres");

            if (product.Width < 0)
                return (false, "The Width of the product can not be negative");

            if (product.Height < 0)
                return (false, "The Heigth of the product can not be negative");

            if (product.Depth < 0)
                return (false, "The Depth of the product can not be negative");

            if (product.Size < 0)
                return (false, "The Size of the product can not be negative");

            if (product.TimeStamp < 0)
                return (false, "The TimeStamp of the product is invalid.");

            if (!string.IsNullOrEmpty(product.ImageUrl) && !Uri.IsWellFormedUriString(product.ImageUrl, UriKind.Absolute))
                return (false, "The ImageUrl of the product is not a valid URL.");

            return (true, "");
        }

        internal static (bool isValid, string message) ValidateJanCode(string janCode)
        {
            if (string.IsNullOrEmpty(janCode))
                return (false, "The JanCode can not be empty");

            if (!long.TryParse(janCode, out _))
                return (false, "The JanCode should contain only digits.");

            if (janCode.Length != 13)
                return (false, "The JanCode size is incorrect, please insert the properly 13 digits of a JanCode");

            return (true, "");
        }
    }
}
