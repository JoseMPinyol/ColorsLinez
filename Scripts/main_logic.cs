using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Events;

public class main_logic : MonoBehaviour
{
    public GameObject Inicio;
    public GameObject go_to_inicio;
    public GameObject go_to_end;
    public GameObject main_panel;
    public GameObject credit_panel;
    public GameObject points_panel;

    Stack<GameObject> panels = new Stack<GameObject>();

    // Use this for initialization
    void Start()
    {
        load_or_create_file();
        close_current = false;
        Open_Panel(Inicio);
    }



    public ArrayList g_puntos = new ArrayList();
    public ArrayList g_names = new ArrayList();

    #region Saves_File
    private string giveme_path_file()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return Application.persistentDataPath + "/data.siv";
        }
        else
            return "data.siv";
    }

    public void save_results(int curr_rival, int points, string nombre)
    {
        g_puntos.Insert(curr_rival + 1, points);
        g_names.Insert(curr_rival + 1, nombre);
        FileStream fs = File.Open(giveme_path_file(), FileMode.Truncate);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write((System.Int32)g_names.Count);
        for (int i = 0; i < g_names.Count; i++)
        {
            bw.Write((string)g_names[i]);
            bw.Write((System.Int32)g_puntos[i]);
        }
        bw.Flush();
        fs.Close();
    }

    public void load_or_create_file()
    {
        FileStream fs;
        if (File.Exists(giveme_path_file()) == false)
        {
            fs = File.Create(giveme_path_file());
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write((System.Int32)1);
            bw.Write((string)"Pánfilo");
            bw.Write((System.Int32)4);
            bw.Flush();
            fs.Close();
        }
        fs = File.OpenRead(giveme_path_file());
        int N;
        try
        {
            BinaryReader br = new BinaryReader(fs);
            N = br.ReadInt32();
            for (int i = 0; i < N; i++)
            {
                g_names.Add(br.ReadString());
                g_puntos.Add(br.ReadInt32());
            }
            fs.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            fs.Close();
            File.Delete(giveme_path_file()); //si el formato no es válido lo borra
            load_or_create_file();
            return;
        }
    }
    #endregion

    public void Exit_Application()
    {
        credit_panel.SetActive(true);
        StartCoroutine(Exit_Application_Corutine());
    }

    private IEnumerator Exit_Application_Corutine()
    {
        yield return new WaitForSeconds(2.0f);
        Application.Quit();
    }

    public void Restart_Colors_Game()
    {
        SceneManager.LoadScene("colorlinez");
        //Application.LoadLevel("colorlinez");
    }

    int numscapes = 0;
    float time = -100;
    // Update is called once per frame
    void Update()
    {
        if(Time.time - time > 3.5f) { numscapes = 0; time = Time.time;}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            numscapes++;
            if(numscapes == 1)
            {
                if (Inicio.activeSelf == true)
                {
                    go_to_end.SetActive(true);
                    StartCoroutine(Dissable(go_to_end));
                }
                else if (main_panel.activeSelf == true)
                {
                    go_to_inicio.SetActive(true);
                    StartCoroutine(Dissable(go_to_inicio));
                }else 
                {
                    Close_Panel();
                    numscapes = 0;
                }
            }
            else if(numscapes == 2)
            {
                Close_Panel();
                go_to_end.SetActive(false);
                go_to_inicio.SetActive(false);
                numscapes = 0;
            }
        }
    }

    IEnumerator Dissable(GameObject curr)
    {
        yield return new WaitForSeconds(3.5f);
        curr.SetActive(false);
    }

    /// <summary>
    /// Todavía debo redactar esta función para usar una pila y si no hay nadie en la pila, cerrar el juego
    /// </summary>
    /// <param name="Panel_to_Close"></param>
    /// <param name="Panel_to_Open"></param>
    public void Close_Panel()
    {
        if (panels.Count  >= 1)
        {
            panels.Peek().SetActive(false);
            panels.Pop();
            if (panels.Count >= 1)
                panels.Peek().SetActive(true);
        }
        if(panels.Count == 0)
            Exit_Application();

    }

    public bool close_current = true;
    public float time_to_open = 0.0f;

    /// <summary>
    /// Debe setearse close_current a falso antes de llamar si no se quiere que se cierre este, por defecto se cerrará, y debe setearse el tiempo para abrirse antes, por defecto será 0
    /// </summary>
    /// <param name="Panel_to_open"></param>
    public void Open_Panel(GameObject Panel_to_open)
    {
        if(time_to_open > 0.01f)
        {
            StartCoroutine(Wait_Open_Panel(Panel_to_open, close_current, time_to_open));
        }
        else
        {
            if (close_current)
                panels.Peek().SetActive(false);
            Panel_to_open.SetActive(true);
            panels.Push(Panel_to_open);
        }
        close_current = true;
        time_to_open = 0.0f; 
    }

    private IEnumerator Wait_Open_Panel(GameObject Panel_to_open, bool close_current = true, float time_to_open = 0.0f)
    {
        yield return new WaitForSeconds(time_to_open);
        Open_Panel(Panel_to_open);
    }

}
