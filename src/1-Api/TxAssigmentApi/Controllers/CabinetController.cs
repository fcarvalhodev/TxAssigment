using Microsoft.AspNetCore.Mvc;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;
using System;
using System.Threading.Tasks;

namespace TxAssigmentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CabinetController : ControllerBase
    {
        private readonly IServiceCabinet _serviceCabinet;

        public CabinetController(IServiceCabinet serviceCabinet)
        {
            _serviceCabinet = serviceCabinet;
        }

        // POST: /Cabinet
        [HttpPost]
        public async Task<IActionResult> CreateCabinet([FromBody] ModelCabinet cabinet)
        {
            var result = await _serviceCabinet.CreateCabinet(cabinet);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        // PUT: /Cabinet/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCabinet(Guid id, [FromBody] ModelCabinet cabinet)
        {
            var result = await _serviceCabinet.UpdateCabinet(id, cabinet);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        // DELETE: /Cabinet/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabinet(Guid id)
        {
            var result = await _serviceCabinet.DeleteCabinet(id);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        // GET: /Cabinet/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCabinetById(Guid id)
        {
            var result = await _serviceCabinet.GetCabinetById(id);
            if (result.Success)
                return Ok(result.Data);
            else
                return NotFound(result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCabinets()
        {
            var result = await _serviceCabinet.GetAllCabinets();
            if (result.Success)
                return Ok(result.Data);
            else
                return NotFound(result.Message);
        }
    }
}
