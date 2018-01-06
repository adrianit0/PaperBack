using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaJuego : MonoBehaviour {

    public bool soloComun = false;

    List<Carta> cartas = new List<Carta>();

    GameManager manager;

    float sep = 1.5f;
    public float tam = 7;
    public float fondo = -0.2f;

    public void Configurar (GameManager manager) {
        this.manager = manager;
    }

    public string CrearPalabra () {
        string palabra = "";
        for (int i = 0; i < cartas.Count; i++) {
            palabra += cartas[i].textoLetra.text;
        }
        return palabra;
    }

    public int CalcularPremio () {
        int cantidad = 0;
        for(int i = 0; i < cartas.Count; i++) {
            cantidad += cartas[i].GetPremio();
        }
        return cantidad;
    }

    public void AddCarta (Carta carta) {
        cartas.Add(carta);
        MoverCartasAuto();
    }

    public void AddCarta (Carta carta, int pos) {
        cartas.Insert(pos, carta);
        MoverCartasAuto();
    }

    public void AddCarta (Carta carta, float pos) {
        if (cartas.Count==0) {
            AddCarta(carta);
            return;
        }
        if (pos<cartas[0].posX) {
            AddCarta(carta, 0);
            return;
        }
        int cantidad = cartas.Count - 1;
        for (int i = 0; i < cantidad; i++) {
            if (Cercania(i, pos)) {
                AddCarta(carta, i+1);
                return;
            }
        }

        AddCarta(carta);
    }
    

    /// <summary>
    /// Coge una carta de la zona de juego
    /// </summary>
    public void CogerCarta (Carta carta) {
        if (manager.SeleccionarCarta(carta, carta.posX, this)) {
            cartas.Remove(carta);
            MoverCartasAuto();
        }
    }

    public void EliminarCarta (Carta carta) {
        if (cartas.Contains (carta)) {
            cartas.Remove(carta);
        } else {
            Debug.Log("La carta " + carta.GetName() + " no esta en el deck");
        }
    }

    public void CogerCarta (float posX) {
        if(cartas.Count == 0)
            return;

        int cantidad = cartas.Count - 1;
        for(int i = 0; i < cantidad; i++) {
            if(Cercania (i, posX)) {
                CogerCarta(cartas[i]);
                return;
            }
        }

        CogerCarta(cartas[cartas.Count-1]);
    }

    /// <summary>
    /// Fijas la carta
    /// </summary>
    public void FijarCarta (Carta carta) {
        for (int i = 0; i < cartas.Count; i++) { 
            if (cartas[i].transform.position.x > carta.transform.position.x) {
                cartas.Insert(i, carta);
                return;
            }
        }
        cartas.Add(carta);
    }

    public Carta[] DescartarMano () {
        Carta[] _lista = cartas.ToArray();
        cartas = new List<Carta>();

        return _lista;
    }

    public void MoverCartasAuto () {
        float cant = cartas.Count;
        float _sep = sep;
        if (_sep*cant>tam) {
            _sep = tam / cant;
        }

        for (int i = 0; i < cartas.Count; i++) {
            float pos = transform.position.x + (-1 * ((cant - 1) * _sep) / 2 + _sep * i);
            if (!cartas[i].IsTouching())
                manager.MoverCarta(cartas[i].gameObject, new Vector3(pos, transform.position.y, fondo));

            cartas[i].ChangeLayer(i);
        }
    }

    public void MoverCartasAuto(float exluido) {
        if(cartas.Count == 0)
            return;

        float cant = cartas.Count+1;
        float _sep = sep;
        if(_sep * cant > tam) {
            _sep = tam / cant;
        }

        int x = (exluido<cartas[0].posX && !Cercania(0, exluido)) ? 1 : 0;
        for(int i = 0; i < cartas.Count; i++) {
            if(!cartas[i].IsTouching()) {
                float pos = transform.position.x + (-1 * ((cant - 1) * _sep) / 2 + _sep * x);
                manager.MoverCarta(cartas[i].gameObject, new Vector3(pos, transform.position.y, fondo), true);
            }

            if(Cercania(i, exluido))
                x++;

            x++;
        }
    }

    bool Cercania (int valor, float point) {
        if(valor >= cartas.Count-1)
            return true;
        

        float punto1 = cartas[valor].posX;
        float punto2 = cartas[valor + 1].posX;

        float punto0 = (punto1 - (punto2 - punto1));

        //Debug.Log(valor + " | " + punto1 +" | "+ point +" |" +" | " + punto2);

        //if(point < punto1 || point > punto2)
        //    return false

        return (point > ((punto0+punto1) / 2) && point < ((punto1 + punto2) / 2));
        

        //return (point - punto1) <= ((punto2 - punto1) / 2);
    }
}