namespace Jatetxea.Api.Helpers;

public static class SegurtasunHelper
{
    private const int WorkFactor = 11;

    /// <summary>
    /// Testu lau moduko PIN/pasahitz baten BCrypt hash-a sortzen du.
    /// </summary>
    public static string HashPin(string pin)
    {
        if (string.IsNullOrEmpty(pin)) return string.Empty;
        return BCrypt.Net.BCrypt.HashPassword(pin.Trim(), WorkFactor);
    }

    /// <summary>
    /// PINa egiaztatzen du gordetako balioaren aurka (BCrypt hash edo behin-behineko testu laua).
    /// </summary>
    /// <param name="pinSartua">Erabiltzaileak sartutako PINa.</param>
    /// <param name="pasahitzaGordea">Datu-basean gordetako pasahitza_hash balioa.</param>
    /// <param name="berrituBeharDa">True itzultzen du balioa testu laua bazen eta BCrypt-era migratu behar bada.</param>
    /// <returns>True PINa zuzena bada.</returns>
    public static bool EgiaztatuPin(string pinSartua, string pasahitzaGordea, out bool berrituBeharDa)
    {
        berrituBeharDa = false;

        if (string.IsNullOrEmpty(pinSartua) || string.IsNullOrEmpty(pasahitzaGordea))
            return false;

        string garbia = pinSartua.Trim();
        string gordea = pasahitzaGordea.Trim();

        // 1. Egiaztatu BCrypt hash formatua ($2a$, $2b$, $2y$, $2x$)
        if (IsBcryptHash(gordea))
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(garbia, gordea);
            }
            catch
            {
                return false;
            }
        }

        // 2. Aldi baterako migrazio leuna: testu laua bada, zuzenean konparatu eta migrazioa markatu
        if (string.Equals(garbia, gordea, StringComparison.Ordinal))
        {
            berrituBeharDa = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Kate batek BCrypt hash egitura duen egiaztatzen du.
    /// </summary>
    public static bool IsBcryptHash(string balioa)
    {
        if (string.IsNullOrEmpty(balioa) || balioa.Length < 50) return false;
        return balioa.StartsWith("$2a$") || balioa.StartsWith("$2b$") ||
               balioa.StartsWith("$2y$") || balioa.StartsWith("$2x$");
    }
}
