using Magalu.API.Models;
using Magalu.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Magalu.API.Controllers.V1
{
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private ClientService ClientService { get; set; }
        
        public ClientController(ClientService clientService)
        {
            ClientService = clientService;
            
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Client model)
        {
            try
            {
                await ClientService.Create(model.Email, model.Name);
                return Ok(model.Email);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{email}")]
        public async Task<IActionResult> Put([FromRoute] string email, [FromForm] Client model)
        {
            _ = email ?? throw new ArgumentException("informe um email", nameof(email));
            try
            {
                await ClientService.Update(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete([FromRoute] string email)
        {
            try
            {
                await ClientService.Delete(email);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await ClientService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> Get([FromRoute] string email)
        {
            try
            {
                var result = await ClientService.GetByEmail(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{email}/product")]
        public async Task<IActionResult> ProductAdd([FromRoute] string email, [FromForm] string productId)
        {
            try
            {
                var result = await ClientService.AddProduct(email, productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{email}/product")]
        public async Task<IActionResult> ProductDelete([FromRoute] string email, [FromForm] string productId)
        {
            try
            {
                var result = await ClientService.RemoveProduct(email, productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
