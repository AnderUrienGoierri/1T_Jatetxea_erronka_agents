using NHibernate.Driver;
using NHibernate.Engine;

namespace Jatetxea.Api.Data;

/// <summary>
/// MySqlConnector (2.x) liburutegirako NHibernate driver pertsonalizatua.
/// Proiektuan dagoeneko instalatuta dagoen MySqlConnector asinkronoa erabiltzen du,
/// MySql.Data gehigarririk behar izan gabe eta MySQL 8.0 dialektoarekin bateragarri.
/// </summary>
public class MySqlConnectorDriver : ReflectionBasedDriver
{
    public MySqlConnectorDriver()
        : base(
            "MySqlConnector",
            "MySqlConnector",
            "MySqlConnector.MySqlConnection",
            "MySqlConnector.MySqlCommand")
    {
    }

    public override bool UseNamedPrefixInSql => true;

    public override bool UseNamedPrefixInParameter => true;

    public override string NamedPrefix => "?";

    public override bool SupportsMultipleOpenReaders => false;

    public override bool SupportsMultipleQueries => true;

    public override bool RequiresTimeSpanForTime => true;

    public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session)
    {
        return new BasicResultSetsCommand(session);
    }
}
