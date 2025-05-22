using System.Text.Json.Serialization;
using Verx.TransactionFlow.Application.CreateTransation;
using Verx.TransactionFlow.Application.ProcessEvent;

namespace Verx.Authentication.Service.Application;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(CreateTransactionCommand))]
public partial class ApplicationJsonSerialiazerContext : JsonSerializerContext
{
}