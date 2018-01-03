using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: Poder enviar la carta comprada al descarta
public class ShopController : MonoBehaviour {

    GameManager manager;
    Tienda tienda;

    public GameObject panel;
    public Text textoDinero;
    public GameObject[] productos;
    Dictionary<Carta, int> cartasTienda;

    void Awake() {
        manager = GetComponent<GameManager>();
        tienda = new Tienda();
    }

    public void Inicializar() {
        RemoveContent();
        SetButtons();
        tienda.BarajarMazos();
        ActualizarTienda();
        AbrirTienda();
    }

    public void AddCarta (Carta carta) {
        if(carta.GetTipo() == LUGAR.Tienda)
            tienda.AddCartaLetra(carta);
        else if(carta.GetTipo() == LUGAR.Fama)
            tienda.AddCartaFama(carta);
        else
            Debug.Log("La carta "+carta.GetName()+" no ha sido añadida correctamente");
    }

    void SetButtons () {
        BotonSeleccionado(0, 2, 0);
        BotonSeleccionado(1, 3, 0);
        BotonSeleccionado(2, 3, 1);
        BotonSeleccionado(3, 4, 0);
        BotonSeleccionado(4, 4, 1);
        BotonSeleccionado(5, 5, 0);
        BotonSeleccionado(6, 5, 1);
        BotonSeleccionado(7, 6, 0);
        BotonSeleccionado(8, 6, 1);
        BotonSeleccionado(9, 7, 0);
        BotonSeleccionado(10, 7, 1);
        BotonSeleccionado(11, 8, 0);
        BotonSeleccionado(12, 8, 1);
    }

    void ActualizarTienda () {
        cartasTienda = new Dictionary<Carta, int>();
        MostrarProducto(0, 2, 0);
        MostrarProducto(1, 3, 0);
        MostrarProducto(2, 3, 1);
        MostrarProducto(3, 4, 0);
        MostrarProducto(4, 4, 1);
        MostrarProducto(5, 5, 0);
        MostrarProducto(6, 5, 1);
        MostrarProducto(7, 6, 0);
        MostrarProducto(8, 6, 1);
        MostrarProducto(9, 7, 0);
        MostrarProducto(10, 7, 1);
        MostrarProducto(11, 8, 0);
        MostrarProducto(12, 8, 1);
    }

    void MostrarProducto (int index, int precio, int posicion) {
        Carta _carta = tienda.GetCard(precio, posicion);
        if(_carta == null || productos[index] == null)
            return;

        manager.MoverCarta(_carta.gameObject, productos[index].transform.position);
        _carta.transform.localScale = new Vector3(0.6f, 0.6f, 1);
        _carta.canvas.sortingLayerName = "Tienda";
        _carta.canvas.sortingOrder = 1;

        cartasTienda.Add(_carta, precio);

        //Debug.Log("Se ha cambiado correctamente");
    }
    
    public void AbrirTienda() {
        int dinero = manager.GetPlayer().GetMoney();

        ActualizarDinero(dinero);
        ActualizarPrecio(dinero);
        panel.SetActive(true);
    }

    void ActualizarDinero(int dineroActual) {
        textoDinero.text = dineroActual + "€";
    }

    void ActualizarPrecio (int dinero) {
        foreach(KeyValuePair<Carta, int> valorTienda in cartasTienda) {
            valorTienda.Key.imagen.color = (valorTienda.Value > dinero) ? Color.gray : Color.white;
        }
    }

    void BotonSeleccionado (int index, int precio, int posicion) {
        GameObject _obj = productos[index];
        if(_obj == null)
            return;

        Boton _boton = _obj.GetComponent<Boton>();
        if(_boton == null)
            return;

        _boton.pulsarBoton.AddListener(() => SeleccionarProducto(index, precio, posicion));
    }

    void SeleccionarProducto (int index, int precio, int posicion) {
        Jugador _player = manager.GetPlayer();

        if(_player.SpendMoney(precio)) {
            //Le das la carta y lo envias al descarte

            ActualizarDinero(_player.GetMoney());
            ActualizarPrecio(_player.GetMoney());
            ActualizarTienda();
            return;
        }

        Debug.Log("No se ha podido realizar la compra por falta de capital");
    }

    void RemoveContent () {
        foreach (GameObject obj in productos) {
            Transform subObj = obj.transform.GetChild(0);
            if(subObj != null)
                Destroy(subObj.gameObject);
        }
    }
}
