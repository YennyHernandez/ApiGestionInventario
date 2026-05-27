using Inventario.Domain.Models.Dto;
using FluentValidation;

namespace Inventario.Application.Validators
{
    public class CategoriaUpdateValidator : AbstractValidator<CategoriaUpdateDto>
    {
        public CategoriaUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID es requerido.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");
        }
    }
}
