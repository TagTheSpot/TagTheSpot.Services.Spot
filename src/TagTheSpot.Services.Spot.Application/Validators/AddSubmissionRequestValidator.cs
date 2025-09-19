using FluentValidation;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Application.Extensions;
using TagTheSpot.Services.Spot.Domain.Spots;

namespace TagTheSpot.Services.Spot.Application.Validators
{
    public sealed class AddSubmissionRequestValidator
        : AbstractValidator<AddSubmissionRequest>
    {
        public AddSubmissionRequestValidator()
        {
            RuleFor(x => x.CityId)
                .NotEmpty();

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.SpotType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustBeEnumValue<AddSubmissionRequest, SpotType>();

            RuleFor(x => x.SkillLevel)!
                .MustBeEnumValue<AddSubmissionRequest, SkillLevel>()
                .When(x => !string.IsNullOrWhiteSpace(x.SkillLevel));

            RuleFor(x => x.Accessibility)!
                .MustBeEnumValue<AddSubmissionRequest, Accessibility>()
                .When(x => !string.IsNullOrWhiteSpace(x.Accessibility));

            RuleFor(x => x.Condition)!
                .MustBeEnumValue<AddSubmissionRequest, Condition>()
                .When(x => !string.IsNullOrWhiteSpace(x.Condition));

            RuleFor(x => x.Description)
                .MinimumLength(10)
                .MaximumLength(1000).WithMessage("Description must be between 10 and 1000 characters.")
                .NotEmpty();

            RuleFor(x => x.Images)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(images => images.Count >= 1)
                .WithMessage("At least one image is required.")
                .Must(images => images.Count <= 20)
                .WithMessage("You can upload up to 20 images only.");

            RuleForEach(x => x.Images)
                .SetValidator(new ImageFileValidator());
        }
    }
}
