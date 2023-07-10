using Dapper.Contrib.Extensions;

namespace Allocate.Common.Database.DataModels;

/// <summary>
/// Data Model that is persisted to the Database.
/// It is an abstract class instead of an interface because 
/// the Dapper.SqlMapperExtensions generic methods used in DatabaseService
/// require a concrete class type.
/// When creating a table in a FluentMigrator script,
/// extend BaseMigrator to get the CreateAllocateWebTable method,
/// which will automatically add columns for all the below properties.
/// </summary>
public abstract class DataModelBase
{
    [ExplicitKey]
    public Guid Id { get; set; }
}
