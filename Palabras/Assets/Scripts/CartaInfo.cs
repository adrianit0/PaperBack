using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CartaInfo {
    public string letra;
    public int precio;
    public int premio;
    public int fama;
    public string descripcion;
    //Otra info
    public LUGAR lugarInicial;
    public int copias;

    public GameObject prefab;

    //Generica
    public CartaInfo () {
        letra = "";
        precio = 0;
        premio = 0;
        descripcion = "";
    }

    public CartaInfo (string letra) {
        this.letra = letra;

        precio = 0;
        premio = 0;
        fama = 0;
        descripcion = "";
        lugarInicial = LUGAR.Comun;
        copias = 0;
        prefab = null;
    }

    public CartaInfo (string letra, int precio, int premio, string descripcion, Color color) {
        this.letra = letra;
        this.precio = precio;
        this.premio = premio;
        this.descripcion = descripcion;
    }

    public CartaInfo (CartaInfo copia) {
        letra = copia.letra;
        precio = copia.precio;
        premio = copia.premio;
        fama = copia.fama;
        descripcion = copia.descripcion;

        lugarInicial = copia.lugarInicial;
        copias = copia.copias;
        prefab = copia.prefab;
    }
}
