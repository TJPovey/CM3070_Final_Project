using FluentValidation;
using SnagIt.API.Core.Application.Models.Shared;


namespace SnagIt.API.Core.Application.Features.Shared.Validators
{
    public class LocationPostValidator : AbstractValidator<LocationPostDto>
    {
        public LocationPostValidator()
        {
            RuleFor(data => data.Latitude)
                .GreaterThanOrEqualTo(90m * -1)
                .LessThanOrEqualTo(90m);

            RuleFor(data => data.Longitude)
                .GreaterThanOrEqualTo(180m * -1)
                .LessThanOrEqualTo(180m);

            RuleFor(data => data.Elevation)
                .NotNull();

        }
    }
}
