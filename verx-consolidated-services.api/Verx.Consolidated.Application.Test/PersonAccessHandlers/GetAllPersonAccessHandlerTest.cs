using Moq;
using AutoFixture;
using FluentAssertions;
using Verx.Consolidated.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Application.Command.PersonAccessCommands;
using Verx.Consolidated.Application.Handler.PersonAccessHandlers;

namespace Verx.Consolidated.Application.Test.PersonAccessHandlers;

public class GetAllPersonAccessHandlerTest
{
    Fixture _fixture = new();
    GetAllPersonAccessHandler _handler;

    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IConsolidatedNSqlRepository> _personRepositoryMock = new();
    Mock<ILogger<GetAllPersonAccessHandler>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new GetAllPersonAccessHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object);
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
        var personAccessList = _fixture.CreateMany<ConsolidatedDto>();
        _personRepositoryMock.Setup(m => m.GetAll()).Returns(personAccessList);

        var result = await _handler.Handle(new GetAllPersonAccessCommand(), new CancellationToken());

        result.Should().HaveCount(personAccessList.Count());
        _personRepositoryMock.Verify(m => m.GetAll(), Times.Once);
        _activityFactoryMock.Verify(m => m.Start("GetAllPersonAccess-Handler"), Times.Once);
    }
}