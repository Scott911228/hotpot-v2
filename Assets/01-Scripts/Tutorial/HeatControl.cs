using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatControl : MonoBehaviour
{
    private FloatTips FloatTipsScript;
    public int HeatLevel = 0;
    public Animator HeatButtonAnim;
    public Text HeatText;
    // Start is called before the first frame update

    public void ChangeHeat()
    {
        HeatLevel++;
        switch (HeatLevel % 3)
        {
            case 0:
                FloatTipsScript.DisplayTips("已切換火候至 小火");
                HeatButtonAnim.SetTrigger("small");
                HeatText.text = "小火";
                break;
            case 1:
                FloatTipsScript.DisplayTips("已切換火候至 中火");
                HeatButtonAnim.SetTrigger("medium");
                HeatText.text = "中火";
                break;
            case 2:
                FloatTipsScript.DisplayTips("已切換火候至 大火");
                HeatButtonAnim.SetTrigger("strong");
                HeatText.text = "大火";
                break;
            default:
                break;
        }
        Debug.Log(HeatLevel.ToString());
    }
    void Start()
    {
        FloatTipsScript = GameObject.Find("FloatTips").transform.Find("FloatTipsBase").GetComponent<FloatTips>();
        InvokeRepeating("AddHeatByLevel", 0f, 0.1f);
    }

    void AddHeatByLevel()
    {
        if(!GameObject.Find("GameControl").GetComponent<GameManager>().isGamePlaying) return;
        if(!GameObject.Find("GameControl").GetComponent<WaveSpawn>().hasEverSummoned) return;
        switch (HeatLevel % 3)
        {
            case 0:
                PlayerStats.Money += 0.5;
                break;
            case 1:
                break;
            case 2:
                if (PlayerStats.Money >= 0.5)
                {
                    PlayerStats.Money -= 0.5;
                }
                else
                {
                    FloatTipsScript.DisplayTips("熱量不足，已將火候設為小火。");
                    HeatButtonAnim.SetTrigger("small");
                    HeatText.text = "小火";
                    HeatLevel = 0;
                }
                break;
            default:
                break;
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}
