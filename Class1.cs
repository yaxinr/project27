namespace ClassLibrary1
{
    public class Class1
    {

    }

    interface IClient
    {
        Task<IResponse> GetApplicationStatus(string id, CancellationToken cancellationToken);
    }

    interface IResponse
    {
    }

    record SuccessResponse(string Id, string Status) : IResponse;
    record FailureResponse() : IResponse;
    record RetryResponse(TimeSpan Delay) : IResponse;

    interface IHandler
    {
        Task<IApplicationStatusResponse> GetApplicationStatus(string id);
    }

    public interface IApplicationStatusResponse
    {
    }

    class Handler : IHandler
    {
        private readonly IClient _client1;
        private readonly IClient _client2;
        private IClient _service1;
        private IClient _service2;

        //private readonly ILogger<Handler> _logger;

        public Handler(
          IClient service1,
          IClient service2
            //,          ILogger<Handler> logger
            )
        {
            _service1 = service1;
            _service2 = service2;
            //_loggger = logger;
        }

        public Task<IApplicationStatus> GetApplicationStatus(string id)
        {
            //TODO: place code here

            return Task.CompletedTask;
        }

        Task<IApplicationStatusResponse> IHandler.GetApplicationStatus(string id)
        {
            throw new NotImplementedException();
        }
    }

    interface IApplicationStatus
    {
    }

    record SuccessStatus(string ApplicationId, string Status) : IApplicationStatus;
    record FailureStatus(DateTime? LastRequestTime, int RetriesCount) : IApplicationStatus;
}