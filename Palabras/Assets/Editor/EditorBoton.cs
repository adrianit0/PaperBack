using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Boton))]
[CanEditMultipleObjects()]
public class EditorBoton : Editor {

    MeshRenderer mesh;

    Boton boton;
    SerializedObject property;
    SerializedProperty myEvent;

    void CogerComponentes () {
        boton = (Boton)target;
        property = new SerializedObject((Boton)target);

        if(mesh != null)
            return;
        mesh = boton.GetComponent<MeshRenderer>();
    }

    void Awake () {
        CogerComponentes();
    }

    void OnEnable () {
        CogerComponentes();
    }

    public override void OnInspectorGUI() {
        property.Update();

        bool _int = EditorGUILayout.Toggle("Interactable: ", boton.interactable);
        if (_int != boton.interactable) {
            boton.interactable = _int;
            _int = boton.interactable;
        }
        if(!boton.interactable)
            EditorGUILayout.HelpBox("Si está desactivado no funcionará el botón", MessageType.Info);


        GUILayout.Space(5);
        
        Serializar("_mesh");
        if(boton._mesh == null) {
            EditorGUILayout.HelpBox("Sin mesh no se podrá cambiar las texturas", MessageType.Warning);
            boton.cambiarMaterial = false;
        }            

        GUILayout.Space(10);

        if (boton._mesh != null) {
            Serializar("cambiarMaterial");
            if (boton.cambiarMaterial) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Materiales", EditorStyles.boldLabel);
                GUILayout.Space(5);
                Serializar("imagenNormal", "imagenEncendido", "imagenPresionado", "imagenDesactivada");
                EditorGUILayout.EndVertical();
            }
        } else {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Materiales", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("Sin mesh no se puede cambiar el material según su estado", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
        

        GUILayout.Space(10);

        boton.mostrarContorno = EditorGUILayout.Toggle("Mostrar contorno", boton.mostrarContorno);
        if (boton.mostrarContorno) {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Contorno: ", EditorStyles.boldLabel);
            GUILayout.Space(5);
            Serializar("contorno");
            if (boton.contorno == null) {
                EditorGUILayout.HelpBox("No tienes contorno, tendras que tener uno para hacerlo funcionar.\nPuedes ligarlo desde la jerarquía o crear uno.", MessageType.Error);
                if (GUILayout.Button ("Crear contorno")) {
                    boton.CrearContorno();
                }
            } else {
                Color _newColor = EditorGUILayout.ColorField("Color: ", boton.colorContorno);
                if (_newColor!= boton.colorContorno) {
                    boton.colorContorno = _newColor;
                    boton.CambiarColor(_newColor);
                }
            }
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(10);
        
        //boton.posicionPulsadoRelativo = EditorGUILayout.Vector3Field("Posición pulsado local:", boton.posicionPulsadoRelativo);
        Serializar("posicionPulsadoRelativo");

        GUILayout.Space(10);

        Serializar("pulsarBoton", "onPointerEnter", "onPointerExit");

        property.ApplyModifiedProperties();
    }

    void Serializar (string nombreVariable) {
        myEvent = property.FindProperty(nombreVariable);
        if (myEvent == null) {
            EditorGUILayout.HelpBox(string.Format("La variable {0} no existe", nombreVariable), MessageType.Warning);
            return;
        }
        EditorGUILayout.PropertyField(myEvent);
    }

    void Serializar (params string[] nombreVariables) {
        for (int i = 0; i < nombreVariables.Length; i++) {
            Serializar(nombreVariables[i]);
        }
    }
}
