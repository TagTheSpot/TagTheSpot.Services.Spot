using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace TagTheSpot.Services.Spot.Application.Validators
{
    public sealed class ImageFileValidator : AbstractValidator<IFormFile>
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MinFileSizeBytes = 1 * 1024;     // 1 KB
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ImageFileValidator()
        {
            RuleFor(file => file.Length)
                .GreaterThan(MinFileSizeBytes)
                .WithMessage("Image file is too small (must be > 1KB).");

            RuleFor(file => file.Length)
                .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage("Image file is too large (must be <= 5MB).");

            RuleFor(file => file.FileName)
                .Must(filename => AllowedExtensions.Contains(Path.GetExtension(filename).ToLower()))
                .WithMessage("Unsupported image format. Allowed: .jpg, .jpeg, .png, .webp");
        }
    }
}
