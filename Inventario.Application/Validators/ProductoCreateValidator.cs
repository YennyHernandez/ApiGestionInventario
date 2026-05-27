using Inventario.Domain.Models.Dto;
using FluentValidation;

namespace Inventario.Application.Validators
{
    public class ProductoCreateValidator : AbstractValidator<ProductoCreateDto>
    {
        public ProductoCreateValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("El SKU es requerido.")
                .MaximumLength(50).WithMessage("El SKU no puede exceder 50 caracteres.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

            RuleFor(x => x.Precio)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

            RuleFor(x => x.StockMinimo)
                .GreaterThanOrEqualTo(0).WithMessage("El stock minimo no puede ser negativo.");

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("La categoria es requerida.");
        }
    }
}
