using Application.Features.Commands.Customers.v1;
using Application.Features.Queries.Customers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers.v1
{
    public sealed class CustomerController(IMediator mediator) : ApiController(mediator)
    {
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer([FromBody] DeleteCustomerRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] ResultCustomerRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
    }
}