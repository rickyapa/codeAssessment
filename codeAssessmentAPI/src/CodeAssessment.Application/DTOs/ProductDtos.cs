namespace CodeAssessment.Application.DTOs;

public sealed record ProductDto(
    int Id,
    string Name,
    int Stock,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool Active);

public sealed record CreateProductRequest(string Name, int Stock = 0);

public sealed record UpdateProductRequest(string Name, int Stock);
