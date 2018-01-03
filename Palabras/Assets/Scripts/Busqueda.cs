using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Busqueda : MonoBehaviour {
    public InputField input;
    public Button boton;
    public Text texto;

    BuscarPalabra buscador;

    void Awake () {
        buscador = new BuscarPalabra(this);
    }

    void Start () {
        boton.onClick.AddListener(() => RealizarBusqueda(input.text));
    }

    void Update() {
        if (Input.GetKey (KeyCode.Return))
            RealizarBusqueda(input.text);
    }

    void RealizarBusqueda (string busqueda) {
        if(string.IsNullOrEmpty(busqueda))
            return;

        busqueda = busqueda.ToLower();

        Stopwatch tiempo = Stopwatch.StartNew();
        string debug = "";

        int palabra = buscador.EncontrarPalabra(busqueda);

        if (palabra >-1) {
            debug += "La palabra \"" + buscador.GetLine(palabra) + "\" existe.\n";
            debug += "En la linea " + (palabra+1);
        } else {
            switch (palabra) {
                case -1:
                    debug += "Palabra \"" + busqueda + "\" no encontrada";
                    break;
                case -2:
                    debug += "Poner los comodines en alguno de los 2 primeros caracteres no esta incluido en esta version.";
                    break;
                case -3:
                    debug += "¡Demasiados comodines! Maximo admitible en una palabra son 10";
                    break;
                default:
                    debug += "Palabra \"" + busqueda + "\" no encontrada (Codigo de error "+-palabra+" desconocido)";
                    break;
            }
            
        }

        tiempo.Stop();
        debug += "\nTiempo de búsqueda: " + tiempo.Elapsed.Milliseconds.ToString() + "ms (" + tiempo.Elapsed.Ticks.ToString() + " ticks)";
        texto.text = debug;
    }

    public void Debug (object message) {
        UnityEngine.Debug.Log(message);
    }
}
