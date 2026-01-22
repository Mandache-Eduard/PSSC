using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Models;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;
using OrderManagement.Domain.Workflows;
using static OrderManagement.Domain.Models.OrderPlacedEvent;

namespace OrderManagement.Api.Controllers
{
    /// <summary>
    /// Controller for managing orders
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly PlaceOrderWorkflow _placeOrderWorkflow;
        private readonly ModifyOrderWorkflow _modifyOrderWorkflow;
        private readonly CancelOrderWorkflow _cancelOrderWorkflow;
        private readonly ReturnOrderWorkflow _returnOrderWorkflow;

        public OrdersController(
            ILogger<OrdersController> logger,
            PlaceOrderWorkflow placeOrderWorkflow,
            ModifyOrderWorkflow modifyOrderWorkflow,
            CancelOrderWorkflow cancelOrderWorkflow,
            ReturnOrderWorkflow returnOrderWorkflow)
        {
            _logger = logger;
            _placeOrderWorkflow = placeOrderWorkflow;
            _modifyOrderWorkflow = modifyOrderWorkflow;
            _cancelOrderWorkflow = cancelOrderWorkflow;
            _returnOrderWorkflow = returnOrderWorkflow;
        }

        /// <summary>
        /// Get all orders from the database
        /// </summary>
        /// <returns>List of all orders</returns>
        /// <response code="200">Returns the list of orders</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrders([FromServices] IOrdersRepository ordersRepository)
        {
            _logger.LogInformation("Getting all orders");
            
            var orders = await ordersRepository.GetAllOrdersAsync();
            
            return Ok(new
            {
                totalCount = orders.Count(),
                orders = orders.Select(o => new
                {
                    orderNumber = o.OrderNumber,
                    totalAmount = o.TotalAmount,
                    status = o.Status,
                    orderDate = o.OrderDate,
                    shippingCity = o.ShippingCity,
                    shippingCountry = o.ShippingCountry
                })
            });
        }

        /// <summary>
        /// Get an order by order number
        /// </summary>
        /// <param name="orderNumber">The order number</param>
        /// <param name="ordersRepository">Orders repository (injected)</param>
        /// <returns>Order details</returns>
        /// <response code="200">Returns the order</response>
        /// <response code="404">Order not found</response>
        [HttpGet("{orderNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(
            string orderNumber,
            [FromServices] IOrdersRepository ordersRepository)
        {
            _logger.LogInformation("Getting order {OrderNumber}", orderNumber);

            if (!OrderNumber.TryParse(orderNumber, out var validOrderNumber))
            {
                return BadRequest(new { error = "Invalid order number format" });
            }

            var (exists, details) = await ordersRepository.GetOrderByNumberAsync(validOrderNumber!);

            if (!exists || details == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            return Ok(new
            {
                orderNumber = validOrderNumber.Value,
                totalAmount = details.TotalAmount,
                orderDate = details.OrderDate,
                status = details.Status
            });
        }

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="request">Order details</param>
        /// <returns>Order placement result</returns>
        /// <response code="200">Order placed successfully</response>
        /// <response code="400">Invalid order data</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            _logger.LogInformation("Placing new order");

            // Map API model to domain model
            var unvalidatedOrderLines = request.OrderLines
                .Select(line => new UnvalidatedOrderLine(line.ProductCode, line.Quantity.ToString()))
                .ToList()
                .AsReadOnly();

            var command = new PlaceOrderCommand(
                unvalidatedOrderLines,
                request.Street,
                request.City,
                request.PostalCode,
                request.Country
            );

            // Execute workflow
            var result = await _placeOrderWorkflow.ExecuteAsync(command);

            // Map result to HTTP response
            return result switch
            {
                OrderPlacedSuccessEvent successEvent => Ok(new
                {
                    message = "Order placed successfully",
                    orderNumber = successEvent.OrderNumber,
                    totalPrice = successEvent.TotalPrice,
                    placedDate = successEvent.PlacedDate
                }),
                OrderPlacedFailedEvent failedEvent => BadRequest(new
                {
                    error = "Order placement failed",
                    reasons = failedEvent.Reasons
                }),
                _ => StatusCode(500, new { error = "Unexpected error occurred" })
            };
        }

        /// <summary>
        /// Modify an existing order
        /// </summary>
        /// <param name="orderNumber">Order number to modify</param>
        /// <param name="newOrderLines">New order lines</param>
        /// <returns>Modification result</returns>
        /// <response code="200">Order modified successfully</response>
        /// <response code="400">Invalid modification data</response>
        [HttpPut("{orderNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyOrder(
            string orderNumber,
            [FromBody] List<InputOrderLine> newOrderLines)
        {
            _logger.LogInformation("Modifying order {OrderNumber}", orderNumber);

            var unvalidatedLines = newOrderLines
                .Select(line => new UnvalidatedOrderLine(line.ProductCode, line.Quantity.ToString()))
                .ToList()
                .AsReadOnly();

            var command = new ModifyOrderCommand(orderNumber, unvalidatedLines);
            var result = await _modifyOrderWorkflow.ExecuteAsync(command);

            return result switch
            {
                OrderModifiedEvent.OrderModifiedSuccessEvent successEvent => Ok(new
                {
                    message = "Order modified successfully",
                    orderNumber = successEvent.OrderNumber,
                    newTotalPrice = successEvent.NewTotalPrice,
                    priceDifference = successEvent.PriceDifference
                }),
                OrderModifiedEvent.OrderModifiedFailedEvent failedEvent => BadRequest(new
                {
                    error = "Order modification failed",
                    reasons = failedEvent.Reasons
                }),
                _ => StatusCode(500, new { error = "Unexpected error occurred" })
            };
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="orderNumber">Order number to cancel</param>
        /// <param name="reason">Cancellation reason</param>
        /// <returns>Cancellation result</returns>
        /// <response code="200">Order cancelled successfully</response>
        /// <response code="400">Invalid cancellation request</response>
        [HttpDelete("{orderNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(
            string orderNumber,
            [FromQuery] string reason)
        {
            _logger.LogInformation("Cancelling order {OrderNumber}", orderNumber);

            var command = new CancelOrderCommand(orderNumber, reason);
            var result = await _cancelOrderWorkflow.ExecuteAsync(command);

            return result switch
            {
                OrderCancelledEvent.OrderCancelledSuccessEvent successEvent => Ok(new
                {
                    message = "Order cancelled successfully",
                    orderNumber = successEvent.OrderNumber,
                    refundAmount = successEvent.RefundAmount
                }),
                OrderCancelledEvent.OrderCancelledFailedEvent failedEvent => BadRequest(new
                {
                    error = "Order cancellation failed",
                    reason = failedEvent.Reason
                }),
                _ => StatusCode(500, new { error = "Unexpected error occurred" })
            };
        }

        /// <summary>
        /// Return an order
        /// </summary>
        /// <param name="orderNumber">Order number to return</param>
        /// <param name="returnReason">Return reason</param>
        /// <param name="returnItems">Items to return</param>
        /// <returns>Return result</returns>
        /// <response code="200">Order returned successfully</response>
        /// <response code="400">Invalid return request</response>
        [HttpPost("{orderNumber}/return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReturnOrder(
            string orderNumber,
            [FromQuery] string returnReason,
            [FromBody] List<UnvalidatedReturnItem> returnItems)
        {
            _logger.LogInformation("Processing return for order {OrderNumber}", orderNumber);

            var command = new ReturnOrderCommand(orderNumber, returnReason, returnItems.AsReadOnly());
            var result = await _returnOrderWorkflow.ExecuteAsync(command);

            return result switch
            {
                OrderReturnedEvent.OrderReturnedSuccessEvent successEvent => Ok(new
                {
                    message = "Order returned successfully",
                    orderNumber = successEvent.OrderNumber,
                    refundAmount = successEvent.RefundAmount,
                    returnNumber = successEvent.ReturnNumber
                }),
                OrderReturnedEvent.OrderReturnedFailedEvent failedEvent => BadRequest(new
                {
                    error = "Order return failed",
                    reasons = failedEvent.Reasons
                }),
                _ => StatusCode(500, new { error = "Unexpected error occurred" })
            };
        }
    }
}

