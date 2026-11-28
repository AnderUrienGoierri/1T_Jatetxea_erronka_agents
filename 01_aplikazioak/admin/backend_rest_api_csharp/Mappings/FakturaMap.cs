using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class FakturaMap : ClassMap<Faktura>
{
    public FakturaMap()
    {
        Table("fakturak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        References(x => x.Eskaera)
            .Column("eskaera_id")
            .Nullable()
            .LazyLoad();

        References(x => x.HornitzaileaEntitatea)
            .Column("hornitzailea_id")
            .Nullable()
            .LazyLoad();

        Map(x => x.FakturaZenbakia).Column("faktura_zenbakia").Length(50).Not.Nullable().Unique();
        Map(x => x.Mota).Column("mota").Length(30).Not.Nullable();
        Map(x => x.Azpimota).Column("azpimota").Length(30).Not.Nullable();
        Map(x => x.Kontzeptua).Column("kontzeptua").Length(255).Not.Nullable();
        Map(x => x.Hornitzailea).Column("hornitzailea").Length(120).Nullable();
        Map(x => x.HornitzaileCif).Column("hornitzaile_cif").Length(20).Nullable();
        Map(x => x.BezeroNif).Column("bezero_nif").Length(20).Nullable();
        Map(x => x.BezeroIzena).Column("bezero_izena").Length(120).Nullable();
        Map(x => x.BezeroHelbidea).Column("bezero_helbidea").Length(200).Nullable();
        Map(x => x.OinarriZergagarria).Column("oinarri_zergagarria").Precision(10).Scale(2).Not.Nullable();
        Map(x => x.BezEhunekoa).Column("bez_ehunekoa").Precision(4).Scale(2).Not.Nullable();
        Map(x => x.BezZenbatekoa).Column("bez_zenbatekoa").Precision(10).Scale(2).Not.Nullable();
        Map(x => x.Guztira).Column("guztira").Precision(10).Scale(2).Not.Nullable();
        Map(x => x.OrdainketaMetodoa).Column("ordainketa_metodoa").Length(50).Not.Nullable();
        Map(x => x.Egoera).Column("egoera").Length(30).Not.Nullable();
        Map(x => x.Data).Column("data").Not.Nullable();
        Map(x => x.EpemugaData).Column("epemuga_data").CustomType("Date").Nullable();
        Map(x => x.Oharrak).Column("oharrak").CustomSqlType("text").Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();
    }
}
