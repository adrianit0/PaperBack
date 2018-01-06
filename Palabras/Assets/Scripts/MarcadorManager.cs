using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarcadorManager : MonoBehaviour {
    public MarcadorJugador[] marcadores;
    MarcadorJugador actual;

    GameManager manager;

    void Awake() {
        manager = GetComponent<GameManager>();
    }

    public void Inicializar (int jugadores) {
        for (int i = 0; i < jugadores; i++) {
            CambiarTextos(i);
        }

        for (int i = jugadores; i < marcadores.Length; i++) {
            marcadores[i].panel.gameObject.SetActive(false);
        }
    }

    public void SeleccionarMarcador (int jugador) {
        StartCoroutine(TransicionOff());
        StartCoroutine(TransicionOn(jugador));
        actual = marcadores[jugador];
    }

    public void CambiarTextos (int jugador) {
        marcadores[jugador].textoNombre.text = TextoNombre(jugador);
        marcadores[jugador].textoInfo.text = TextoStats(jugador);
    }

    string TextoNombre (int jugador) {
        string nombre = manager.GetPlayer(jugador).GetName();
        return string.IsNullOrEmpty(nombre) ? "Jugador " + jugador : nombre;
    }

    string TextoStats (int jugador) {
        Jugador player = manager.GetPlayer(jugador);
        return "Fama: " + player.GetFame() + "★ | Score: " + player.GetMoney() + "€";
    }

    IEnumerator TransicionOn (int jugador) {
        RectTransform rect = marcadores[jugador].panel;
        Image imagen = rect.GetComponent<Image>();
        float posX = rect.anchoredPosition.x;

        float lerp = 0;
        while(lerp < 1) {
            lerp += Time.deltaTime*3;

            rect.anchoredPosition = new Vector2(Mathf.Lerp(posX, 20, lerp), rect.anchoredPosition.y);
            imagen.color = Color.Lerp(Color.white, Color.green, lerp);

            yield return null;
        }
    }

    IEnumerator TransicionOff () {
        if(actual == null)
            yield break;

        RectTransform rect = actual.panel;
        Image imagen = rect.GetComponent<Image>();
        float posX = rect.anchoredPosition.x;

        float lerp = 0;
        while (lerp < 1) {
            lerp += Time.deltaTime*3;

            rect.anchoredPosition = new Vector2(Mathf.Lerp (posX, 5, lerp), rect.anchoredPosition.y);
            imagen.color = Color.Lerp(Color.green, Color.white, lerp);

            yield return null;
        }
    }

}

[System.Serializable]
public class MarcadorJugador {
    public RectTransform panel;
    public Text textoNombre;
    public Text textoInfo;
}