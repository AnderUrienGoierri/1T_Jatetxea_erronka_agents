using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

/// <summary>
/// TPV mahaigaineko formulario guztien oinarrizko klasea (Herentzia + IDpiEgokigarria interfazea).
/// Warm Hospitality gaia eta PerMonitorV2 DPI egokitzapen komuna eskaintzen ditu
/// (klase ez-abstraktua Visual Studio WinForms Designer-ekin bateragarria izateko).
/// </summary>
public class BaseTpvForm : Form, IDpiEgokigarria
{
    public virtual void AplikatuGaiKomuna()
    {
        Theme.ApplyWarmHospitalityTheme(this);
    }

    public virtual void EgokituDpiAldaketan()
    {
        // Azpiklase bakoitzak bere diseinu responsive-a berritzeko gainidatzi dezake
    }

    protected void AktibatuDpiEgokitzapena(int minLogicalW = 800, int minLogicalH = 500)
    {
        Theme.AttachMainFormDpiAdaptation(this, EgokituDpiAldaketan, minLogicalW, minLogicalH);
    }
}
