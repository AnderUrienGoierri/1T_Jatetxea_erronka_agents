using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class IritziaMap : ClassMap<Iritzia>
{
    public IritziaMap()
    {
        Table("iritziak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        References(x => x.Bezeroa)
            .Column("bezeroa_id")
            .Not.Nullable()
            .LazyLoad();

        Map(x => x.Balorazioa).Column("balorazioa").Not.Nullable();
        Map(x => x.Testua).Column("testua").CustomSqlType("text").Nullable();
        Map(x => x.Aktibo).Column("aktibo").Not.Nullable();
        Map(x => x.Ezabatua).Column("ezabatua").Not.Nullable();
        Map(x => x.EzabatzeData).Column("ezabatze_data").Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();
    }
}
