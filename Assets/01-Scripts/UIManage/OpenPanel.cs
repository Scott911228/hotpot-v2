using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public GameObject panel;
    void Start()
    {
        panel.SetActive(false);
    }
    public void SwitchPanelActive()
    {
        panel.SetActive(!(panel.active));
    }
}
