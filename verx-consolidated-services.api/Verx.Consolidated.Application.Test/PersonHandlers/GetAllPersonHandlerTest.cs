using Moq;
using AutoFixture;
using FluentAssertions;
using Verx.Consolidated.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Application.Handler.PersonHandlers;
using Verx.Consolidated.Application.Command.PersonCommands;

namespace Verx.Consolidated.Application.Test.PersonHandlers;

public class GetAllPersonHandlerTest
{
    Fixture _fixture = new();
    GetAllPersonHandler _handler;

    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IPersonNSqlRepository> _personRepositoryMock = new();
    Mock<ILogger<GetAllPersonHandler>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new GetAllPersonHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object);
    }

    [TearDown]
    public void Down()
    {
        _loggerMock = new();
        _activityFactoryMock = new();
        _personRepositoryMock = new();
    }

    [Test]
    public async Task ShoudExecuteHandlerSuccessfully()
    {
        var persons = _fixture.CreateMany<PersonDto>();
        _personRepositoryMock.Setup(m => m.GetAll()).Returns(persons);

        var result = await _handler.Handle(new GetAllPersonCommand(), new CancellationToken());

        result.Should().HaveCount(persons.Count());
        _personRepositoryMock.Verify(m => m.GetAll(), Times.Once);
        _activityFactoryMock.Verify(m => m.Start("GetAllPersons-Handler"), Times.Once);
    }
}