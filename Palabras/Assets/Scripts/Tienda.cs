using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tienda {

    Dictionary<int, List<Carta>> cartasLetras;
    Dictionary<int, List<Carta>> cartasFama;

    public Tienda () {
        //TIENDA CARTAS LETRAS
        cartasLetras = new Dictionary<int, List<Carta>>();
        for (int i = 2; i < 8; i++) {
            cartasLetras.Add(i, new List<Carta>());
        }
        List<Carta> lista = new List<Carta>();
        cartasLetras.Add(8, lista);
        cartasLetras.Add(9, lista);
        cartasLetras.Add(10, lista);

        //TIENDA CARTAS FAMA
        cartasFama = new Dictionary<int, List<Carta>>();
        cartasFama.Add(5, new List<Carta>());
        cartasFama.Add(8, new List<Carta>());
        cartasFama.Add(11, new List<Carta>());
        cartasFama.Add(17, new List<Carta>());
    }
    
    public void AddCartaLetra (Carta carta) {
        if(carta.GetTipo() != LUGAR.Tienda) { 
            Debug.Log("Se esta intentando añadir una carta no compatible en esta tienda. ("+carta.GetTipo()+")");
            return;
        }

        List<Carta> lista;
        if (!cartasLetras.TryGetValue (carta.GetPrecio(), out lista)) {
            Debug.Log("ERROR: El precio esta fuera del rango ("+carta.GetName()+")");
            return;
        }

        lista.Add(carta);
    }

    public void AddCartaFama(Carta carta) {
        if(carta.GetTipo() != LUGAR.Fama) {
            Debug.Log("Se esta intentando añadir una carta no compatible en esta tienda. (" + carta.GetTipo() + ")");
            return;
        }

        List<Carta> lista;
        if(!cartasFama.TryGetValue(carta.GetPrecio(), out lista)) {
            Debug.Log("ERROR: El precio esta fuera del rango (" + carta.GetPrecio() + ")");
            return;
        }

        lista.Add(carta);
    }

    public void BarajarMazos () {
        Dictionary<int, List<Carta>> _mazos = new Dictionary<int, List<Carta>>();
        foreach (KeyValuePair<int, List<Carta>> mazos in cartasLetras) {
            //Debug.Log("ANTES || PRECIO: " + mazos.Key + " CANTIDAD: " + mazos.Value.Count);
            _mazos.Add (mazos.Key, BarajarMazo(mazos.Value));
            //Debug.Log("DESPUES || PRECIO: " + mazos.Key + " CANTIDAD: " + mazos.Value.Count);
        }
        cartasLetras = _mazos;
    }

    List<Carta> BarajarMazo (List<Carta> mazo) {
        if(mazo == null || mazo.Count == 0) {
            return new List<Carta>();
        }

        List<Carta> nuevoMazo = mazo;
        mazo = new List<Carta>();

        while(nuevoMazo.Count > 0) {
            Carta _carta = nuevoMazo[Random.Range(0, nuevoMazo.Count)];
            mazo.Add(_carta);
            nuevoMazo.Remove(_carta);
        }

        return mazo;
    }

    public Carta GetCard (int precioReal, int pos) {
        List<Carta> _lista;
        if (cartasLetras.TryGetValue(precioReal, out _lista)) {
            if (_lista!=null && _lista.Count>pos) {
                return _lista[pos];
            }
        }

        return null;
    }
}
