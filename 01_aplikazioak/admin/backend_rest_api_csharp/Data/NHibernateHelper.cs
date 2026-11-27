using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Jatetxea.Api.Mappings;
using NHibernate;
using NHibernate.Dialect;

namespace Jatetxea.Api.Data;

/// <summary>
/// NHibernate-ko ISessionFactory singleton-a kudeatzen duen laguntza-klase zentrala.
/// Konexio-katea DB_CONNECTION_STRING ingurune-aldagaitik (Docker) edo appsettings.json-etik hartzen du,
/// eta FluentNHibernate bidez Mappings/ karpetako ClassMap guztiak kargatzen ditu.
/// </summary>
public static class NHibernateHelper
{
    private static readonly object _lock = new();
    private static ISessionFactory? _sessionFactory;
    private static string _connectionString = string.Empty;

    /// <summary>
    /// Konfiguraziotik konexio-katea ebatzi eta gordetzen du.
    /// </summary>
    public static string LortuKonexioKatea(IConfiguration config)
    {
        string? envConn = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        return !string.IsNullOrWhiteSpace(envConn)
            ? envConn
            : (config.GetConnectionString("Jatetxea")
                ?? config.GetConnectionString("JatetxeaRoot")
                ?? throw new InvalidOperationException("Ez da aurkitu MySQL konexio-katerik (DB_CONNECTION_STRING edo ConnectionStrings:Jatetxea)."));
    }

    /// <summary>
    /// ISessionFactory bakarra sortzen du (Singleton patroia, hari-segurua).
    /// </summary>
    public static ISessionFactory SortuSessionFactory(IConfiguration config)
    {
        string connStr = LortuKonexioKatea(config);

        if (_sessionFactory != null && string.Equals(_connectionString, connStr, StringComparison.Ordinal))
        {
            return _sessionFactory;
        }

        lock (_lock)
        {
            if (_sessionFactory != null && string.Equals(_connectionString, connStr, StringComparison.Ordinal))
            {
                return _sessionFactory;
            }

            _connectionString = connStr;
            _sessionFactory = Fluently.Configure()
                .Database(
                    MySQLConfiguration.Standard
                        .Driver<MySqlConnectorDriver>()
                        .Dialect<MySQL8InnoDBDialect>()
                        .ConnectionString(connStr)
                )
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<BezeroaMap>())
                .ExposeConfiguration(cfg =>
                {
                    // Ez konektatu DB-ra abiaraztean hitz-gakoak irakurtzeko:
                    // APIa abiarazi daiteke MySQL behin-behinean itzalita badago ere.
                    cfg.SetProperty(NHibernate.Cfg.Environment.Hbm2ddlKeyWords, "none");
                    // Ez exekutatu SchemaExport/SchemaUpdate automatikoki,
                    // jatetxea_db eskemak ENUM, CHECK, FK eta VIEW propioak baititu.
                })
                .BuildSessionFactory();

            return _sessionFactory;
        }
    }

    /// <summary>
    /// Aplikazio osorako ISessionFactory bakarra.
    /// </summary>
    public static ISessionFactory SessionFactory =>
        _sessionFactory ?? throw new InvalidOperationException("NHibernateHelper ez da hasieratu. Deitu SortuSessionFactory(config) Program.cs-en.");

    /// <summary>
    /// Lan-saio (ISession) berria irekitzen du. Beti 'using' bloke batean erabili behar da.
    /// </summary>
    public static NHibernate.ISession OpenSession() => SessionFactory.OpenSession();
}
