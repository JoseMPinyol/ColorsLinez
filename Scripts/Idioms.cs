using UnityEngine;
using UnityEngine.UI;
using System;

public class Idioms : MonoBehaviour {

    public string[] language;
    public string[] traduction;

    

    // Use this for initialization
    void Awake()
    {
        SystemLanguage curr_idiom = Application.systemLanguage;
        if (curr_idiom != SystemLanguage.Spanish)
        {
            int idx = Array.FindIndex<string>(language, (string a) => { return (a == curr_idiom.ToString());  } );
            if(idx != -1)
                transform.GetComponent<Text>().text = traduction[idx];
        }
            
    }
}
