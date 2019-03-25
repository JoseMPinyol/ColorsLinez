using UnityEngine;
using System.Collections;

public class handle_cell : MonoBehaviour {

	// Use this for initialization
    public Vector2 pos;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void click_cell() {
        transform.parent.parent.GetComponent<game>().clicked(pos);
    }
}
