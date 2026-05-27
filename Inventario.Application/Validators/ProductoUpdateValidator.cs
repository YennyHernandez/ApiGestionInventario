using Inventario.Domain.Models.Dto;
using FluentValidation;

namespace Inventario.Application.Validators
{
    public class ProductoUpdateValidator : AbstractValidator<ProductoUpdateDto>
    {
        public ProductoUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID es requerido.");

            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("El SKU es requerido.")
                .MaximumLength(50).WithMessage("El SKU no puede exceder 50 caracteres.");

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
        }
    }
}
