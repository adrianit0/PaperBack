using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador {
    string nombre;
    List<Carta> mazoRobo;
    List<Carta> mazoDescarte;
    List<Carta> mazoEliminado;
    List<Carta> mano;

    int dinero;

    public Jugador (string nombre) {
        this.nombre = nombre;
        dinero = 0;

        mazoRobo = new List<Carta>();
        mazoDescarte = new List<Carta>();
        mazoEliminado = new List<Carta>();
        mano = new List<Carta>();
    }

    public void BarajarMazo() {
        List<Carta> _mazo = mazoRobo;
        mazoRobo = new List<Carta>();

        for(int i = 0; _mazo.Count > 0; i++) {
            Carta _carta = _mazo[Random.Range(0, _mazo.Count)];
            mazoRobo.Add(_carta);
            _mazo.Remove(_carta);
        }
    }

    public void StartTurn () {
        dinero = 0;
    }

    public string GetName() {
        return nombre;
    }

    public int GetFame () {
        int cantidad = 0;

        for (int i=0; i<mazoRobo.Count; i++) {
            cantidad += mazoRobo[i].GetFame();
        }
        for(int i = 0; i < mazoDescarte.Count; i++) {
            cantidad += mazoDescarte[i].GetFame();
        }

        return cantidad;
    }

    public Carta GetTopCardFromDrawDeck () {
        if(mazoRobo.Count == 0)
            return null;

        Carta _card = mazoRobo[0];
        mazoRobo.Remove(_card);
        return _card;
    }

    public void FillDrawDeck () {
        mazoRobo = new List<Carta>(mazoDescarte);
        mazoDescarte = new List<Carta>();
    }

    public void AddCartaMano (Carta carta) {
        //TODO: Hacer que tenga que robar.
        mano.Add(carta);
    }

    public void AddCarta (Carta carta, bool incluirEnMazoDescarte) {
        if(incluirEnMazoDescarte)
            mazoDescarte.Add(carta);
        else
            mazoRobo.Add(carta);
    }

    public void DiscardCard (Carta carta) {
        AddCarta(carta, true);
    }

    public bool ContainCard (Carta carta) {
        return mano.Contains(carta);
    }

    public void EliminarCarta (Carta carta) {
        //TODO: La carta eliminada tiene que estar en la mano.
        mazoEliminado.Add(carta);
    }

    //TAMAÑO
    public int GetDeckLength(bool robo) {
        if(robo)
            return mazoRobo.Count;
        else
            return mazoDescarte.Count;
    }

    public int GetMoney() {
        return dinero;
    }

    public void ReceiveMoney (int cantidad) {
        dinero += cantidad;
    }

    public bool SpendMoney (int cantidad) {
        //Si la cantidad a gastar es mayor no permitira comprar la letra
        if(cantidad > dinero)
            return false;

        dinero -= cantidad;
        return true;
    }
}
