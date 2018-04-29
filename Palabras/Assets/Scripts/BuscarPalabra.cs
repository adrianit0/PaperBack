using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System;

public class BuscarPalabra {

    const string path = "Assets/Nuevo.txt";
    const string newPath = "Assets/Nuevo2.txt";
    const string root = "Assets/";

    Dictionary<string, int> indice;
    string[] lineas;

    //Otro contenido
    Busqueda script;

    public BuscarPalabra(Busqueda script) {
        indice = new Dictionary<string, int>();
        lineas = GetLines();
        CargarIndice();

        this.script = script;

        getSolution();
    }

    /// <summary>
    /// Metodo 1
    /// </summary>
    void LeerConReader() {
        StreamReader reader = new StreamReader(path);
        string palabra = reader.ReadToEnd();
        reader.Close();
    }

    void LeerDirectamente() {
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);
    }

    public int EncontrarPalabra(string palabra) {
        if(palabra.Length <= 1) {
            return -1;
        }
        palabra = palabra.ToLower();

        for(int i = 0; i < palabra.Length; i++) {
            if(palabra[i].Equals('*')) {
                return EncontrarPalabraComodin(palabra);
            }
        }

        string substring = palabra.Substring(0, 2);

        int inicio = 0;
        if(!indice.TryGetValue(substring, out inicio))
            return -1;

        if(inicio == -1)
            return -1;

        int fin = (substring != "zu" ? GetNextIndex(substring) : lineas.Length);

        for(int i = inicio; i < fin; i++) {
            if(palabra.Equals(lineas[i]))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Busca una palabra con algun comodin
    /// </summary>
    int EncontrarPalabraComodin(string palabra) {
        int cantidad = 0;
        int hasIndex = 0;
        for(int i = 0; i < palabra.Length; i++) {
            if(palabra[i] == '*') {
                cantidad++;
                if(i < 2)
                    hasIndex++;
            }
        }

        if(cantidad == palabra.Length) {
            //ENCUENTRA UNA PALABRA CUALQUIERA ALEATORIA DEL DICCIONARIO DE LA MISMA CANTIDAD DE LETRA
            return EncontrarPalabraComodin(cantidad);
        }

        if(hasIndex > 0) {
            return EncontrarPalabraComodinInicio(palabra, hasIndex);
        }

        int index = 0;
        while(palabra.IndexOf('*') > -1) {
            palabra = ReplaceFirst(palabra, "*", index.ToString());
            index++;
        }

        //Si tiene mas de 10 comodines (Mayor que 9) se saldría para evitar errores (Ademas no deberia poderse usar mas de 10)...
        if(index > 10)
            return -3;

        //LO MISMO QUE ANTES
        string substring = palabra.Substring(0, 2);

        int inicio = 0;
        if(!indice.TryGetValue(substring, out inicio))
            return -1;

        if(inicio < 0)
            return -1;

        int fin = (substring != "zu" ? GetNextIndex(substring) : lineas.Length);

        string firstSubstring = palabra.Substring(0, palabra.IndexOf("0"));
        string lastSubstring = palabra.Substring(palabra.IndexOf((index - 1).ToString()) + 1);

        for(int i = inicio; i < fin; i++) {
            //Primer requisito para buscar la palabra es tener la misma longitud
            //Segundo requisito es tener el mismo inicio del substring  (xxx*def*ghi)
            //Tercer requisito es tener el mismo final del substring    (abc*def*xxx)
            //Cuarto y ultimo, tener todos los del medio                (abc*xxx*ghi)
            if(palabra.Length == lineas[i].Length && firstSubstring.Equals(lineas[i].Substring(0, firstSubstring.Length)) && lastSubstring.Equals(lineas[i].Substring(palabra.Length - lastSubstring.Length))) {
                //Debug("HA LLEGADO HASTA AQUI");
                bool encontrado = true;
                for(int x = 0; x < index - 1; x++) {
                    int _primero = palabra.IndexOf(x.ToString());
                    int _segundo = palabra.IndexOf((x + 1).ToString());
                    string _busqueda = palabra.Substring(_primero + 1, _segundo - _primero - 1);
                    string _comparar = lineas[i].Substring(_primero + 1, _segundo - _primero - 1);

                    //Debug("BUSQUEDA: " + _busqueda + " COMPARAR: " + _comparar + " REAL: "+lineas[i]);

                    if(!_busqueda.Equals(_comparar)) {
                        encontrado = false;
                        break;
                    }
                }

                if(encontrado)
                    return i;
            }
        }

        return -1;
    }

    int EncontrarPalabraComodinInicio(string palabra, int inicio) {
        if(inicio == 0 || inicio > 2)
            return -1;
        List<string> indices = (inicio == 1) ? DevolverIndiceSimple() : DevolverIndice();

        if(inicio == 1) {
            for(int i = 0; i < indices.Count; i++) {
                string _palabra = ReplaceFirst(palabra, "*", indices[i]);
                int valor = EncontrarPalabra(_palabra);
                if(valor > -1)
                    return valor;
            }
        } else {
            palabra = palabra.Substring(2);
            for(int i = 0; i < indices.Count; i++) {
                int valor = EncontrarPalabra(indices[i] + palabra);
                if(valor > -1)
                    return valor;
            }
        }

        return -1;
    }

    /// <summary>
    /// Busca una palabra con todo comodines
    /// Tiene que ser menor que 10
    /// </summary>
    int EncontrarPalabraComodin(int comodines) {
        if(comodines > 10)
            return -3;

        System.Random aleatorio = new System.Random();
        int inicio = aleatorio.Next(1, (int) (lineas.Length * 0.75f));
        int final = lineas.Length;

        for(int i = inicio; i < final; i++) {
            if(lineas[i].Length == comodines) {
                //Debug("PALABRA " + lineas[i] + " encontrada.");
                return i;
            }
        }

        return -1;
    }

    public string GetLine(int line) {
        return lineas[line];
    }

    int GetNextIndex(string index) {
        if(index.Length != 2)
            return -1;

        bool esEste = false;
        foreach(KeyValuePair<string, int> entry in indice) {
            if(esEste && entry.Value != -1) {
                return entry.Value;
            } else if(index == entry.Key) {
                esEste = true;
            }
        }

        return -1;
    }

    //QUITA TODAS LAS PALABRAS CON UN SOLO VALOR
    void QuitarPalabrasSueltas() {
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);
        List<string> palabras = new List<string>(lines.Length);

        for(int i = 0; i < lines.Length; i++) {
            if(lines[i].Length > 1) {
                palabras.Add(lines[i]);
            }
        }

        //Debug.Log("lines: " + lines.Length + " - nuevaLista: " + palabras.Count);
        CrearArchivo(palabras.ToArray(), "ordenado");
    }

    void OrdenarPalabras() {
        List<string> indices = DevolverIndice();
        List<int> valorIndice = new List<int>(indices.Count);
        //Debug.Log("Indice: " + indices.Count);

        string[] palabras = GetLines();
        List<string> listaOrdenada = new List<string>(palabras.Length);

        int index = 0;
        for(int i = 0; i < palabras.Length; i++) {
            bool encontrado = false;
            if(palabras[i].Substring(0, 2) != indices[index]) {
                string inicio = palabras[i].Substring(0, 2);
                string debug = i + " - " + palabras[i] + " (" + indices[index] + " - " + inicio + ")";
                for(int j = 0; j < index; j++) {
                    if(indices[j] == inicio) {
                        encontrado = true;
                        listaOrdenada.Insert(valorIndice[j], palabras[i]);
                        debug += " >> " + listaOrdenada[valorIndice[j] - 1] + " >> " + listaOrdenada[valorIndice[j]] + " << " + listaOrdenada[valorIndice[j] + 1];
                        for(int otroFOR = j; otroFOR < valorIndice.Count; otroFOR++) {
                            valorIndice[otroFOR]++;
                        }
                        break;
                    }
                }
                if(!encontrado) {
                    valorIndice.Add(System.Math.Max(i - 1, 0));
                    index++;
                }
                //Debug.Log(debug);
            }

            if(!encontrado)
                listaOrdenada.Add(palabras[i]);
        }

        CrearArchivo(listaOrdenada.ToArray(), "ordenado");
    }

    //COMPRUEBA EL ORDEN DE LAS PALABRAS
    void ComprobarOrden() {
        List<string> indices = DevolverIndice();

        string[] palabras = GetLines();
        int p = 0;
        for(int i = 0; i < palabras.Length; i++) {
            if(palabras[i].Substring(0, 2) != indices[p]) {
                string inicio = palabras[i].Substring(0, 2);
                for(int j = 0; j < p; j++) {
                    if(indices[j] == inicio) {
                        //Debug.Log("ERROR: La palabra " + palabras[i] + " del index " +indices[p]+ " está en un index inferior");
                        return;
                    }
                }
                p++;
            }
        }

        // Debug.Log("La lista no tiene ni un solo error de posición");
    }

    void ComprobarOrden2() {
        string[] palabras = GetLines();
        int quantity = palabras.Length - 1;

        for(int i = 0; i < quantity; i++) {
            if(palabras[i].Substring(0, 2).CompareTo(palabras[i + 1].Substring(0, 2)) > 0) {
                //Debug.Log("ERROR: Palabra " + palabras[i] + " es superior a " + palabras[i + 1] + " en la linea " + i);
            }
        }
    }

    /// <summary>
    /// Carga el índice para poder ser usado a posteriori
    /// </summary>
    void CargarIndice() {
        int i = 0;
        int oldI = 0;

        List<string> neoIndice = DevolverIndice();

        for(int x = 0; x < neoIndice.Count; x++) {
            oldI = i;
            for(; i < lineas.Length; i++) {
                if(neoIndice[x].CompareTo(lineas[i].Substring(0, 2)) < 0) {
                    indice.Add(neoIndice[x], -1);
                    break;
                }
                if(neoIndice[x] == lineas[i].Substring(0, 2)) {
                    indice.Add(neoIndice[x], i);
                    break;
                }
            }

        }

        //Debug.Log("Completado con exito");
    }

    string[] GetLines() {
        return File.ReadAllLines(path, Encoding.UTF8);
    }

    void CrearNuevoArchivo() {
        string[] lineas = GetLines();
        string[] newLineas = new string[lineas.Length];



        //Abro el escritor de palabras
        StreamWriter writer = new StreamWriter(newPath, true);
        for(int i = 0; i < lineas.Length; i++) {
            for(int x = 0; x < lineas[i].Length; x++) {
                lineas[i] = lineas[i].ToLower();
                if(lineas[i][x].Equals('á')) {
                    newLineas[i] += "a";
                } else if(lineas[i][x].Equals('é')) {
                    newLineas[i] += "e";
                } else if(lineas[i][x].Equals('í')) {
                    newLineas[i] += "i";
                } else if(lineas[i][x].Equals('ó')) {
                    newLineas[i] += "o";
                } else if(lineas[i][x].Equals('ú') || lineas[i][x].Equals('ü')) {
                    newLineas[i] += "u";
                } else {
                    newLineas[i] += lineas[i][x];
                }
            }

            writer.WriteLine(newLineas[i]);
        }
        writer.Close();

        //AssetDatabase.ImportAsset(newPath);
    }

    void CrearArchivo(string[] palabras, string nombreArchivo, string formato = "txt") {
        string camino = root + nombreArchivo + "." + formato;

        StreamWriter writer = new StreamWriter(camino, true);
        for(int i = 0; i < palabras.Length; i++) {
            writer.WriteLine(palabras[i]);
        }
        writer.Close();

        //AssetDatabase.ImportAsset (camino);
    }

    List<string> DevolverIndice() {
        List<string> indices = new List<string>(729);
        for(int x = 97; x <= 122; x++) {
            for(int y = 97; y <= 122; y++) {
                indices.Add(char.ConvertFromUtf32(x) + char.ConvertFromUtf32(y));
                if(y == 110)
                    indices.Add(char.ConvertFromUtf32(x) + "ñ");
            }
            if(x == 110) {
                for(int y = 97; y <= 122; y++) {
                    indices.Add("ñ" + char.ConvertFromUtf32(y));
                    if(y == 110)
                        indices.Add("ññ");
                }
            }
        }

        return indices;
    }

    public List<string> DevolverIndiceSimple() {
        List<string> indices = new List<string>(27);
        for(int x = 97; x <= 122; x++) {
            indices.Add(char.ConvertFromUtf32(x));
            if(x == 110)
                indices.Add("ñ");
        }

        return indices;
    }

    string ReplaceFirst(string text, string search, string replace) {
        int pos = text.IndexOf(search);
        if(pos < 0) {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    void Debug(object message) {
        if(script != null)
            script.Debug(message);
    }

    void getSolution() {
        extraer();
    }

    void extraer() {

    }
}
    