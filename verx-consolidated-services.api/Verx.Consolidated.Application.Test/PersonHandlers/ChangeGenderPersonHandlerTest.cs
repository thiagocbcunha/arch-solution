using Moq;
using AutoFixture;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Enums;
using Verx.Consolidated.Domain.Contracts;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Application.Command.PersonCommands;
using Verx.Consolidated.Application.CreateTransaction;
using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Application.Test.PersonHandlers;

public class ChangeGenderPersonHandlerTest
{
    Fixture _fixture = new();
    ChangeGenderPersonHandler _handler;
    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IMessagingSender> _messagingSenderMock = new();
    Mock<ITransactionRepository> _personRepositoryMock = new();
    Mock<ILogger<CreatePersonHandler>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new ChangeGenderPersonHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object, _messagingSenderMock.Object);
        _personRepositoryMock.Setup(i => i.UpdateAsync(It.IsAny<TransactionEntity>()));
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
        var changeCommand = new ChangeGenderPersonCommand(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(), DateTime.Now, GenderEnum.Female);
        await _handler.Handle(changeCommand, new CancellationToken());

        _messagingSenderMock.Verify(m => m.Send(It.IsAny<PersonDto>()), Times.Once);
        _activityFactoryMock.Verify(m => m.Start("ChangeGenderPerson-Handler"), Times.Once);
        _personRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<TransactionEntity>()), Times.Once);
    }
}