
using Library.Application.Interfaces;
using Library.Application.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();

        return services;
    }
}
