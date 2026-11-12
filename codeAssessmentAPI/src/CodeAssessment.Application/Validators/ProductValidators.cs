using CodeAssessment.Application.DTOs;
using FluentValidation;

namespace CodeAssessment.Application.Validators;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0);
    }
}
