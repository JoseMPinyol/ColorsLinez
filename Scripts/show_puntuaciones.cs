using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class show_puntuaciones : MonoBehaviour {

    // Use this for initialization
    public main_logic mlogic;
    public handle_region_points[] handle_regions = new handle_region_points[2];
    public Text cpage;
    public Text mpage;

    private int curr_page = 1;

    public int min_page = 1;
    public int max_page = 7;
    public int num_by_page = 8;
    int who_page = 0;

	void Start () {
        mlogic = (GameObject.Find("LOGIC_CONFIG")).GetComponent<main_logic>();
    }

    bool first_time = true;
    bool can_drag = true;
    float deltax  = 0;
    void Update()
    {
        if (first_time)
        {
            update_idx(1, first_time);
            first_time = false;
        }
        if (can_drag && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            deltax += Input.GetTouch(0).deltaPosition.x;            
            if(deltax >= Screen.width/5 || deltax <= Screen.width / 5)
            {
                if (deltax > 0)
                    prev_best();
                else
                    next_best();
                can_drag = false;
                deltax = 0;
            }

        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            can_drag = true;
    }

    public void restart()
    {
        curr_page = 1;
        first_time = true;
        for (int i = 0; i < 2; i++)
            handle_regions[i].Restart();

        who_page = 1 - who_page; //garantizo utilizar la última nuevamente.
    }

    public void next_best()
    {
        if (curr_page == max_page)
            return;
        curr_page++;
        update_idx(1);
    }
	
    public void prev_best()
    {
        if (curr_page == min_page)
            return;
        curr_page--;
        update_idx(-1);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="start">start está 0-indexed</param>
    private void update_idx(int dir = 1 , bool first_time = false)
    {
        who_page = 1 - who_page;
        mpage.text = max_page.ToString();
        cpage.text = curr_page.ToString();
        int start = (curr_page - 1) * 10;
        for (int i = 0; i < 10; i++)
        {
            int pos = start + i + 1;
            int points;
            string nombre = "";
            if((int)mlogic.g_puntos.Count   > i + start)
            {
                points = (int)mlogic.g_puntos[i + start];
                nombre = (string)mlogic.g_names[i + start];
            }
            else
            {
                points = 0; nombre = "None";
            }
            //Debug.Log(handle_regions[who_page].entrys[i]);
            handle_regions[who_page].entrys[i].SET_VALUES(nombre, pos, points);
        }
        handle_regions[who_page].PlayEntrada(dir);
        if(first_time == false) handle_regions[1-who_page].PlaySalida(dir);

    }

}
