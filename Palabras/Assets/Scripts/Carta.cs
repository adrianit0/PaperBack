using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LUGAR {
    Tienda,
    ManoInicial,
    Fama,
    Comun,
    Reward,
    Power
}

public class Carta : MonoBehaviour {

    CartaInfo info;
    
    public Canvas canvas;
    public Image imagen;
    public Text textoLetra;
    public Text textoPremio;
    public Text textoFama;
    public Text textoDescripcion;

    bool isTouching = false;

    public float posX = 0;
    public int ite = 0;
    
    public void ConfigurarCarta (CartaInfo carta) {
        info = carta;
        
        textoLetra.text = carta.letra;
        textoPremio.text = carta.premio.ToString();
        textoDescripcion.text = carta.descripcion;
        textoFama.text = carta.fama + "★";
        textoFama.gameObject.SetActive(carta.fama > 0);
    }

    public void CopiarCarta (Carta carta) {
        if (carta == null || carta == this) {
            return;
        }

        ConfigurarCarta(new CartaInfo(carta.GetInfo()));
    }

    public CartaInfo GetInfo () {
        return info;
    }

    public string GetTrueLetter () {
        return info.letra;
    }

    public void SetVisualLetterForComodin (string letter) { 
        //Solo valen comodines
        if (info.letra!= "*") {
            return;
        }
        //Solo pueden ser de 1 valor
        if (letter.Length!=1) {
            return;
        }

        textoLetra.text = letter;
        textoLetra.color = (letter == "*") ? Color.black : Color.red;
    }

    public bool IsTouching () {
        return isTouching;
    }

    public void setTouch (bool set) {
        isTouching = set;
    }

    public void ChangeLayer (int nueva) {
        canvas.sortingOrder = nueva;
    }

    public void SelectCarta (bool selected) {
        canvas.sortingLayerName = (selected) ? "Seleccionado" : "Cartas";
    }

    public int GetPremio () {
        return info.premio;
    }

    public int GetPrecio () {
        return info.precio;
    }

    public LUGAR GetTipo () {
        return info.lugarInicial;
    }

    public void SetTipo (LUGAR nuevoTipo) {
        info.lugarInicial = nuevoTipo;
    }

    public string GetName() {
        return "[" + info.letra + "] " +info.precio+"€/"+info.premio+"€/"+info.fama+ "★";
    }
}
