using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class HornitzaileaMap : ClassMap<Hornitzailea>
{
    public HornitzaileaMap()
    {
        Table("hornitzaileak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        Map(x => x.Izena).Column("izena").Length(120).Not.Nullable().Unique();
        Map(x => x.Cif).Column("cif").Length(20).Not.Nullable().Unique();
        Map(x => x.ProduktuMota).Column("produktu_mota").Length(80).Not.Nullable();
        Map(x => x.BezEhunekoa).Column("bez_ehunekoa").Precision(4).Scale(2).Not.Nullable();
        Map(x => x.Telefonoa).Column("telefonoa").Length(20).Nullable();
        Map(x => x.Emaila).Column("emaila").Length(120).Nullable();
        Map(x => x.Helbidea).Column("helbidea").Length(200).Nullable();
        Map(x => x.Aktibo).Column("aktibo").Not.Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();

        HasMany(x => x.Lehengaiak)
            .KeyColumn("hornitzailea_id")
            .Inverse()
            .LazyLoad();

        HasMany(x => x.Fakturak)
            .KeyColumn("hornitzailea_id")
            .Inverse()
            .LazyLoad();
    }
}
