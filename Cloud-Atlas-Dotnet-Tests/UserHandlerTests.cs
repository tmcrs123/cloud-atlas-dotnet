using Cloud_Atlas_Dotnet.Application.Auth;
using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Application.Handlers;
using Cloud_Atlas_Dotnet.Domain.Patterns;
using Cloud_Atlas_Dotnet.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Cloud_Atlas_Dotnet_Tests
{
    public class UserHandlerTests
    {
        [Fact]
        public async void CreateUserHandler_Ok()
        {
            //Arrange
            var passwordServiceMock = new Mock<IPasswordService>();

            passwordServiceMock.Setup(m => m.HashPassword(It.IsAny<string>())).Returns(() => "bananas");

            var repositoryMock = new Mock<IRepository>();

            repositoryMock.Setup(r => r.UsernameExists(It.IsAny<string>())).ReturnsAsync(false);

            repositoryMock.Setup(r => r.CreateUser(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(new Guid());

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(s => s.GetService(typeof (IRepository)))
                .Returns(() => repositoryMock.Object);

            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.Setup(s => s.ServiceProvider)
                .Returns(serviceProviderMock.Object);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

            serviceScopeFactoryMock.Setup(s => s.CreateScope())
                .Returns(() => serviceScopeMock.Object);

            var handler = new CreateUserHandler(serviceScopeFactoryMock.Object, passwordServiceMock.Object);

            //Act
            var result = await handler.Handle(new CreateUserCommand()
            {
                Email = "some email",
                Name = "some name",
                Password = "some password",
                Username = "some username"
            }, new CancellationToken());

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async void CreateUserHandler_Failure_UserExists()
        {
            //Arrange
            var passwordServiceMock = new Mock<IPasswordService>();

            passwordServiceMock.Setup(m => m.HashPassword(It.IsAny<string>())).Returns(() => "bananas");

            var repositoryMock = new Mock<IRepository>();

            repositoryMock.Setup(r => r.UsernameExists(It.IsAny<string>())).ReturnsAsync(true);

            repositoryMock.Setup(r => r.CreateUser(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(new Guid());

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(s => s.GetService(typeof(IRepository)))
                .Returns(() => repositoryMock.Object);

            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.Setup(s => s.ServiceProvider)
                .Returns(serviceProviderMock.Object);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

            serviceScopeFactoryMock.Setup(s => s.CreateScope())
                .Returns(() => serviceScopeMock.Object);

            var handler = new CreateUserHandler(serviceScopeFactoryMock.Object, passwordServiceMock.Object);

            //Act
            var result = await handler.Handle(new CreateUserCommand()
            {
                Email = "some email",
                Name = "some name",
                Password = "some password",
                Username = "some username"
            }, new CancellationToken());

            //Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ApplicationError>(result.Error);
            Assert.Equal(ErrorType.Conflict, result.Error.ErrorType);
        }
    }
}
