using MediatR;
using Microsoft.AspNetCore.Mvc;

public class Mediator
{
    // Step 1: Define a Request
    public record CreateOrderCommand(string ProductName, int Quantity) : IRequest<int>;

    // Step 2: Define a Handler
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        public async Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
        {
            Console.WriteLine($"Order created for {request.ProductName}, Qty: {request.Quantity}");
            var orderId = await Task.FromResult(101);
            return orderId;
        }
    }

    // Step 3: Send Command via Mediator
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var orderId = await _mediator.Send(command);
            return Ok(orderId);
        }
    }

}