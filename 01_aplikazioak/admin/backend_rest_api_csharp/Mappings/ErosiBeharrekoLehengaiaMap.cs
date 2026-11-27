using FluentNHibernate.Mapping;
using Jatetxea.Api.Models;

namespace Jatetxea.Api.Mappings;

/// <summary>
/// v_erosi_beharreko_lehengaiak MySQL ikuspegiaren irakurketa-soileko mapeoa.
/// </summary>
public class ErosiBeharrekoLehengaiaMap : ClassMap<ErosiBeharrekoLehengaia>
{
    public ErosiBeharrekoLehengaiaMap()
    {
        Table("v_erosi_beharreko_lehengaiak");
        ReadOnly();
        SchemaAction.None();

        Id(x => x.LehengaiaId).Column("lehengaia_id").GeneratedBy.Assigned();

        Map(x => x.Izena).Column("izena");
        Map(x => x.Unitatea).Column("unitatea");
        Map(x => x.UnekoStocka).Column("uneko_stocka");
        Map(x => x.GutxienekoStocka).Column("gutxieneko_stocka");
        Map(x => x.ProposatutakoErosketa).Column("proposatutako_erosketa");
        Map(x => x.Hornitzailea).Column("hornitzailea");
    }
}
