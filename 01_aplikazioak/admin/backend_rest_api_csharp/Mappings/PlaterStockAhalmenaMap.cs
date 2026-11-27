using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

/// <summary>
/// v_plateren_stock_ahalmena MySQL ikuspegiaren irakurketa-soileko mapeoa.
/// </summary>
public class PlaterStockAhalmenaMap : ClassMap<PlaterStockAhalmena>
{
    public PlaterStockAhalmenaMap()
    {
        Table("v_plateren_stock_ahalmena");
        ReadOnly();
        SchemaAction.None();

        Id(x => x.PlateraId).Column("platera_id").GeneratedBy.Assigned();

        Map(x => x.PlaterIzena).Column("plater_izena");
        Map(x => x.EginDaitezkeenPlaterak).Column("egin_daitezkeen_platerak");
    }
}
