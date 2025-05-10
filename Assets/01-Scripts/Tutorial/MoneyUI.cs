using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class MoneyUI : MonoBehaviour
{
    private PlayerStats playerStats;
    public Text MoneyText;
    void Start()
    {
        playerStats = GameObject.Find("GameControl").GetComponent<PlayerStats>();
    }
    void Update()
    {
        MoneyText.text = Math.Floor(playerStats.Money).ToString();
    }
}
