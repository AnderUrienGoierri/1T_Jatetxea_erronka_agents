using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class MahaiaMap : ClassMap<Mahaia>
{
    public MahaiaMap()
    {
        Table("mahaiak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        Map(x => x.Zenbakia).Column("zenbakia").Not.Nullable().Unique();
        Map(x => x.Edukiera).Column("edukiera").Not.Nullable();
        Map(x => x.Kokapena).Column("kokapena").Length(50).Not.Nullable();
        Map(x => x.Egoera).Column("egoera").Length(30).Not.Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();

        HasMany(x => x.Erreserbak)
            .KeyColumn("mahaia_id")
            .Inverse()
            .LazyLoad();

        HasMany(x => x.Eskaerak)
            .KeyColumn("mahaia_id")
            .Inverse()
            .LazyLoad();
    }
}
