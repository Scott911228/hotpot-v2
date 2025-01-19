using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsText : MonoBehaviour
{
    public Text TextElement;
    public static string Content = "將小高拖曳至豆皮上吧。";
    // Start is called before the first frame update
    public void ChangeText(string text)
    {
        TextElement.text = text;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
