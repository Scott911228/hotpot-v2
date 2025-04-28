using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsText : MonoBehaviour
{
    public Text TextElement;
    public static string Content = "將小高拖曳至豆皮上吧。";

    public static TipsText Instance; // Singleton
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保證在場景切換時不會銷毀
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
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
