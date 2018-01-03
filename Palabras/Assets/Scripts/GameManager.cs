using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public CartaInfo[] cartas;

    public GameObject prefab;

    public GameObject mazoRoboObj;
    public GameObject mazoDescObj;

    public int cantJugadores = 2;       //MAXIMO 5 JUGADORES
    int jugadorActual = 0;              //ROTATIVO, ENTRE LOS 5 JUGADORES
    Jugador[] jugadores;

    public ZonaJuego zonaMano;
    public ZonaJuego zonaJuego;
    public ZonaJuego zonaComun;

    public Boton enviarPalabra;

    Vector3 posGuardado = new Vector3(0, -9, -0.2f);

    Carta cartaSeleccionada;
    float posSeleccionado;
    ZonaJuego zonaSeleccionado;

    int layerMask;

    BuscarPalabra buscador;
    ShopController tienda;
    CartaComun cartaComun;
    ComodinManager comodinMan;

    void Awake() {
        buscador = new BuscarPalabra(null);
        tienda = GetComponent<ShopController>();
        cartaComun = GetComponent<CartaComun>();
        comodinMan = GetComponent<ComodinManager>();
    }

    void Start() {
        zonaMano.Configurar(this);
        zonaJuego.Configurar(this);
        zonaComun.Configurar(this);

        layerMask = zonaJuego.gameObject.layer;

        enviarPalabra.pulsarBoton.AddListener(() => EnviarPalabra());

        jugadores = new Jugador[cantJugadores];
        for (int i = 0; i < cantJugadores;i++) {
            jugadores[i] = new Jugador("Jugador " + i);
        }

        for (int i = 0; i < cartas.Length; i++) {
            int cantidad = cartas[i].copias;

            for (int x = 0; x < cartas[i].copias; x++) {
                LUGAR lugar = cartas[i].lugarInicial;
                int _copiasJugador = lugar== LUGAR.ManoInicial ? cantJugadores : 1;

                for (int y = 0; y < _copiasJugador; y++) {
                    Carta script = CrearCarta(i);

                    /*AÑADIR AQUI A DONDE TIENE QUE IR CADA CARTA. Las cartas a jugadores tiene que repetirse en cada jugador*/
                    switch(lugar) {
                        case LUGAR.ManoInicial:
                            jugadores[y].AddCarta(script, false);
                            break;
                        case LUGAR.Tienda:
                        case LUGAR.Fama:
                            tienda.AddCarta(script);
                            break;
                        case LUGAR.Comun:
                            cartaComun.AddCarta(script);
                            break;
                        
                    }
                   
                }
            }
        }

        tienda.Inicializar();
        cartaComun.SeleccionarUna();
        //CambiarTamañoMazos();
        for (int i=0; i < cantJugadores; i++) {
            jugadores[i].BarajarMazo();
        }
        StartCoroutine (RobarCarta(5));
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Input.GetMouseButtonDown (0)) {
            if(Physics.Raycast(ray, out hit, 500.0f) && hit.transform.gameObject.layer == layerMask) {
                ZonaJuego _script = hit.transform.gameObject.GetComponent<ZonaJuego>();
                _script.CogerCarta(hit.point.x);
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if(cartaSeleccionada!=null&&Physics.Raycast(ray, out hit, 500.0f)) {
                ZonaJuego _script = hit.transform.gameObject.GetComponent<ZonaJuego>();
                SoltarCarta(_script, hit.point.x);
            }
        }

        if (cartaSeleccionada!=null && Physics.Raycast(ray, out hit, 500.0f)) {
            cartaSeleccionada.transform.position = new Vector3(hit.point.x, hit.point.y, -0.4f);
            ZonaJuego _script = hit.transform.gameObject.GetComponent<ZonaJuego>();
            if (_script != null) {
                _script.MoverCartasAuto(hit.point.x);
            }
        }
    }

    public Carta CrearCarta (int idCarta) {
        GameObject obj = Instantiate(cartas[idCarta].prefab!=null ? cartas[idCarta].prefab : prefab);
        Carta script = obj.GetComponent<Carta>();
        obj.transform.position = posGuardado;

        script.ConfigurarCarta(cartas[idCarta]);
        return script;
    }

    void EnviarPalabra () {
        string palabra = zonaJuego.CrearPalabra();

        int index = buscador.EncontrarPalabra(palabra);
        if (index>=0) {
            int premio = zonaJuego.CalcularPremio();

            Debug.Log("La palabra " + palabra + "(" + buscador.GetLine(index) + ") existe. PREMIO GANADO: " + premio);
        } else {
            Debug.Log("La palabra " + palabra + " no existe");
        }
    }
    
    public void AddCartaMano (Carta carta, int jugador) {
        if(jugador < 0 || jugador >= jugadores.Length)
            return;

        jugadores[jugador].AddCartaMano(carta);
        zonaMano.AddCarta(carta);
    }

    public bool SeleccionarCarta (Carta carta, float pos, ZonaJuego zona) {
        if (cartaSeleccionada!=null) {
            Debug.Log("Ya hay una carta seleccionada.");
            return false;
        }
        cartaSeleccionada = carta;
        posSeleccionado = pos;
        zonaSeleccionado = zona;
        cartaSeleccionada.setTouch(true);
        cartaSeleccionada.SelectCarta(true);
        return true;
    }

    public void SoltarCarta (ZonaJuego nuevaZona, float nuevaPos) {
        if (cartaSeleccionada == null) {
            Debug.Log("Error: Carta no seleccionada.");
            return;
        }

        if(zonaSeleccionado != nuevaZona) {
            zonaSeleccionado.MoverCartasAuto();
        }

        if (nuevaZona==null) {
            nuevaZona = zonaSeleccionado;
            nuevaPos = posSeleccionado;
        }

        if(nuevaZona.soloComun && cartaSeleccionada.GetTipo() != LUGAR.Comun) {
            nuevaZona = zonaSeleccionado;
        }

        //SI ES UN COMODIN Y ENTRA EN LA ZONA JUEGO
        if (cartaSeleccionada.GetTrueLetter() == "*" && nuevaZona == zonaJuego) {
            comodinMan.SeleccionarComodin(cartaSeleccionada);
        } else {
            comodinMan.panel.SetActive(false);
        }

        cartaSeleccionada.setTouch(false);
        cartaSeleccionada.SelectCarta(false);
        nuevaZona.AddCarta(cartaSeleccionada, nuevaPos);
        zonaSeleccionado = null;
        cartaSeleccionada = null;
    }

    public void MoverCarta (GameObject obj, Vector3 position, bool forced = false) {
        StartCoroutine(_MoverCarta(obj, position, obj.transform.rotation.eulerAngles, forced));
    }

    public void MoverCarta(GameObject obj, Vector3 position, Vector3 rotation) {
        StartCoroutine(_MoverCarta(obj, position, rotation));
    }

    IEnumerator _MoverCarta(GameObject obj, Vector3 position, Vector3 rotation, bool forced = false) {
        Vector3 origen = obj.transform.position;
        Vector3 rotOrigen = obj.transform.rotation.eulerAngles;

        Carta _script = obj.GetComponent<Carta>();
        int ite = 0;
        if(_script != null) {
            _script.ite++;
            ite = _script.ite;
            if (!forced)
                _script.posX = position.x;
        }
           

        float lerp = 0;
        while(lerp < 1) {
            if(_script != null && ite != _script.ite)
                yield break;
            lerp += Time.deltaTime*5;
            obj.transform.position = Vector3.Lerp(origen, position, lerp);
            obj.transform.localRotation = Quaternion.Euler(Vector3.Lerp(rotOrigen, rotation, lerp));
            yield return null;
        }

        obj.transform.position = position;
        obj.transform.localRotation = Quaternion.Euler(rotation);
    }

    /*ESTO SIEMPRE SE HARA RESPECTO AL JUGADOR ACTUAL*/
    public IEnumerator RobarCarta (int cantidad) {
        int _actual = jugadorActual;
        Jugador _jugador = jugadores[_actual];

        for (int i = 0; i< cantidad; i++) {
            if (_jugador.GetDeckLength(true)==0) {
                StartCoroutine(MoverMazo());
            }

            Carta carta = _jugador.GetTopCardFromDrawDeck();
            if(carta == null) {
                Debug.Log("ERROR: El mazo de robo esta vacio");
                yield break;
            }
            
            carta.transform.position = new Vector3(mazoRoboObj.transform.position.x, mazoRoboObj.transform.position.y, mazoRoboObj.transform.position.z * 2);
            AddCartaMano(carta, _actual);
            //ORDENAR AQUI
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator MoverMazo() {
        int _actual = jugadorActual;
        Jugador _jugador = jugadores[_actual];
        _jugador.FillDrawDeck();

        Vector3 origen = mazoDescObj.transform.position;
        Vector3 destino = mazoRoboObj.transform.position;
        float lerp = 0;
        while (lerp < 1) {
            lerp += Time.deltaTime;
            mazoRoboObj.transform.position = Vector3.Lerp(origen, destino, lerp);
            yield return null;
        }

        CambiarTamañoMazos(_actual);
        _jugador.BarajarMazo();
    }

    public int GetPlayerID() {
        return jugadorActual;
    }

    public Jugador GetPlayer() {
        return jugadores[jugadorActual];
    }

    public List<string> DevolverIndiceSimple(bool incluirComodin) {
        if (!incluirComodin)
            return buscador.DevolverIndiceSimple();

        List<string> lista = buscador.DevolverIndiceSimple();
        lista.Add("*");
        return lista;
    }

    public void CambiarTamañoMazos (int jugador) {
        float cantidad = jugadores[jugador].GetDeckLength(true);
        mazoRoboObj.transform.localScale = new Vector3(1.5f, 2.1f, cantidad / 100);
        mazoRoboObj.transform.position = new Vector3(mazoRoboObj.transform.position.x, mazoRoboObj.transform.position.y, -cantidad / 200);
        mazoRoboObj.SetActive(cantidad > 0);
        
        cantidad = jugadores[jugador].GetDeckLength(false);
        mazoDescObj.transform.localScale = new Vector3(1.5f, 2.1f, cantidad / 100);
        mazoDescObj.transform.position = new Vector3(mazoDescObj.transform.position.x, mazoDescObj.transform.position.y, -cantidad / 200);
        mazoDescObj.SetActive(cantidad > 0);
    }
}