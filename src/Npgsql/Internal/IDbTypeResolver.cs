using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql.Internal;

/// <summary>
/// An Npgsql resolver for DbType. Used by Npgsql to map DbType to DataTypeName and back.
/// </summary>
[Experimental(NpgsqlDiagnostics.DbTypeResolverExperimental)]
public interface IDbTypeResolver
{
    /// <summary>
    /// Attempts to map a DbType to a data type name.
    /// </summary>
    /// <param name="dbType">The DbType name to map.</param>
    /// <returns>The data type name if it could be mapped, the name can be non-normalized and without schema.</returns>
    string? GetDataTypeName(DbType dbType);

    /// <summary>
    /// Attempts to map a data type name to a DbType.
    /// </summary>
    /// <param name="dataTypeName">The data type name to map, in a normalized form but possibly without schema.</param>
    /// <returns>The DbType if it could be mapped, null otherwise.</returns>
    DbType? GetDbType(string dataTypeName);
}
