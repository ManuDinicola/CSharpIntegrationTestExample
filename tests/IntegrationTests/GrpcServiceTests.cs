using FluentAssertions;
using Grpc.Net.Client;
using GrpcService;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class GrpcServiceTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public GrpcServiceTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task OverrideStartupFilterTest()
        {
            await using TestApplication? application = new TestApplication(testOutputHelper);

            GrpcChannel? channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            {
                HttpClient = application.CreateClient()
            });

            Greeter.GreeterClient? grpcClient = new Greeter.GreeterClient(channel);

            HelloReply? reply = await grpcClient.SayHelloAsync(new HelloRequest() { Name = "Manu" });

            reply.Message.Should().Be("Test");
        }
    }
}