using Moq;
using AutoFixture;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Enums;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Application.Command.PersonCommands;
using Verx.Consolidated.Application.CreateTransaction;
using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Application.Test.PersonHandlers;

public class CreatePersonHandlerTest
{
    Fixture _fixture = new();
    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IMessagingSender> _messagingSenderMock = new();
    Mock<ITransactionRepository> _personRepositoryMock = new();
    Mock<ILogger<CreatePersonHandler>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
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
        var insertCommand = new CreatePersonCommand(_fixture.Create<string>(), _fixture.Create<string>(), DateTime.Now, GenderEnum.Female);
        _personRepositoryMock.Setup(i => i.AddAsync(It.IsAny<TransactionEntity>()));

        var handler = new CreatePersonHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object, _messagingSenderMock.Object);
        await handler.Handle(insertCommand, new CancellationToken());

        _messagingSenderMock.Verify(m => m.Send(It.IsAny<PersonDto>()), Times.Once);
        _activityFactoryMock.Verify(m => m.Start("InsertPerson-Handler"), Times.Once);
        _personRepositoryMock.Verify(m => m.AddAsync(It.IsAny<TransactionEntity>()), Times.Once);
    }
}