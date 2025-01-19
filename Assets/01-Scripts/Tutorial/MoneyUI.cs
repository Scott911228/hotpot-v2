using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class MoneyUI : MonoBehaviour
{
    public Text MoneyText;
    void Update()
    {
        MoneyText.text = Math.Floor(PlayerStats.Money).ToString();
    }
}
