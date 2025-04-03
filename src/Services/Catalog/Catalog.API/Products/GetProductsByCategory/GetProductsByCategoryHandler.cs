namespace Catalog.API.Products.GetProductsByCategory
{
    public record GetProductsByCategorQuery(string Category) : IQuery<GetProductsByCategoryResult>;

    public record GetProductsByCategoryResult(IEnumerable<Product> Products);

    internal class GetProductsByCategoryQueryHandler(IDocumentSession session, ILogger<GetProductsByCategoryQueryHandler> logger) : IQueryHandler<GetProductsByCategorQuery, GetProductsByCategoryResult>
    {
        public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategorQuery query, CancellationToken cancellationToken)
        {
            IReadOnlyList<Product> products = await session.Query<Product>()
                                                           .Where(p => p.Category.Contains(query.Category))
                                                           .ToListAsync();

            return new GetProductsByCategoryResult(products);
        }
    }
}
