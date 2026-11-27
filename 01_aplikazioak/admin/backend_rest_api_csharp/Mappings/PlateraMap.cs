using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class PlateraMap : ClassMap<Platera>
{
    public PlateraMap()
    {
        Table("platerak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        Map(x => x.Izena).Column("izena").Length(100).Not.Nullable();
        Map(x => x.Deskribapena).Column("deskribapena").CustomSqlType("text").Nullable();
        Map(x => x.Kategoria).Column("kategoria").Length(40).Not.Nullable();
        Map(x => x.Prezioa).Column("prezioa").Precision(6).Scale(2).Not.Nullable();
        Map(x => x.Alergenoak).Column("alergenoak").Length(200).Nullable();
        Map(x => x.Aktibo).Column("aktibo").Not.Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();

        HasMany(x => x.Osagaiak)
            .KeyColumn("platera_id")
            .Inverse()
            .LazyLoad();

        HasMany(x => x.EskaeraLerroak)
            .KeyColumn("platera_id")
            .Inverse()
            .LazyLoad();
    }
}
