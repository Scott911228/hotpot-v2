using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static double Money;

    public static int Life;

    public int Rounds;
    void Start()
    {
        Money = (double)GameObject.Find("LevelSettings")?.GetComponent<LevelSettings>().Money;
        Life = (int)GameObject.Find("LevelSettings")?.GetComponent<LevelSettings>().Life;

        Rounds = 0;
    }

}
