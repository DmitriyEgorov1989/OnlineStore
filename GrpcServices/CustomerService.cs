using Grpc.Core;
using OnlineStore.CustomerService.Exceptions;

namespace OnlineStore.CustomerService.GrpcServices
{
    public sealed class CustomerService : Customers.CustomersBase
    {
        public override Task<CreateCustomerResponse> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
        {
            throw new NotFoundException($"Clients with id = {request.Customer.Id} not found");
        }

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

        public override Task<GetCustomerByLastNameResponse> GetCustomerByLastName(GetCustomerByLastNameRequest request, ServerCallContext context)
        {
            throw new NotFoundException($"Clients with id = {request.LastName} not found"); ;
        }

        public override Task<GetCustomersResponse> GetCustomers(GetCustomersRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Clients not found"));
        }

        public override Task<GetCustomersForGeneratorResponse> GetCustomersForGenerator(GetCustomersForGeneratorRequest request, ServerCallContext context)
        {
            throw new NotFoundException($"Clients with id = {request.Id} not found");
        }
    }
}
