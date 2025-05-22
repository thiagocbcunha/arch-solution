using AutoFixture;
using FluentAssertions;
using Verx.Consolidated.Domain.Entities;
using Verx.Consolidated.Domain.Entities.PersonAccessEntity;

namespace Verx.Consolidated.Domain.Test
{
    public class PersonTests
    {
        private Fixture _fixture = new();

        [Test]
        public void ShoudChangePasswordSuccessfully()
        {
            var newGender = _fixture.Create<Gender>();
            var person = _fixture.Create<TransactionEntity>();
            
            person.SetGender(newGender);

            person.Gender.Should().Be(newGender);
        }
    }
}