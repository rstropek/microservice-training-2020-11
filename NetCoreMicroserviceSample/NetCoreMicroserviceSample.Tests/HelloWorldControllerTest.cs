using Xunit;
using NetCoreMicroserviceSample.Api.Controllers;
using Moq;

namespace NetCoreMicroserviceSample.Tests
{
    public class HelloWorldControllerTest
    {
        [Fact]
        public void GetHelloWorld()
        {
            var blMock = new Mock<IBusinessLogic>();
            blMock.Setup(b => b.GetNiceGreeting()).Returns("FooBar").Verifiable();

            var controller = new HelloWorldController(blMock.Object);
            var greeting = controller.HelloWorld(1);

            Assert.Equal("FooBar", greeting);
            blMock.Verify(b => b.GetNiceGreeting(), Times.Once());
            blMock.Verify(b => b.GetNotSoNiceGreeting(), Times.Never());
        }
    }
}
