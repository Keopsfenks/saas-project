using Application.Features.Commands.Orders.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers.v1
{
    public sealed class ShipmentController(IMediator mediator) : ApiController(mediator)
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }

    }
}