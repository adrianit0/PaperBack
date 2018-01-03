using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardModule : IModulo {

    int id;

    GameManager manager;
    MainEditor mainEditor;

    public CardModule(MainEditor mainEditor) {
        this.mainEditor = mainEditor;

        manager = (GameManager) GameObject.FindObjectOfType(typeof(GameManager));
    }

    public void OnList() {
        if (manager==null) {
            id = 0;
            return;
        }
        id = mainEditor.ListaBotones<CartaInfo>(ref manager.cartas, id);
    }

    public void OnBody() {
        if(manager == null) {
            EditorGUILayout.HelpBox("No hay manager en esta escena para hacer funcionar este modulo.", MessageType.Warning);
            return;
        }
        if(id >= manager.cartas.Length) {
            EditorGUILayout.HelpBox("El id es superior al valor real...", MessageType.Warning);
            return;
        }

        CartaInfo info = manager.cartas[id];
        
        info.letra = EditorGUILayout.TextField("Letra: ", info.letra);
        if(info.letra.Length>2) {
            info.letra = info.letra.Substring(0, 2);
        }

        info.precio = EditorGUILayout.IntSlider("Precio: ", info.precio, 0, 17);
        info.premio = EditorGUILayout.IntSlider("Premio: ", info.premio, 0, 17);
        info.fama = EditorGUILayout.IntSlider("Fama: ", info.fama, 0, 17);
        info.descripcion = EditorGUILayout.TextField("Habilidad: ", info.descripcion);
        info.copias = EditorGUILayout.IntField("Copias a crear: ", info.copias);
        if (info.copias==0) {
            EditorGUILayout.HelpBox("Si tiene 0 copias no se creara ninguna.", MessageType.Info);
        }
        info.lugarInicial = (LUGAR) EditorGUILayout.EnumPopup("Lugar de creacion inicial: ", info.lugarInicial);

        info.prefab = (GameObject) EditorGUILayout.ObjectField("Prefab", info.prefab, typeof(GameObject), true);
        if(info.prefab == null)
            EditorGUILayout.HelpBox("Si no tiene prefab no tendra efecto alguno.", MessageType.Info);
    }

    public string GetName(int i) {
        if (manager==null) {
            return "Vacio";
        }
        return ((manager.cartas[i].letra!="")?((manager.cartas[i].letra=="*")?"Comodin":manager.cartas[i].letra):"Sin letra...") + " ("+manager.cartas[i].precio+"€/"+manager.cartas[i].premio+"€/"+manager.cartas[i].fama+"★) x" + manager.cartas[i].copias;
    }
}
