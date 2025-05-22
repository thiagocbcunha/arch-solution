using AutoFixture;
using FluentAssertions;
using Verx.Consolidated.Domain.Entities.PersonEntity;

namespace Verx.Consolidated.Infra.IntegrationTest
{
    public class PersonAccessRepositoryTest : BaseRepositoryTest
    {
        [Test]
        public async Task ShouldBeAddNewPersonAccessSuccessfully()
        {
            var person = await CreateFullPerson(new Gender(1, "Feminino"));
            var personAccess = GetPersonAccess(person.Id);
            await _personAccessRepository.AddAsync(personAccess);
        }

        [Test]
        public async Task ShouldBeUpdateNewPersonSuccessfully()
        {
            var newPass = _fixture.Create<string>();
            var personAccess = await CreateFullPersonAccess();
            var oldPass = personAccess.PersonAccessEvent.EncryptedPass;
            
            personAccess.ChangePassword(newPass);
            
            await _personAccessRepository.UpdateAsync(personAccess);

            personAccess.PersonAccessEvent.EncryptedPass.Should().NotBe(oldPass);
        }

        [Test]
        public async Task ShouldBeGetPersonSuccessfully()
        {
            var personAccess = await CreateFullPersonAccess();
            var personAccessDb = await _personAccessRepository.GetByIdAsync(personAccess.PersonId);

            personAccess.Id.Should().Be(personAccessDb.Id);
            personAccess.Email.Should().Be(personAccessDb.Email);
        }
    }
}