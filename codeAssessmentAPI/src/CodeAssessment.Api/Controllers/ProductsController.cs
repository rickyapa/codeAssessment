using CodeAssessment.Application.Commands;
using CodeAssessment.Application.DTOs;
using CodeAssessment.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CodeAssessment.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(
    GetProductsQueryHandler getProducts,
    GetProductByIdQueryHandler getProductById,
    CreateProductCommandHandler createProduct,
    UpdateProductCommandHandler updateProduct,
    DeleteProductCommandHandler deleteProduct,
    AddStockCommandHandler addStock,
    DecrementStockCommandHandler decrementStock,
    SearchProductsQueryHandler searchProducts,
    GetProductsByStockRangeQueryHandler getProductsByStockRange) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await getProducts.HandleAsync(new GetProductsQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await getProductById.HandleAsync(new GetProductByIdQuery(id), cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Stock);
        var product = await createProduct.HandleAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(id, request.Name, request.Stock);
        var product = await updateProduct.HandleAsync(command, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);
        return await deleteProduct.HandleAsync(command, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/decrement-stock/{quantity:int}")]
    public async Task<ActionResult<ProductDto>> DecrementStock(int id, int quantity, CancellationToken cancellationToken)
    {
        var product = await decrementStock.HandleAsync(new DecrementStockCommand(id, quantity), cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost("{id:int}/add-to-stock/{quantity:int}")]
    public async Task<ActionResult<ProductDto>> AddToStock(int id, int quantity, CancellationToken cancellationToken)
    {
        var product = await addStock.HandleAsync(new AddStockCommand(id, quantity), cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> Search([FromQuery] string? name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("The name query parameter is required.");
        }

        return Ok(await searchProducts.HandleAsync(new SearchProductsQuery(name), cancellationToken));
    }

    [HttpGet("stock-level")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetByStockLevel(
        [FromQuery] int? min,
        [FromQuery] int? max,
        CancellationToken cancellationToken)
    {
        if (min is null || max is null)
        {
            return BadRequest("Both min and max query parameters are required.");
        }

        return Ok(await getProductsByStockRange.HandleAsync(
            new GetProductsByStockRangeQuery(min.Value, max.Value),
            cancellationToken));
    }
}
