using FluentValidation;
using UrlShortener.Models;
namespace UrlShortener.Validator;
public class ShortenUrlRequestValidator : AbstractValidator<UrlRequest>
{
    public ShortenUrlRequestValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty().WithMessage("URL is required.")
            .Must(BeAValidUrl).WithMessage("Invalid URL format.");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
