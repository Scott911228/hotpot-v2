using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static double Money;
    public int StartMoney = 400;

    public static int Lives;
    public int StartLives = 20;

    public int Rounds;
    void Start()
    {
        Money = StartMoney;
        Lives = StartLives;

        Rounds = 0;
    }

}
