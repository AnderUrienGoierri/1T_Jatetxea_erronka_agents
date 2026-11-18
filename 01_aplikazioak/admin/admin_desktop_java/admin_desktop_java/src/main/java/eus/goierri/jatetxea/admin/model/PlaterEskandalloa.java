package eus.goierri.jatetxea.admin.model;

import java.util.ArrayList;
import java.util.List;

public final class PlaterEskandalloa {
    public int plateraId;
    public String platerIzena;
    public double prezioa;
    public List<RecipeIngredient> osagaiak = new ArrayList<>();

    public static final class RecipeIngredient {
        public int lehengaiaId;
        public String lehengaiaIzena;
        public double kantitatea;
        public String unitatea;
    }
}
