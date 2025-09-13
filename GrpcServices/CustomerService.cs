using Grpc.Core;

namespace OnlineStore.CustomerService.GrpcServices
{
    public sealed class CustomerService : Customers.CustomersBase
    {
        public override Task<GetCustomerByIdResponse> GetCustomerById(GetCustomerByIdRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetCustomerByIdResponse
            {
                Customer = new Customer
                {
                    Id = 1,
                }
            });
        }
    }
}
