using Grpc.Core;

namespace GrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly ITest test;

        public GreeterService(ILogger<GreeterService> logger, ITest test)
        {
            _logger = logger;
            this.test = test;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = test.Console(request.Name)
            });
        }
    }
}