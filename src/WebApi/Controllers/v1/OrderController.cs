using Application.Dtos;
using Application.Features.Commands.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers.v1
{
    public sealed class OrderController(IMediator mediator) : ApiController(mediator)
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {

            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
    }
}