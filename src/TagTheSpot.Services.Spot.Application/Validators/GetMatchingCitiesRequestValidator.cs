using FluentValidation;
using TagTheSpot.Services.Spot.Application.DTO;

namespace TagTheSpot.Services.Spot.Application.Validators
{
    public sealed class GetMatchingCitiesRequestValidator
        : AbstractValidator<GetMatchingCitiesRequest>
    {
        public GetMatchingCitiesRequestValidator()
        {
            RuleFor(x => x.CityPattern)
                .MaximumLength(50).WithMessage("City length cannot be greater than 50 characters.");
        }
    }
}
