using Microsoft.Extensions.DependencyInjection;

public static class SdClientExtensions
{
    public static IServiceCollection AddServiceDiscoveryClient(this IServiceCollection services)
        => services
            .AddSingleton<IDbStore, InMemoryDbStore>()
            .AddSingleton<IEndpointResolver, EndpointResolver>()
            .AddHostedService<SdConsumerHostedService>();
}