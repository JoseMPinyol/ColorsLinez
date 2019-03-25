using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class handle_entry : MonoBehaviour {

    public Text nombre;
    public Text pos;
    public Text points;

    public void SET_VALUES(string name , int pos , int points)
    {
        nombre.text = name;
        this.pos.text = pos.ToString();
        this.points.text = points.ToString();
    }
}
