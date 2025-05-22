using Moq;
using AutoFixture;
using FluentAssertions;
using System.Linq.Expressions;
using Verx.Consolidated.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Application.Command.PersonAccessCommands;
using Verx.Consolidated.Application.Handler.PersonAccessHandlers;

namespace Verx.Consolidated.Application.Test.PersonAccessHandlers;

public class GetPersonAccessByIdHandlerTest
{
    Fixture _fixture = new();
    GetPersonAccessByIdHandler _handler;

    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IConsolidatedNSqlRepository> _personRepositoryMock = new();
    Mock<ILogger<GetPersonAccessByIdHandler>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new GetPersonAccessByIdHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object);
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
        var id = Guid.NewGuid();
        var personAccessList = _fixture.Build<ConsolidatedDto>().With(i => i.Id, id).CreateMany();
        _personRepositoryMock.Setup(m => m.GetMany(It.IsAny<Expression<Func<ConsolidatedDto, bool>>>())).Returns(personAccessList);

        var result = await _handler.Handle(new GetPersonAccessByIdCommand(id), new CancellationToken());

        result.Should().NotBeNull();
        _activityFactoryMock.Verify(m => m.Start("GetPersonAccessById-Handler"), Times.Once);
        _personRepositoryMock.Verify(m => m.GetMany(It.IsAny<Expression<Func<ConsolidatedDto, bool>>>()), Times.Once);
    }
}