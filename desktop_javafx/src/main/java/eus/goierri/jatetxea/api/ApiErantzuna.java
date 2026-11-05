package eus.goierri.jatetxea.api;

import java.util.List;

/**
 * Backend APIaren (ApiResponse) erantzun estandarra jasotzeko klase generikoa.
 *
 * @param <T> Datu mota (Eskaera, Lehengaia, Platera...)
 */
public class ApiErantzuna<T> {
    private String status;
    private int code;
    private String message;
    private T data;
    private List<T> dataList;

    public String getStatus() {
        return status;
    }

    public int getCode() {
        return code;
    }

    public String getMessage() {
        return message;
    }

    public T getData() {
        return data;
    }

    public List<T> getDataList() {
        return dataList;
    }
}
