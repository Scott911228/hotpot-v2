using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LiveUI : MonoBehaviour
{
    public Text Livestext;
    
    void Update()
    {
        Livestext.text = PlayerStats.Lives.ToString() ;
    }
}
