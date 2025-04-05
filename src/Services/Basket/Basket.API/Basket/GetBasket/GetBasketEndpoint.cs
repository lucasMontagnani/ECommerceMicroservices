namespace Basket.API.Basket.GetBasket
{
    //public record GetBasketRequest(string UserName);
    public record GetBasketResponse(ShoppingCart Cart);

    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
            {
                GetBasketResult result = await sender.Send(new GetBasketQuery(userName));

                GetBasketResponse response = result.Adapt<GetBasketResponse>();

                return Results.Ok(response);
            })
            .WithName("GetBasketByUsername")
            .Produces<GetBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Basket By Username")
            .WithDescription("Get Basket By Username");
        }
    }
}
