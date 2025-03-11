using Application.Features.Commands.Cargoes.v1;
using Application.Features.Queries.Cargoes.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers.v1
{
    public sealed class CargoController(IMediator mediator) : ApiController(mediator)
    {
        [HttpPost]
        public async Task<IActionResult> CreateCargo([FromBody] CreateCargoRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCargo([FromBody] UpdateCargoRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCargo([FromBody] DeleteCargoRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCargos([FromQuery] ResultCargoRequest request)
        {
            var response = await Mediator.Send(request);

            if (response.IsSuccessful)
                return Ok(response);

            return StatusCode(response.StatusCode, response);
        }
    }
}