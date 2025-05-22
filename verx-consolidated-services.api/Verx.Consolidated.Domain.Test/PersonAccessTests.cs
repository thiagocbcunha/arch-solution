using AutoFixture;
using FluentAssertions;
using Verx.Consolidated.Domain.Entities.PersonAccessEntity;
using Verx.Consolidated.Domain.Exceptions;

namespace Verx.Consolidated.Domain.Test
{
    public class PersonAccessTests
    {
        private Fixture _fixture = new();

        [TestCase(true, TestName = "ShoudDisablePersonAccess")]
        [TestCase(false, TestName = "ShoudEnablePersonAccess")]
        public void ShoudChangeActivedPersonAccess(bool flagActived)
        {
            var personAccessEvent = new PersonAccessEvent(flagActived, _fixture.Create<string>(), _fixture.Create<DateTime>());
            var personAccess = new PersonAccess(_fixture.Create<string>(), personAccessEvent);

            if(flagActived)
                personAccess.Disable();
            else
                personAccess.Enable();

            flagActived.Should().NotBe(personAccessEvent.Actived);
        }

        [Test]
        public void ShoudChangePasswordSuccessfully()
        {
            var newPassword = _fixture.Create<string>();
            var personAccessEvent = new PersonAccessEvent(true, _fixture.Create<string>(), DateTime.Now.AddDays(-50));
            var personAccess = new PersonAccess(_fixture.Create<string>(), personAccessEvent);

            personAccess.ChangePassword(newPassword);

            personAccess.PersonAccessEvent.EncryptedPass.Should().Be(newPassword);
        }

        [Test]
        public void ShoudNotChangePasswordByDateNotDue()
        {
            var notDueDate = DateTime.Now;
            var personAccessEvent = new PersonAccessEvent(true, _fixture.Create<string>(), notDueDate);
            var personAccess = new PersonAccess(_fixture.Create<string>(), personAccessEvent);

            Action changePass = () => personAccess.ChangePassword(_fixture.Create<string>());

            changePass.Should().ThrowExactly<BusinessException>().WithMessage("User alredy chanded password before 30 days.");
        }

        [Test]
        public void ShoudNotChangePasswordByNotActivated()
        {
            var notActivated = false;
            var personAccessEvent = new PersonAccessEvent(notActivated, _fixture.Create<string>(), DateTime.Now.AddDays(-50));
            var personAccess = new PersonAccess(_fixture.Create<string>(), personAccessEvent);

            Action changePass = () => personAccess.ChangePassword(_fixture.Create<string>());

            changePass.Should().ThrowExactly<BusinessException>().WithMessage("User not actived.");
        }

        [Test]
        public void ShoudNotChangePasswordByTheSamePassword()
        {
            var samePassword = _fixture.Create<string>();
            var personAccessEvent = new PersonAccessEvent(true, samePassword, DateTime.Now.AddDays(-50));
            var personAccess = new PersonAccess(_fixture.Create<string>(), personAccessEvent);

            Action changePass = () => personAccess.ChangePassword(samePassword);

            changePass.Should().ThrowExactly<BusinessException>().WithMessage("New password is the same old password.");
        }
    }
}