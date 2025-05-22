using Moq;
using AutoFixture;
using FluentAssertions;
using System.Linq.Expressions;
using Verx.Consolidated.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Application.Handler.PersonHandlers;
using Verx.Consolidated.Application.Command.PersonCommands;

namespace Verx.Consolidated.Application.Test.PersonHandlers;

public class GetPersonByIdHandlerTest
{
    Fixture _fixture = new();
    GetPersonByIdHandler _handler;

    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IPersonNSqlRepository> _personRepositoryMock = new();
    Mock<ILogger<GetPersonByIdHandler>> _loggerMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new GetPersonByIdHandler(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object);
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
        var personAccessList = _fixture.Build<PersonDto>().With(i => i.Id, id).CreateMany();
        _personRepositoryMock.Setup(m => m.GetMany(It.IsAny<Expression<Func<PersonDto, bool>>>())).Returns(personAccessList);

        var result = await _handler.Handle(new GetPersonByIdCommand(id), new CancellationToken());

        result.Should().NotBeNull();
        _activityFactoryMock.Verify(m => m.Start("GetPersonById-Handler"), Times.Once);
        _personRepositoryMock.Verify(m => m.GetMany(It.IsAny<Expression<Func<PersonDto, bool>>>()), Times.Once);
    }
}