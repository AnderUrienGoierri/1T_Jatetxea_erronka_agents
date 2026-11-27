using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class BezeroaMap : ClassMap<Bezeroa>
{
    public BezeroaMap()
    {
        Table("bezeroak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        Map(x => x.Izena).Column("izena").Length(80).Not.Nullable();
        Map(x => x.Abizena).Column("abizena").Length(100).Not.Nullable();
        Map(x => x.Telefonoa).Column("telefonoa").Length(20).Not.Nullable();
        Map(x => x.Emaila).Column("emaila").Length(120).Not.Nullable().Unique();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();

        HasMany(x => x.Erreserbak)
            .KeyColumn("bezeroa_id")
            .Inverse()
            .LazyLoad();

        HasMany(x => x.Iritziak)
            .KeyColumn("bezeroa_id")
            .Inverse()
            .LazyLoad();
    }
}
