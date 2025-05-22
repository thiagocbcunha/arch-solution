using System.Data;

namespace Verx.Consolidated.Infra.Dapper.Contracts;

public interface IConnectionFactory
{
    IDbConnection Connection();
}