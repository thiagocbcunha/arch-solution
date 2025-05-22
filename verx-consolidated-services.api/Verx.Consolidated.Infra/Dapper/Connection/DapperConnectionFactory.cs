using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Verx.Consolidated.Infra.Dapper.Contracts;

namespace Verx.Consolidated.Infra.Dapper.Connection;

public class DapperConnectionFactory(IConfiguration configuration) : IConnectionFactory
{
    public IDbConnection Connection()
    {
        return new SqlConnection(configuration.GetConnectionString("VerxConsolidatedDB"));
    }
}
