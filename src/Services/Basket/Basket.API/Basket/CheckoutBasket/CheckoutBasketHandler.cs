using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket
{
    public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto) : ICommand<CheckoutBasketResult>;
    public record CheckoutBasketResult(bool IsSuccess);

    public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
    {
        public CheckoutBasketCommandValidator()
        {
            RuleFor(x => x.BasketCheckoutDto).NotNull().WithMessage("BasketCheckoutDto can't be null");
            RuleFor(x => x.BasketCheckoutDto.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }

    public class CheckoutBasketHandler(IBasketRepository repository, IPublishEndpoint publishEndpoint) : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
    {
        public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
        {
            // Get existing basket with total price
            ShoppingCart basket = await repository.GetBasket(command.BasketCheckoutDto.UserName, cancellationToken);
            if (basket == null)
                return new CheckoutBasketResult(false);

            // Set totalprice on basketcheckout event message
            BasketCheckoutEvent eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
            eventMessage.TotalPrice = basket.TotalPrice;

            // Add cartItens to the event message
            eventMessage.Items = new List<Item>();
            foreach (ShoppingCartItem item in basket.Items)
            {
                Item basketItem = new Item(item.ProductId, item.Quantity, item.Price);
                eventMessage.Items.Add(basketItem);
            }

            // Send basket checkout event to rabbitmq using masstransit
            await publishEndpoint.Publish(eventMessage, cancellationToken);

            // Delete the basket
            await repository.DeleteBasket(command.BasketCheckoutDto.UserName, cancellationToken);

            return new CheckoutBasketResult(true);
        }
    }
}
