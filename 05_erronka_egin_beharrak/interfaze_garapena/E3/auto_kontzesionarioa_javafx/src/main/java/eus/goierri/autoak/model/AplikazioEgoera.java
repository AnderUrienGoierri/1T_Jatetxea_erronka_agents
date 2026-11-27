package eus.goierri.autoak.model;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/**
 * Bi pantailen artean partekatutako egoera nagusia:
 * 1. pantailako iragazkiak eta auto bakoitzaren pertsonalizazioak gordetzen ditu.
 */
public final class AplikazioEgoera {
    private final List<Autoa> katalogoa;
    private final IragazkiEgoera iragazkiak = new IragazkiEgoera();
    private final Map<String, AutoKonfigurazioa> konfigurazioak = new LinkedHashMap<>();
    private Autoa hautatutakoAutoa;

    public AplikazioEgoera(List<Autoa> katalogoa) {
        this.katalogoa = katalogoa;
        if (!katalogoa.isEmpty()) {
            this.hautatutakoAutoa = katalogoa.get(0);
        }
    }

    public List<Autoa> getKatalogoa() {
        return katalogoa;
    }

    public IragazkiEgoera getIragazkiak() {
        return iragazkiak;
    }

    public Autoa getHautatutakoAutoa() {
        return hautatutakoAutoa;
    }

    public void setHautatutakoAutoa(Autoa hautatutakoAutoa) {
        this.hautatutakoAutoa = hautatutakoAutoa;
    }

    public AutoKonfigurazioa lortuKonfigurazioa(Autoa autoa) {
        return konfigurazioak.computeIfAbsent(autoa.id(), id -> new AutoKonfigurazioa(autoa));
    }

    public boolean baduPertsonalizazioa(Autoa autoa) {
        AutoKonfigurazioa cfg = konfigurazioak.get(autoa.id());
        return cfg != null && cfg.isPertsonalizatua();
    }
}
