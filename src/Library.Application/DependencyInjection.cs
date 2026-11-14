
using FluentValidation;
using Library.Application.Interfaces;
using Library.Application.Service;
using Library.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();

        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<
            CreateBookRequestValidator>();

        return services;
    }
}
