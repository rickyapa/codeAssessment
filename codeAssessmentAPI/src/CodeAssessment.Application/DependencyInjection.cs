using CodeAssessment.Application.Commands;
using CodeAssessment.Application.Queries;
using CodeAssessment.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CodeAssessment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
        services.AddScoped<CreateProductCommandHandler>();
        services.AddScoped<UpdateProductCommandHandler>();
        services.AddScoped<DeleteProductCommandHandler>();
        services.AddScoped<AddStockCommandHandler>();
        services.AddScoped<DecrementStockCommandHandler>();
        services.AddScoped<GetProductsQueryHandler>();
        services.AddScoped<GetProductByIdQueryHandler>();
        services.AddScoped<SearchProductsQueryHandler>();
        services.AddScoped<GetProductsByStockRangeQueryHandler>();
        return services;
    }
}
