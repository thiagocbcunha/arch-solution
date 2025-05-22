using Moq;
using AutoFixture;
using FluentAssertions;
using Verx.Consolidated.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Domain.Exceptions;
using Verx.Consolidated.Enterprise.Library.Contracts;
using Verx.Consolidated.Application.Handler.PersonHandlers;
using Verx.Consolidated.Domain.Entities.PersonAccessEntity;
using Verx.Consolidated.Application.Command.PersonAccessCommands;

namespace Verx.Consolidated.Application.Test.PersonAccessHandlers;

public class ChangePasswordHanlderTest
{
    Fixture _fixture = new();
    ChangePasswordHanlder _handler;

    Mock<IActivityTracing> _activityFactoryMock = new();
    Mock<IMessagingSender> _messagingSenderMock = new();
    Mock<ILogger<ChangePasswordHanlder>> _loggerMock = new();
    Mock<IEnterpriseSecurity> _enterpriseSecurityMock = new();
    Mock<IPersonAccessRepository> _personRepositoryMock = new();

    [SetUp]
    public void Setup()
    {
        _handler = new ChangePasswordHanlder(_loggerMock.Object, _activityFactoryMock.Object, _personRepositoryMock.Object, _enterpriseSecurityMock.Object, _messagingSenderMock.Object);
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
        var newPass = "123456789123";
        var oldPass = "321456987456";
        _enterpriseSecurityMock.Setup(m => m.GetHash(newPass)).Returns(newPass);
        _enterpriseSecurityMock.Setup(m => m.GetHash(oldPass)).Returns(oldPass);

        var changeCommand = new ChangePasswordCommand(Guid.NewGuid(), "teste@teste.com.br", newPass, oldPass, DateTime.Now.AddDays(-50), true);
        await _handler.Handle(changeCommand, new CancellationToken());

        _activityFactoryMock.Verify(m => m.Start("ChangePassword-Handler"), Times.Once);
        _messagingSenderMock.Verify(m => m.Send(It.IsAny<ConsolidatedDto>()), Times.Once);
        _enterpriseSecurityMock.Verify(m => m.GetHash(It.IsAny<string>()), Times.Exactly(2));
        _personRepositoryMock.Verify(m => m.UpdateAsync(It.IsAny<PersonAccess>()), Times.Once);
    }

    [Test]
    public void ShoudNotChangePasswordByDateNotDue()
    {
        var notDueDate = DateTime.Now;
        var changeCommand = new ChangePasswordCommand(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), notDueDate, true);

        Func<Task> changePass = async () => await _handler.Handle(changeCommand, new CancellationToken());

        changePass.Should().ThrowExactlyAsync<BusinessException>().WithMessage("User alredy chanded password before 30 days.");
    }

    [Test]
    public void ShoudNotChangePasswordByNotActivated()
    {
        var notActivated = false;
        var changeCommand = new ChangePasswordCommand(Guid.NewGuid(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), DateTime.Now.AddDays(-50), notActivated);

        Func<Task> changePass = async () => await _handler.Handle(changeCommand, new CancellationToken());

        changePass.Should().ThrowExactlyAsync<BusinessException>().WithMessage("User not actived.");
    }

    [Test]
    public void ShoudNotChangePasswordByTheSamePassword()
    {
        var samePassword = _fixture.Create<string>();
        var changeCommand = new ChangePasswordCommand(Guid.NewGuid(), _fixture.Create<string>(), samePassword, samePassword, DateTime.Now.AddDays(-50), true);

        Func<Task> changePass = async () => await _handler.Handle(changeCommand, new CancellationToken());

        changePass.Should().ThrowExactlyAsync<BusinessException>().WithMessage("New password is the same old password.");
    }
}