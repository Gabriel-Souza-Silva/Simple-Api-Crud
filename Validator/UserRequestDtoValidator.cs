using FluentValidation;
using SimpleCrud.Models.Request;

namespace SimpleCrud.Validator
{
    public class UserRequestDtoValidator : AbstractValidator<UserRequestDto>
    {
        public UserRequestDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("A senha é obrigatória.");
            RuleFor(x => x.Age).InclusiveBetween(18, 99).WithMessage("A idade deve estar entre 18 e 99 anos.");
            RuleFor(x => x.Gender).NotNull().WithMessage("O gênero é obrigatório.");
            RuleFor(x => x.DocumentNumber).NotEmpty().WithMessage("O número do documento é obrigatório.");
            RuleFor(x => x.Adress).NotEmpty().WithMessage("O endereço é obrigatório.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("O endereço de e-mail é inválido.");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("O número de telefone é obrigatório.");
        }
    }
}
