using Application.Features.Commands.Shippers.v1;
using Application.Features.Queries.Shippers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers.v1
{
    public sealed class ShipperController(IMediator mediator) : ApiController(mediator)
    {
        [HttpPost]
        public async Task<IActionResult> CreateShipper([FromBody] CreateShipperRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateShipper([FromBody] UpdateShipperRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteShipper([FromBody] DeleteShipperRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllShippers([FromQuery] ResultShipperRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
    }
}