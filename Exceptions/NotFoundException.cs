namespace OnlineStore.CustomerService.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public override string Message { get; }

        public NotFoundException(string exceptionMessage)
        {
            Message = exceptionMessage;
        }
    }
}
