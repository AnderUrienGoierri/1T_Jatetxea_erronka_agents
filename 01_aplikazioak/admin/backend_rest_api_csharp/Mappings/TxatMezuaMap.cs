using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class TxatMezuaMap : ClassMap<TxatMezua>
{
    public TxatMezuaMap()
    {
        Table("txat_mezuak");

        Id(x => x.Id).Column("id").GeneratedBy.Identity();

        References(x => x.Langilea)
            .Column("langilea_id")
            .Not.Nullable();

        Map(x => x.Gela).Column("gela").Length(40).Not.Nullable();
        Map(x => x.Testua).Column("testua").CustomSqlType("TEXT").Not.Nullable();
        Map(x => x.SortzeData).Column("sortze_data").Not.Nullable();
    }
}
