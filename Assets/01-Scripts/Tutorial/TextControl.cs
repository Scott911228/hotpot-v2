using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TextControl : MonoBehaviour
{
    public static void BroadcastControlMessage(string message)
    {
        Fungus.Flowchart.BroadcastFungusMessage(message);
    }
    public static void TutorialText_3()
    {
        Fungus.Flowchart.BroadcastFungusMessage("tutorial/text3");
        TipsText.Instance.ChangeText("將所有金錢用來部屬小高吧。");
    }
}
