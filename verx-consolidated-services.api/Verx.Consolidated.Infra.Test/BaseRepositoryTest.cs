using AutoFixture;
using Microsoft.Extensions.Configuration;
using Verx.Consolidated.Infra.Dapper.Connection;
using Verx.Consolidated.Infra.Dapper.Repositories;
using Verx.Consolidated.Domain.Entities.PersonAccessEntity;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Infra.IntegrationTest;

public abstract class BaseRepositoryTest
{
    protected Fixture _fixture = new();
    protected ITransactionRepository _personRepository;
    protected IPersonAccessRepository _personAccessRepository;

    public BaseRepositoryTest()
    {
        var connectionString = new Dictionary<string, string>
        {
                {"ConnectionStrings:Verx.ConsolidatedOnboarding", "Server=127.0.0.1,1433; Database=Verx.ConsolidatedOnboarding; User Id=sa;Password=SqlServer2022!;"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(initialData: connectionString)
        .Build();

        var connectionFactory = new DapperConnectionFactory(configuration);
        _personRepository = new TransactionRepository(connectionFactory);
        _personAccessRepository = new PersonAccessRepository(connectionFactory);
    }

    public TransactionEntity GetPerson(Gender gender)
    {
        var person = new TransactionEntity()
        {
            Name = _fixture.Create<string>(),
            BirthDate = _fixture.Create<DateTime>(),
            DocumentNumber = _fixture.Create<string>().Substring(0, 12)
        };

        person.SetGender(gender);

        return person;
    }
    public PersonAccess GetPersonAccess(Gender gender)
    {
        var person = GetPerson(gender);
        return GetPersonAccess(person.Id);
    }

    public PersonAccess GetPersonAccess(Guid personId)
    {
        var Email = _fixture.Create<string>();
        var password = _fixture.Create<string>();
        var personAccess = new PersonAccess(Email, new PersonAccessEvent(true, password, DateTime.Now.AddDays(-50)));
        
        personAccess.Enable();
        personAccess.SetId(personId);

        return personAccess;
    }

    public async Task<PersonAccess> CreateFullPersonAccess()
    {
        var person = await CreateFullPerson();

        var personAccess = GetPersonAccess(person.Id);
        personAccess.ChangePassword(_fixture.Create<string>());
        await _personAccessRepository.AddAsync(personAccess);

        return personAccess;
    }

    public async Task<TransactionEntity> CreateFullPerson(Gender gender)
    {
        var person = GetPerson(gender);
        await _personRepository.AddAsync(person);

        return person;
    }

    public async Task<TransactionEntity> CreateFullPerson()
    {
        return await CreateFullPerson(new Gender(1, "Feminino"));
    }
}