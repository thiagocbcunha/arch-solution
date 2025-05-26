using FluentAssertions;
using AutoFixture.Xunit2;
using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Domain.Tests.Entities;

public class TransactionEventEntityTests
{

    [Theory, AutoData]
    public void Constructor_ShouldInitializeProperties(
        string currency, string description, string sender, string receiver)
    {
        // Act
        var entity = new TransactionEventEntity(currency, description, sender, receiver);

        // Assert
        entity.Currency.Should().Be(currency);
        entity.Description.Should().Be(description);
        entity.SenderAccountId.Should().Be(sender);
        entity.ReceiverAccountId.Should().Be(receiver);
        entity.VersionNum.Should().Be(0);
    }

    [Theory, AutoData]
    public void ChangeCurrency_ShouldUpdateCurrencyAndVersion(string initial, string updated)
    {
        var entity = new TransactionEventEntity(initial);

        var oldDate = entity.CreateDate;
        entity.ChangeCurrency(updated);

        entity.Currency.Should().Be(updated);
        entity.VersionNum.Should().Be(2); // _versionNum=1, VersionNum=0, ChangeVersionNum: VersionNum=2
        entity.IsChanged.Should().BeTrue();
        entity.CreateDate.Should().BeAfter(oldDate);
    }

    [Theory, AutoData]
    public void ChangeDescription_ShouldUpdateDescriptionAndVersion(string initial, string updated)
    {
        var entity = new TransactionEventEntity(description: initial);

        var oldDate = entity.CreateDate;
        entity.ChangeDescription(updated);

        entity.Description.Should().Be(updated);
        entity.VersionNum.Should().Be(2);
        entity.IsChanged.Should().BeTrue();
        entity.CreateDate.Should().BeAfter(oldDate);
    }

    [Theory, AutoData]
    public void ChangeSenderAccountId_ShouldUpdateSenderAndVersion(string initial, string updated)
    {
        var entity = new TransactionEventEntity(senderAccountId: initial);

        var oldDate = entity.CreateDate;
        entity.ChangeSenderAccountId(updated);

        entity.SenderAccountId.Should().Be(updated);
        entity.VersionNum.Should().Be(2);
        entity.IsChanged.Should().BeTrue();
        entity.CreateDate.Should().BeAfter(oldDate);
    }

    [Theory, AutoData]
    public void ChangeReceiverAccountId_ShouldUpdateReceiverAndVersion(string initial, string updated)
    {
        var entity = new TransactionEventEntity(receiverAccountId: initial);

        var oldDate = entity.CreateDate;
        entity.ChangeReceiverAccountId(updated);

        entity.ReceiverAccountId.Should().Be(updated);
        entity.VersionNum.Should().Be(2);
        entity.IsChanged.Should().BeTrue();
        entity.CreateDate.Should().BeAfter(oldDate);
    }

    [Theory, AutoData]
    public void ChangeCreateBy_ShouldUpdateCreateByAndVersion(string updated)
    {
        var entity = new TransactionEventEntity();

        var oldDate = entity.CreateDate;
        entity.ChangeCreateBy(updated);

        entity.CreateBy.Should().Be(updated);
        entity.VersionNum.Should().Be(2);
        entity.IsChanged.Should().BeTrue();
        entity.CreateDate.Should().BeAfter(oldDate);
    }
}
