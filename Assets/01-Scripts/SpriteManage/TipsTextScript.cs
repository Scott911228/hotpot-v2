using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsTextScript : MonoBehaviour
{
    public Text text;
    private int a = 0;
    private string[] tips = new string[] {
        "沒有路！", "#黑特芋頭", "這裡沒有實用的提示", "hello world", "我要放假", "我不要做畢專我不要做畢專"
    };

    // Start is called before the first frame update
    void Start()
    {
        text.text = tips[a % tips.Length];
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void ChangeText(Text text)
    {
        a++;
        text.text = tips[a%tips.Length];
    }
}
