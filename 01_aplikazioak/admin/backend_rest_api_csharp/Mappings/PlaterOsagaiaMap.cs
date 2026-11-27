using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

public class PlaterOsagaiaMap : ClassMap<PlaterOsagaia>
{
    public PlaterOsagaiaMap()
    {
        Table("plater_osagaiak");

        CompositeId()
            .KeyProperty(x => x.PlateraId, "platera_id")
            .KeyProperty(x => x.LehengaiaId, "lehengaia_id");

        Map(x => x.Kantitatea).Column("kantitatea").Precision(10).Scale(3).Not.Nullable();

        References(x => x.Platera)
            .Column("platera_id")
            .Not.Insert()
            .Not.Update()
            .LazyLoad();

        References(x => x.Lehengaia)
            .Column("lehengaia_id")
            .Not.Insert()
            .Not.Update()
            .LazyLoad();
    }
}
