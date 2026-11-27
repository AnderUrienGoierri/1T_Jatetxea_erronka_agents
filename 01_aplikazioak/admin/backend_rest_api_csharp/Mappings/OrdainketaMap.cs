using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class OrdainketaMap : ClassMap<Ordainketa>
{
    public OrdainketaMap()
    {
        Table("ordainketak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        References(x => x.Eskaera)
            .Column("eskaera_id")
            .Not.Nullable()
            .LazyLoad();

        Map(x => x.TiketZenbakia).Column("tiket_zenbakia").Length(50).Not.Nullable().Unique();
        Map(x => x.Mota).Column("mota").Length(30).Not.Nullable();
        Map(x => x.Zenbatekoa).Column("zenbatekoa").Precision(8).Scale(2).Not.Nullable();
        Map(x => x.Jasotakoa).Column("jasotakoa").Precision(8).Scale(2).Nullable();
        Map(x => x.Itzulitakoa).Column("itzulitakoa").Precision(8).Scale(2).Nullable();
        Map(x => x.TxartelMaskaratua).Column("txartel_maskaratua").Length(30).Nullable();
        Map(x => x.TxartelMarka).Column("txartel_marka").Length(30).Nullable();
        Map(x => x.OperazioKodea).Column("operazio_kodea").Length(60).Not.Nullable();
        Map(x => x.TokenZifratua).Column("token_zifratua").Length(120).Nullable();
        Map(x => x.Egoera).Column("egoera").Length(30).Not.Nullable();
        Map(x => x.LangileaId).Column("langilea_id").Nullable();
        Map(x => x.TiketTestua).Column("tiket_testua").CustomSqlType("mediumtext").Nullable();
        Map(x => x.Data).Column("data").Not.Nullable();
    }
}
