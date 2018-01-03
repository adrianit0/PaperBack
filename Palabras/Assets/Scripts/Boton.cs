using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Boton : MonoBehaviour {

    bool _interactable = true;

    public bool interactable  {
        set {
            if(_mesh == null)
                GetComponent<MeshRenderer>();
            _interactable = value;
            _mesh.sharedMaterial = (value) ? imagenNormal : imagenDesactivada;
        }
        get { return _interactable; }
    }

    public MeshRenderer _mesh;

    public bool cambiarMaterial = true;
    public bool mostrarContorno = true;

    public Material imagenNormal;
    public Material imagenEncendido;
    public Material imagenPresionado;
    public Material imagenDesactivada;

    public Color colorContorno = new Color(1f, 1f, 0f, 1f);
    public GameObject contorno;
    
    string outlineColor = "_OutlineColor";
    string outlineWidht = "_Outline";

    Vector3 posicionInicial;
    Vector3 posicionPulsado;
    public Vector3 posicionPulsadoRelativo = Vector3.zero;

    public UnityEvent pulsarBoton;
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;

    bool presionando = false;

    void Awake () {
        if(onPointerEnter == null) onPointerEnter = new UnityEvent();
        if(onPointerExit == null) onPointerExit = new UnityEvent();
    }
    
    void Start () {
        posicionInicial = transform.localPosition;
        posicionPulsado = posicionInicial - posicionPulsadoRelativo;

        if(_mesh != null && cambiarMaterial)
            _mesh.material = (interactable) ? imagenNormal : imagenDesactivada;

        if (contorno!=null)
            contorno.SetActive(false);
        
    }

    public void CrearContorno() {
        if (_mesh==null) {
            _mesh = GetComponent<MeshRenderer>();
            if (_mesh==null) {
                Debug.LogError("Este GameObject no tiene MeshRenderer");
                return;
            }
        }
        
        GameObject outlineObj = new GameObject();
        outlineObj.transform.position = _mesh.transform.position;
        outlineObj.transform.rotation = _mesh.transform.rotation;
        outlineObj.transform.parent = _mesh.transform;
        outlineObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        outlineObj.name = "Contorno";
        outlineObj.AddComponent<MeshFilter>();
        outlineObj.AddComponent<MeshRenderer>();
        Mesh mesh;
        mesh = (Mesh)Instantiate(_mesh.GetComponent<MeshFilter>().sharedMesh);
        outlineObj.GetComponent<MeshFilter>().mesh = mesh;
        
        int materialsNum = _mesh.sharedMaterials.Length;
        Material[] materials = new Material[materialsNum];
        MeshRenderer _outlineMesh = outlineObj.GetComponent<MeshRenderer>();

        for(int i = 0; i < materialsNum; i++) {
            materials[i] = new Material(Shader.Find("Outline/Outline"));
            materials[i].SetColor(outlineColor, colorContorno);
            materials[i].SetFloat(outlineWidht, 0);
        }
        _outlineMesh.sharedMaterials = materials;
        _outlineMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _outlineMesh.receiveShadows = false;

        contorno = outlineObj;
        contorno.SetActive(false);
    }

    public void CambiarColor (Color newColor) {
        MeshRenderer _outlineMesh = contorno.GetComponent<MeshRenderer>();
        for (int i = 0; i < _mesh.sharedMaterials.Length; i++) {
            _outlineMesh.sharedMaterials[i].SetColor(outlineColor, newColor);
        }
    }

    void OnMouseDown() {
        if(interactable) {
            if(_mesh != null && cambiarMaterial)
                _mesh.material = imagenPresionado;
            if(posicionPulsadoRelativo != Vector3.zero)
                transform.localPosition = posicionPulsado;
            presionando = true;
        }
    }

    void OnMouseEnter() {
        if(interactable) {
            if(_mesh != null && cambiarMaterial)
                _mesh.material = (presionando) ? imagenPresionado : imagenEncendido;
            if(mostrarContorno && contorno != null)
                contorno.SetActive(true);
            if (posicionPulsadoRelativo != Vector3.zero)
                transform.localPosition = (presionando) ? posicionPulsado : posicionInicial;
            onPointerEnter.Invoke();
        }
    }

    void OnMouseExit() {
        if(interactable) {
            if(_mesh != null && cambiarMaterial)
                _mesh.material = imagenNormal;
            if (mostrarContorno && contorno != null)
                contorno.SetActive(false);
            onPointerExit.Invoke();
        }
    }

    void OnMouseUp() {
        if(interactable) {
            if(posicionPulsadoRelativo != Vector3.zero)
                transform.localPosition = posicionInicial;
            presionando = false;
        }
    }

    void OnMouseUpAsButton() {
        if(interactable) {
            if(_mesh != null && cambiarMaterial)
                _mesh.material = imagenEncendido;

            pulsarBoton.Invoke();
        }
    }
}
