using UnityEngine;
using System.Collections;

public class handle_region_points : MonoBehaviour {

    // Use this for initialization
    public handle_entry[] entrys;
    private Animator animator;
	void Start () {
        entrys = GetComponentsInChildren<handle_entry>();
        animator = GetComponent<Animator>();
	}

    public void PlayEntrada(int dir = 1)
    {
        if(dir == 1) animator.Play("EntradaPanelIz");
        else animator.Play("EntradaPanelDe"); ;
    }
    public void PlaySalida(int dir = 1)
    {
        if(dir==1) animator.Play("SalidaPanelDe");
        else
            animator.Play("SalidaPanelIz");
    }
    public void Restart() //mejorar esto para que no se quede en un estado intermedio.
    {
        animator.Play("EntradaPanelIz");
    }
}
