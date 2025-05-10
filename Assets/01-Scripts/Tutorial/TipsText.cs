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
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    public void ChangeText(string text)
    {
        TextElement.text = text;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
