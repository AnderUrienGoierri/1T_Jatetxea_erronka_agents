using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class StockMugimenduaMap : ClassMap<StockMugimendua>
{
    public StockMugimenduaMap()
    {
        Table("stock_mugimenduak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        References(x => x.Lehengaia)
            .Column("lehengaia_id")
            .Not.Nullable()
            .LazyLoad();

        Map(x => x.Mota).Column("mota").Length(40).Not.Nullable();
        Map(x => x.Kantitatea).Column("kantitatea").Precision(10).Scale(3).Not.Nullable();

        References(x => x.Eskaera)
            .Column("eskaera_id")
            .Nullable()
            .LazyLoad();

        Map(x => x.AlbaranZenbakia).Column("albaran_zenbakia").Length(50).Nullable();

        References(x => x.Faktura)
            .Column("faktura_id")
            .Nullable()
            .LazyLoad();

        Map(x => x.Oharrak).Column("oharrak").Length(200).Nullable();
        Map(x => x.Data).Column("data").Not.Nullable();
    }
}
