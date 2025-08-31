using FluentValidation;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;

namespace TagTheSpot.Services.Spot.Application.Validators
{
    public sealed class GetRandomSpotsByCityIdRequestValidator
        : AbstractValidator<GetRandomSpotsByCityIdRequest>
    {
        public GetRandomSpotsByCityIdRequestValidator()
        {
            RuleFor(x => x.CityId)
                .NotEmpty();

            RuleFor(x => x.Count)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThan(0)
                .LessThan(100);
        }
    }
}
