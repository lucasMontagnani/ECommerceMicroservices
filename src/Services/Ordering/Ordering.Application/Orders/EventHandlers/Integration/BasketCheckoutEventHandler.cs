using BuildingBlocks.Messaging.Events;
using MassTransit;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Orders.EventHandlers.Integration
{
    public class BasketCheckoutEventHandler(ISender sender, ILogger<BasketCheckoutEventHandler> logger) : IConsumer<BasketCheckoutEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            // TODO: Create new order and start order fullfillment process
            logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

            CreateOrderCommand command = MapToCreateOrderCommand(context.Message);
            await sender.Send(command);
        }

        private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
        {
            // Create full order with incoming event data
            AddressDto addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine, message.Country, message.State, message.ZipCode);
            PaymentDto paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration, message.CVV, message.PaymentMethod);
            Guid orderId = Guid.NewGuid();

            List<OrderItemDto> basketItens = [];
            foreach (Item item in message.Items)
            {
                OrderItemDto orderItem = new OrderItemDto(orderId, item.ProductId, item.Quantity, item.Price);
                basketItens.Add(orderItem);
            }

            OrderDto orderDto = new OrderDto(
                Id: orderId,
                CustomerId: message.CustomerId,
                OrderName: message.UserName,
                ShippingAddress: addressDto,
                BillingAddress: addressDto,
                Payment: paymentDto,
                Status: Ordering.Domain.Enums.OrderStatus.Pending,
                OrderItems: basketItens
            );

            return new CreateOrderCommand(orderDto);
        }
    }
}
