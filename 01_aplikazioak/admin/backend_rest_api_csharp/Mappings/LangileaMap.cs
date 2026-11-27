using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class LangileaMap : ClassMap<Langilea>
{
    public LangileaMap()
    {
        Table("langileak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        Map(x => x.Erabiltzailea).Column("erabiltzailea").Length(50).Not.Nullable().Unique();
        Map(x => x.PasahitzaHash).Column("pasahitza_hash").Length(255).Not.Nullable();
        Map(x => x.Izena).Column("izena").Length(80).Not.Nullable();
        Map(x => x.Abizena).Column("abizena").Length(100).Not.Nullable();
        Map(x => x.NanDni).Column("nan_dni").Length(20).Not.Nullable().Unique();
        Map(x => x.Emaila).Column("emaila").Length(120).Not.Nullable().Unique();
        Map(x => x.Telefonoa).Column("telefonoa").Length(20).Nullable();
        Map(x => x.Rola).Column("rola").Length(40).Not.Nullable();
        Map(x => x.Saila).Column("saila").Length(40).Not.Nullable();
        Map(x => x.Kargua).Column("kargua").Length(100).Not.Nullable();
        Map(x => x.OdooEmployeeId).Column("odoo_employee_id").Nullable();
        Map(x => x.Aktibo).Column("aktibo").Not.Nullable();
        Map(x => x.TxataAktibo).Column("txata_aktibo").Not.Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();

        HasMany(x => x.Eskaerak)
            .KeyColumn("langilea_id")
            .Inverse()
            .LazyLoad();
    }
}
