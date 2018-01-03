using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartaComun : MonoBehaviour {

    public Carta cartaFondo;
    public TextMesh texto;
    Carta seleccionada;
    
    int longitud = 7;
    List<Carta> cartas;
    GameManager manager;

    void Awake() {
        cartas = new List<Carta>();
        manager = GetComponent<GameManager>();
    }

    public void AddCarta (Carta carta) {
        if (carta.GetTipo () != LUGAR.Comun) {
            Debug.Log("La carta "+carta.GetName()+" no es comun");
            return;
        }

        cartas.Add(carta);
    }

    public void SeleccionarUna () {
        if (cartas.Count==0) {
            Debug.Log("No hay mas cartas comunes. No deberia la partida haber terminado (?)");
            return;
        }

        seleccionada = cartas[Random.Range(0, cartas.Count - 1)];
        cartas.Remove(seleccionada);
        cartaFondo.CopiarCarta(seleccionada);
        manager.zonaComun.AddCarta(seleccionada);
    }

    public void ComprobarPalabra (string palabra) {
        if(palabra.Length != longitud)
            return;

        //Da la carta al jugador actual
        DarCarta();

        //Aumenta en una la longitud
        longitud++;
        texto.text = longitud + " letras";

        if (longitud==11) {
            //Terminar partida

        } else {
            SeleccionarUna();
        }
    }

    public void DarCarta () {
        int jugador = manager.GetPlayerID();

        manager.AddCartaMano(seleccionada, jugador);
        manager.zonaComun.EliminarCarta(seleccionada);
        seleccionada.SetTipo(LUGAR.ManoInicial);

        SeleccionarUna();
    }
}
