using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComodinManager : MonoBehaviour {

    public GameObject panel;
    public GameObject prefab;

    Carta comodin;

    GameManager manager;

    Vector3 inicialPos = new Vector3(-3.9f, 1.1f, -0.2f);
    float sepX = 1f;
    float sepY = 1.2f;

    void Awake() {
        manager = GetComponent<GameManager>();
    }

    void Start() {
        CrearPanel();
        panel.SetActive(false);
    }

    void CrearPanel () {
        string[] indice = manager.DevolverIndiceSimple(true).ToArray();

        int x = 0;
        int totalX = 9;
        int y = 0;
        for (int i = 0; i < indice.Length; i++) {
            GameObject _obj = Instantiate(prefab);
            _obj.transform.parent = panel.transform;

            Carta _carta = _obj.GetComponent<Carta>();
            if(_carta != null) {
                _carta.ConfigurarCarta(new CartaInfo(indice[i].ToUpper()));
                Boton _boton = _obj.GetComponent<Boton>();
                _boton.pulsarBoton.AddListener(() => SeleccionarBoton(_carta, _boton));
            }
                
            _obj.transform.localPosition = new Vector3(inicialPos.x + sepX * x, inicialPos.y - sepY * y, inicialPos.z);

            x++;

            if(x == totalX) {
                x = 0;
                y++;
                totalX = (y % 2 == 0) ? 9 : 10;
                inicialPos.x = (y % 2 == 0) ? -3.9f : -4.5f;
            }
        }
    }

    public void SeleccionarComodin (Carta carta) {
        if (carta.GetTrueLetter()!="*") {
            Debug.Log("La carta no es un comodin");
            return;
        }

        comodin = carta;
        panel.SetActive(true);
    }

    public void CerrarPanel () {
        comodin = null;
        panel.SetActive(false);
    }

    void SeleccionarBoton (Carta carta, Boton boton) {
        if (comodin==null) {
            Debug.Log("No hay comodin seleccionado");
            return;
        }

        comodin.SetVisualLetterForComodin(carta.GetTrueLetter());
        comodin = null;
        boton.contorno.SetActive(false);
        panel.SetActive(false);
    }

}
