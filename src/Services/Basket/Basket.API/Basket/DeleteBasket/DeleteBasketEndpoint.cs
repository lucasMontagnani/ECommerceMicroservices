﻿
namespace Basket.API.Basket.DeleteBasket
{
    //public record DeleteBasketRequest(string UserName);
    public record DeleteBasketResponse(bool IsSuccess);

    public class DeleteBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
            {
                DeleteBasketResult result = await sender.Send(new DeleteBasketCommand(userName));

                DeleteBasketResponse response = result.Adapt<DeleteBasketResponse>();

                return Results.Ok(response);
            })
            .WithName("DeleteBasketByUsername")
            .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Basket By Username")
            .WithDescription("Delete Basket By Username");
        }
    }
}
