using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class EskaeraLerroaMap : ClassMap<EskaeraLerroa>
{
    public EskaeraLerroaMap()
    {
        Table("eskaera_lerroak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        References(x => x.Eskaera)
            .Column("eskaera_id")
            .Not.Nullable()
            .LazyLoad();

        References(x => x.Platera)
            .Column("platera_id")
            .Not.Nullable()
            .LazyLoad();

        Map(x => x.Kantitatea).Column("kantitatea").Not.Nullable();
        Map(x => x.UnitatePrezioa).Column("unitate_prezioa").Precision(6).Scale(2).Not.Nullable();
        Map(x => x.Egoera).Column("egoera").Length(30).Not.Nullable();
        Map(x => x.Oharrak).Column("oharrak").Length(150).Nullable();
    }
}
