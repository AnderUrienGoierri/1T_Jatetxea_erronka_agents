using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class EskaeraMap : ClassMap<Eskaera>
{
    public EskaeraMap()
    {
        Table("eskaerak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        References(x => x.Mahaia)
            .Column("mahaia_id")
            .Not.Nullable()
            .LazyLoad();

        References(x => x.Langilea)
            .Column("langilea_id")
            .Not.Nullable()
            .LazyLoad();

        Map(x => x.Egoera).Column("egoera").Length(30).Not.Nullable();
        Map(x => x.Guztira).Column("guztira").Precision(8).Scale(2).Not.Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();

        HasMany(x => x.Lerroak)
            .KeyColumn("eskaera_id")
            .Inverse()
            .LazyLoad();

        HasMany(x => x.Ordainketak)
            .KeyColumn("eskaera_id")
            .Inverse()
            .LazyLoad();
    }
}
