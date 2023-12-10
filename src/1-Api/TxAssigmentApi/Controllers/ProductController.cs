using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssigmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IServiceProduct _serviceProduct;

        public ProductController(IServiceProduct serviceProduct)
        {
            _serviceProduct = serviceProduct;
        }

        [HttpGet("{janCode}")]
        public async Task<IActionResult> GetProductByJanCode(string janCode)
        {
            var response = await _serviceProduct.GetProductByJanCode(janCode);

            if (response.Success)
                return Ok(response.Data);
            else
                return NotFound(response.Message);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ModelProduct productModel)
        {
            var response = await _serviceProduct.CreateProduct(productModel);

            if (response.Success)
                return CreatedAtAction(nameof(GetProductByJanCode), new { janCode = productModel.JanCode }, productModel);
            else
                return BadRequest(response.Message);
        }

        [Authorize]
        [HttpPut("{janCode}")]
        public async Task<IActionResult> UpdateProduct(string janCode, [FromBody] ModelProduct productModel)
        {
            var response = await _serviceProduct.UpdateProduct(janCode, productModel);

            if (response.Success)
                return Ok(response.Message);
            else
                return BadRequest(response.Message);
        }

        [Authorize]
        [HttpDelete("{janCode}")]
        public async Task<IActionResult> DeleteProduct(string janCode)
        {
            var response = await _serviceProduct.DeleteProduct(janCode);

            if (response.Success)
                return Ok(response.Message);
            else
                return BadRequest(response.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _serviceProduct.GetAllProducts();

            if (response.Success)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
    }
}
