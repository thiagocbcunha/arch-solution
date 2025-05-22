using Moq;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Application.Handler.PersonHandlers;
using Verx.Consolidated.Domain.Entities.PersonAccessEntity;
using Verx.Consolidated.Application.Command.PersonAccessCommands;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Contracts;

namespace Verx.Consolidated.Application.Test.PersonAccessHandlers;

public class DisableUserHandlerTest
{
    Fixture _fixture = new();
    DisablePersonAccessHandler _handler;

    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IMessagingSender> _messagingSenderMock = new();
    Mock<ILogger<DisablePersonAccessHandler>> _loggerMock = new();
    Mock<IPersonAccessRepository> _personRepositoryMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new DisablePersonAccessHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object, _messagingSenderMock.Object);
        _personRepositoryMock.Setup(i => i.UpdateAsync(It.IsAny<PersonAccess>()));
    }

    [TearDown]
    public void Down()
    {
        _loggerMock = new();
        _activityFactoryMock = new();
        _messagingSenderMock = new();
        _personRepositoryMock = new();
    }

    [Test]
    public async Task ShoudExecuteHandlerSuccessfully()
    {
        var changeCommand = new DisablePersonAccessCommand(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(), DateTime.Now, true);
        await _handler.Handle(changeCommand, new CancellationToken());

        _activityFactoryMock.Verify(m => m.Start("DisableUser-Handler"), Times.Once);
        _messagingSenderMock.Verify(m => m.Send(It.IsAny<ConsolidatedDto>()), Times.Once);
        _personRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<PersonAccess>()), Times.Once);
    }
}