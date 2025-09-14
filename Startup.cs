using OnlineStore.CustomerService.ClientBalancing;
using OnlineStore.CustomerService.Infrastructure;
using OnlineStore.ServiceDiscovery;

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
            serviceCollection.AddGrpcClient<SdService.SdServiceClient>(option=>
            {
                var url = _configuration.GetValue<string>("ONLINESTORE_SD_ADDRESS");
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException("ONLINESTORE_SD_ADDRESS variable is null or Emty");
                }
                option.Address=new Uri(url);
            });
            serviceCollection.AddGrpcReflection();
            serviceCollection.AddControllers();
            serviceCollection.AddEndpointsApiExplorer();

            serviceCollection.AddHostedService<SdConsumerHostedService>();
            
            serviceCollection.AddSingleton<IDbStore,DbStore>(); 
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