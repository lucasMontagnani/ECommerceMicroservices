namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);

    public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.Cart).NotEmpty().WithMessage("The Shopping Cart can not be null");
            RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("The UserName is required");
        }
    }

    public class StoreBasketCommandHandler(IBasketRepository repository) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = command.Cart;

            // Store basket in database (use Marten upsert: if exist = update, if not = create)
            await repository.StoreBasket(shoppingCart, cancellationToken);

            // TODO: Update cache

            return new StoreBasketResult(command.Cart.UserName);
        }
    }
}
