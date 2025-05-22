using AutoFixture;
using Verx.Consolidated.Application.Command.PersonAccessCommands;
using Verx.Consolidated.Application.Handler.PersonAccessHandlers;
using Moq;
using FluentAssertions;
using System.Linq.Expressions;
using Verx.Consolidated.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Enterprise.Library.Contracts;

namespace Verx.Consolidated.Application.Test.PersonAccessHandlers;

public class GetPersonAccessByEmailHandlerTest
{
    Fixture _fixture = new();
    GetPersonAccessByEmailHandler _handler;

    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IConsolidatedNSqlRepository> _personRepositoryMock = new();
    Mock<ILogger<GetPersonAccessByEmailHandler>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new GetPersonAccessByEmailHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object);
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
        var email = "test@teste.com.br";
        var personAccessList = _fixture.Build<ConsolidatedDto>().With(i => i.Email, email).CreateMany();
        _personRepositoryMock.Setup(m => m.GetMany(It.IsAny<Expression<Func<ConsolidatedDto, bool>>>())).Returns(personAccessList);

        var result = await _handler.Handle(new GetPersonAccessByEmailCommand(email), new CancellationToken());

        result.Should().NotBeNull();
        _activityFactoryMock.Verify(m => m.Start("GetPersonAccessByEmail-Handler"), Times.Once);
        _personRepositoryMock.Verify(m => m.GetMany(It.IsAny<Expression<Func<ConsolidatedDto, bool>>>()), Times.Once);
    }
}