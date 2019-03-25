using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Xml;


public class game : MonoBehaviour {

    public main_logic mlogic;

	// Use this for initialization
    //XML
    public GridLayoutGroup glay;
    public Transform[,] table;
    public Transform cell;
    public Sprite[] colors;
    public Image [] nexts;
    public Text puntos;
    public Text rival;
    public Text King_name;
    public Text myself;
    public Text the_king;
    private int[] next_idxs = new int[3];

    private int[,] matrix = new int[9, 9];


    private Vector2 last_pos;

    public InputField r_name;
    private string nombre;

    public void Click_Ok_Button_name(){
        nombre = r_name.text;
        if (nombre == "") return;
        myself.text = nombre;
        mlogic.Open_Panel(mlogic.main_panel);
    }


    int curr_rival;
    int points = 0;


    /// <summary>
    /// Si idx < 0 es su propio rival
    /// </summary>
    /// <param name="idx"></param>
    private void set_rival(int idx) {
        if (idx >= 0) {
            rival.text = (idx + 1).ToString() + "- " + (string)g_names[idx] + " " + (int)g_puntos[idx] + "p";

        }else
            rival.text = "";
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"> Es el index, 0-indexed de mi posición actual</param>
    private void set_myself(int idx) {
        puntos.text = points.ToString();
    }

    private void set_the_king()
    {
        King_name.text = (string)g_names[0];
        the_king.text = ((int)g_puntos[0]).ToString();
    }

    private ArrayList g_puntos;
    private ArrayList g_names;
    void Start() {
        //mlogic = (GameObject.Find("LOGIC_CONFIG")).GetComponent<main_logic>();
        g_puntos = mlogic.g_puntos;
        g_names = mlogic.g_names;

        curr_rival = g_puntos.Count - 1;
        set_rival(curr_rival);

        last_pos = new Vector2(-1, -1); 
        
        glay = GetComponent<GridLayoutGroup>();
       // glay.
        table = new Transform[9, 9];
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++) {
                Transform real_cell = Instantiate(cell);
                table[i, j] = real_cell.GetChild(1);
                real_cell.SetParent(this.transform,false);
                //table[i, j].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                //table[i, j].GetComponentInChildren<Text>().text = "";
                table[i, j].GetComponentInChildren<handle_cell>().pos = new Vector2(i, j);
            }
        Rect a = this.GetComponent<RectTransform>().rect;
        float siz = Mathf.Min(a.height, a.width) / 9.0f;
        glay.cellSize = new Vector2(siz, siz);
        Empezar_Juego();
        
    }

    public void Empezar_Juego() {
        Vector2 pos = new Vector2();
        for(int i = 0; i < 9;i++)
            for(int j = 0; j < 9; j++){
                pos.x = (float)i; pos.y = (float)j;
                set_pos_color(pos,0);
            }
        for (int i = 0; i < 5; i++) {
            pos = generate_valid_position();
            int colo = Random.Range(1, colors.Length);
            set_pos_color(pos, colo);
            search_points(pos);
        }
        set_the_king();
        points = 0;
        set_myself(-1);
        generate_nexts_colors();
        add_points(0);
    }

    private bool game_over() {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                if (matrix[i, j] == 0)
                    return false;
        return true;
    }


    IEnumerator add_nextsballs() {
        yield return new WaitForSeconds(0.5F);
        for (int i = 0; i < 3; i++) {
            if (game_over() == true) {
                mlogic.save_results(curr_rival,points,nombre);
                mlogic.Restart_Colors_Game();
                yield break;
            }
            Vector2 pos = generate_valid_position();
            set_pos_color(pos, next_idxs[i]);
            nexts[i].CrossFadeAlpha(0, 0, true);// = null;// colors[0];
            search_points(pos);
            yield return new WaitForSeconds(0.2F);
        }
        if (game_over() == true) {
            mlogic.save_results(curr_rival, points, nombre);
            mlogic.Restart_Colors_Game();
        }
        generate_nexts_colors();
    }

    private Image animating;
    public IEnumerator round_ball(Vector2 pos)
    {
        animating = table[(int)pos.x, (int)pos.y].parent.GetChild(0).GetComponent<Image>();
        Debug.Log(animating);
        float time_to_round = 2.0f;
        float total_pieces = 20;
        float curr_piece = total_pieces;
        int sign = -1;
        animating.fillMethod = Image.FillMethod.Radial360;
        while (true)
        {
            curr_piece += sign;
            animating.fillAmount = (curr_piece / total_pieces);
            if(curr_piece == 0 || curr_piece == total_pieces){
                sign = -sign;
                animating.fillClockwise = !animating.fillClockwise;
            }
            yield return new WaitForSeconds(time_to_round / total_pieces);

        }
    }

    void Stop_animation_round_ball(Vector2 pos)
    {
        StopCoroutine(last_round_animated);
        animating.fillAmount = 1;
    }
    Coroutine last_round_animated;
    public void clicked(Vector2 pos){ //pos que celda se le hizo click
       // Debug.Log(pos);
        if (last_pos.x == -1 && last_pos.y == -1) {
            if (matrix[(int)pos.x, (int)pos.y] == 0) {                
                return;
            }
            //Código de marcar
            last_round_animated = StartCoroutine(round_ball(pos));
            last_pos = pos;
            return;
        }
        Stop_animation_round_ball(last_pos);
        if (pos == last_pos) { //caso es el mismo
            //table[(int)pos.x,(int)pos.y].GetComponent<Button>(). desmarcar
          last_pos = new Vector2(-1, -1);
          return;
        }

        if (matrix[(int)pos.x, (int)pos.y] != 0) {
            last_pos = pos;
            last_round_animated = StartCoroutine(round_ball(pos));
            return; 
        }
        exists_walk_BFS(last_pos);
        if (dist[(int)pos.x, (int)pos.y]!= -1) { // si se puede llegar
            StartCoroutine(run_animation_porun_animation_pos_and_add_points(last_pos, pos));
        }
        else {
            last_pos = pos;
            return;
        }
        last_pos = new Vector2(-1, -1);
    }

    IEnumerator run_animation_porun_animation_pos_and_add_points(Vector2 last_pos, Vector2 pos)
    {
        ArrayList al = new ArrayList();
        Vector2 curr = pos;
        al.Add(curr);
        while(curr != last_pos)
        {
            int nx = parx[(int)curr.x, (int)curr.y], ny = pary[(int)curr.x, (int)curr.y];
            curr.x = nx; curr.y = ny;
            al.Add(curr);
        }
        for(int i = al.Count - 1;i > 0; i--)
        {
            yield return new WaitForSeconds(0.06F);
            last_pos = (Vector2)al[i];
            pos = (Vector2)al[i - 1];
            set_pos_color(pos, matrix[(int)last_pos.x, (int)last_pos.y]);
            set_pos_color(last_pos, 0);
        }
        int pt = search_points(pos);
        if (pt == 0)
            StartCoroutine(add_nextsballs());
        else
            add_points(pt);
    }

    void add_points(int pt) {
        points += pt;
        while (curr_rival >= 0 && (int)g_puntos[curr_rival] < points)  curr_rival--;
        set_rival(curr_rival);
        set_myself(curr_rival + 1);
        set_the_king();

    }

    void set_pos_color(Vector2 pos,int colo) {
        matrix[(int)pos.x, (int)pos.y] = colo;
        Image celda = table[(int)pos.x, (int)pos.y].GetComponent<Image>();
        celda.sprite = colors[colo];
        celda.enabled = false;
        celda.enabled = true;
    }

    IEnumerator dissappear_ball(Vector2 pos)
    {
        Transform celda = table[(int)pos.x, (int)pos.y];
        celda.GetComponent<Image>().CrossFadeAlpha(0, 0.15f, true);
        yield return new WaitForSeconds(0.2f);
        set_pos_color(pos, 0);
    }

    Vector2 generate_valid_position() {
        Vector2 ret;
        for (; ; ) {
            ret = new Vector2(Random.Range(0, 9), Random.Range(0, 9));
            if (matrix[(int)ret.x, (int)ret.y] == 0) break;
        }
        return ret;
    }

    void generate_nexts_colors() {
        for (int i = 0; i < 3; i++) {
            next_idxs[i] = Random.Range(1, colors.Length);
            nexts[i].CrossFadeAlpha(1, 0, true);
            nexts[i].sprite = colors[next_idxs[i]];

        }
    }

    private int[,] dist = new int[9, 9];
    private int[,] parx = new int[9, 9];
    private int[,] pary = new int[9, 9];
    int[] mvx = new int[]{ 0, 0, -1, 1 };
    int[] mvy =  new int[]{ -1, 1, 0, 0 };

    void exists_walk_BFS(Vector2 pos) {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                dist[i, j] = -1;
        dist[(int)pos.x, (int)pos.y] = 0;
        Queue Q = new Queue();
        Q.Enqueue(new Vector2(pos.x, pos.y));
        while(Q.Count != 0){
            pos = (Vector2)Q.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                int nx = (int)pos.x + mvx[i], ny = (int)pos.y + mvy[i];
                if (nx < 0 || ny < 0 || nx == 9 || ny == 9 || matrix[nx, ny] != 0 || dist[nx, ny] != -1)
                    continue;
                dist[nx, ny] = dist[(int)pos.x, (int)pos.y] + 1;
                parx[nx, ny] = (int)pos.x; pary[nx, ny] = (int)pos.y;
                Q.Enqueue(new Vector2(nx, ny));
            }
        }        
    }



    int[] mpx = new[] { 0, 0, 1, -1, 1, -1, -1, 1 };
    int[] mpy = new[] { -1, 1, 0, 0, 1, -1, 1, -1 };
    private int search_points(Vector2 pos) {
        int points = 0;
        int[] same = new int[8]; // la cantidad que tienen el mismo color en las distintas direcciones
        int x = (int)pos.x, y = (int)pos.y;
        for (int i = 0; i < 8; i++) {
            int j = 0;
            for (; ; ) {
                int nx = x + mpx[i] * (j + 1), ny = y + mpy[i] * (j + 1);
                if (nx < 0 || nx > 8 || ny < 0 || ny > 8) break;
                if (matrix[x, y] != matrix[nx, ny])
                    break;
                j++;
            }
            same[i] = j;
        }
        for (int i = 0; i < 4; i++) { //improve adding to a list and then kill all together.
            int l = 2 * i, r = 2 * i + 1;
            if (same[l] + same[r] >= 4) {
                points += 2 * (same[l] + same[r] + 1);
                for (int j = 0; j < same[l]; j++)
                    StartCoroutine(dissappear_ball(new Vector2(x + mpx[l] * (j + 1), y + mpy[l] * (j + 1))));
                for (int j = 0; j < same[r]; j++)
                    StartCoroutine(dissappear_ball(new Vector2(x + mpx[r] * (j + 1), y + mpy[r] * (j + 1))));
            }
        }
        if (points > 0)
            StartCoroutine(dissappear_ball(pos));
        return points;
    }
}
