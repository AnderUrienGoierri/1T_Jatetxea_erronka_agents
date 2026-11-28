using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class LehengaiaMap : ClassMap<Lehengaia>
{
    public LehengaiaMap()
    {
        Table("lehengaiak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        Map(x => x.Izena).Column("izena").Length(100).Not.Nullable().Unique();
        Map(x => x.Unitatea).Column("unitatea").Length(20).Not.Nullable();
        Map(x => x.UnekoStocka).Column("uneko_stocka").Precision(10).Scale(3).Not.Nullable();
        Map(x => x.GutxienekoStocka).Column("gutxieneko_stocka").Precision(10).Scale(3).Not.Nullable();
        Map(x => x.KostuUnitarioa).Column("kostu_unitarioa").Precision(8).Scale(2).Not.Nullable();
        References(x => x.HornitzaileaEntitatea)
            .Column("hornitzailea_id")
            .Nullable()
            .LazyLoad();
        Map(x => x.Hornitzailea).Column("hornitzailea").Length(120).Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();

        HasMany(x => x.PlaterOsagaiak)
            .KeyColumn("lehengaia_id")
            .Inverse()
            .LazyLoad();

        HasMany(x => x.StockMugimenduak)
            .KeyColumn("lehengaia_id")
            .Inverse()
            .LazyLoad();
    }
}
