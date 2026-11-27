using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class ErreserbaMap : ClassMap<Erreserba>
{
    public ErreserbaMap()
    {
        Table("erreserbak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        References(x => x.Bezeroa)
            .Column("bezeroa_id")
            .Not.Nullable()
            .LazyLoad();

        References(x => x.Mahaia)
            .Column("mahaia_id")
            .Not.Nullable()
            .LazyLoad();

        Map(x => x.Data).Column("data").CustomType("Date").Not.Nullable();
        Map(x => x.Txanda).Column("txanda").Length(30).Not.Nullable();
        Map(x => x.PertsonaKopurua).Column("pertsona_kopurua").Not.Nullable();
        Map(x => x.Egoera).Column("egoera").Length(30).Not.Nullable();
        Map(x => x.Oharrak).Column("oharrak").Length(255).Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();
    }
}
