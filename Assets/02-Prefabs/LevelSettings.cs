using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [Header("關卡敘述")]
    // ======= 關卡名稱 ======= 
    public string StageName = "";
    [Header("開場數值")]
    // ======= 金錢 ======= 
    public double Money = 100;
    // ======= 基地生命 ======= 
    public int Life = 1;
    [Header("關卡敵人內容")]
    public Wave[] EnemyWaves;
}
