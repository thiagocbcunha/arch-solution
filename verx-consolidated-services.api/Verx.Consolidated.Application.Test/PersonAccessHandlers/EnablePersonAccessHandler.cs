using Moq;
using AutoFixture;
using Verx.Consolidated.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Domain.Entities.PersonAccessEntity;
using Verx.Consolidated.Application.Command.PersonAccessCommands;

namespace Verx.Consolidated.Application.Test.PersonAccessHandlers;

public class EnablePersonAccessHandler
{
    Fixture _fixture = new();
    Handler.PersonHandlers.EnablePersonAccessHandler _handler;

    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IMessagingSender> _messagingSenderMock = new();
    Mock<ILogger<Handler.PersonHandlers.EnablePersonAccessHandler>> _loggerMock = new();
    Mock<IPersonAccessRepository> _personRepositoryMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new Handler.PersonHandlers.EnablePersonAccessHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object, _messagingSenderMock.Object);
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
        var changeCommand = new EnablePersonAccessCommand(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(), DateTime.Now, true);
        await _handler.Handle(changeCommand, new CancellationToken());

        _activityFactoryMock.Verify(m => m.Start("EnableUser-Handler"), Times.Once);
        _messagingSenderMock.Verify(m => m.Send(It.IsAny<ConsolidatedDto>()), Times.Once);
        _personRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<PersonAccess>()), Times.Once);
    }
}