namespace ClassLibrary1
{
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
            var services = new IClient[] { _service1, _service2 };
            var tasks = new Task<IApplicationStatus>[2];
            for (int i = 0; i < 2; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    int triesCount = 0;
                    while (true)
                    {
                        triesCount++;
                        var task = services[i].GetApplicationStatus(id, new CancellationToken() { });
                        task.Wait();
                        var resp = task.Result;
                        if (resp is RetryResponse response) Thread.Sleep((int)response.Delay.TotalMilliseconds);
                        else
                        {
                            if (resp is SuccessResponse successResponse)
                                return (IApplicationStatus)new SuccessStatus(successResponse.Id, successResponse.Status);
                            else return (IApplicationStatus)new FailureStatus(DateTime.Now, triesCount);
                        };
                    }
                });
            }
            int index = Task.WaitAny(tasks, 15000);
            if (index < 0) return Task.FromResult((IApplicationStatus)new FailureStatus(null, 0));
            return tasks[index];
        }
    }

    interface IApplicationStatus
    {
    }

    record SuccessStatus(string ApplicationId, string Status) : IApplicationStatus;
    record FailureStatus(DateTime? LastRequestTime, int RetriesCount) : IApplicationStatus;
}