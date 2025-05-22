using AutoFixture;
using FluentAssertions;
using Verx.Consolidated.Domain.Entities.PersonAccessEntity;
using Verx.Consolidated.Domain.Exceptions;

namespace Verx.Consolidated.Domain.Test
{
    public class PersonAccessEventTests
    {
        private Fixture _fixture = new();

        [TestCase(true, TestName = "ShoudDisablePersonAccessEvent")]
        [TestCase(false, TestName = "ShoudEnablePersonAccessEvent")]
        public void ShoudChangeActivedPersonAccessEvent(bool flagActived)
        {
            var personAccessEvent = new PersonAccessEvent(flagActived, _fixture.Create<string>(), _fixture.Create<DateTime>());

            if(flagActived)
                personAccessEvent.Disable();
            else
                personAccessEvent.Enable();

            flagActived.Should().NotBe(personAccessEvent.Actived);
        }

        [Test]
        public void ShoudSetPasswordPersonAccessEvent()
        {
            var newPassword = _fixture.Create<string>();
            var personAccessEvent = new PersonAccessEvent(true, _fixture.Create<string>(), _fixture.Create<DateTime>());
            personAccessEvent.SetPassword(newPassword);
            personAccessEvent.EncryptedPass.Should().Be(newPassword);
        }
    }
}