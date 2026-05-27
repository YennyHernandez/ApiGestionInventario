using Inventario.Domain.Models.Dto;
using FluentValidation;

namespace Inventario.Application.Validators
{
    public class CategoriaCreateValidator : AbstractValidator<CategoriaCreateDto>
    {
        public CategoriaCreateValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripcion no puede exceder 500 caracteres.");
        }
    }
}
