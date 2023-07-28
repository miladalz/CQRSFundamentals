using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Application.Commands;
using Ordering.API.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        [Route("Order")]
        [HttpPost]
        public async Task<ActionResult<Result>> CreateOrderFromBasketDataAsync([FromBody] OrderDTO orderDTO)
        {
            var createOrderCommand = new CreateOrderCommand(orderDTO.Items, orderDTO.UserId, orderDTO.UserName, orderDTO.City, orderDTO.Street, orderDTO.Country, orderDTO.ZipCode);
            var result = await _mediator.Send(createOrderCommand);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok();
        }
    }
}
