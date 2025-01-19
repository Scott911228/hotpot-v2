using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTextScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Text text;
    private int a = 0;

    static string Tabs(int n)
    {
        return new string('.', n);
    }

    void Start()
    {
        InvokeRepeating("OutputTime", 0.5f, 0.5f);  //0.5s delay, repeat every 0.5s
    }

    void OutputTime()
    {
        a++;
        text.text = "NOW LOADING" + Tabs(a%3+1);
    }
}
