using OnlineStore.CustomerService.Infrastructure;

namespace OnlineStore.CustomerService
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddGrpc(option=>option.Interceptors.Add<LoggerInterceptor>());
            serviceCollection.AddGrpcReflection();
            serviceCollection.AddControllers();
            serviceCollection.AddEndpointsApiExplorer();
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();
            applicationBuilder.UseHttpsRedirection();

            applicationBuilder.UseEndpoints(endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<GrpcServices.CustomerService>();
                endpointRouteBuilder.MapGrpcReflectionService();
                endpointRouteBuilder.MapGet("", () => "Hello World");
            });
        }
    }
}