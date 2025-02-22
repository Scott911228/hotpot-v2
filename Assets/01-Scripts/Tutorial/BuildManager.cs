using Unity.Mathematics;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    SpeedControl speedControl;
    public string DraggingCharacter = null;
    public bool isBuilding = false;
    public TurretBlueprint character1;
    public TurretBlueprint character2;
    public TurretBlueprint character3;
    public TurretBlueprint character4;
    public TurretBlueprint character5;
    private FloatTips FloatTipsScript;
    public Camera mainCamera;
    public static BuildManager instance;
    public int builtCount = 0;
    public static bool WaitForFullBuild = false;
    public TurretBlueprint turretToBuild;
    public bool CanBuild { get { return turretToBuild != null; } }
    public bool HasMoney { get { return PlayerStats.Money >= turretToBuild?.cost; } }


    void Start()
    {
        speedControl = GameObject.Find("SpeedControl").GetComponent<SpeedControl>();
        FloatTipsScript = GameObject.Find("FloatTips").transform.Find("FloatTipsBase").GetComponent<FloatTips>();
    }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in Scene");
            return;
        }
        instance = this;
    }
    public void BuildTurretOn(Node node, Quaternion rotation, TurretBlueprint turretBlueprint)
    {
        turretToBuild = turretBlueprint;
        isBuilding = false;
        GameManager.isBuilding = false;
        GameObject buildingPrefab = turretToBuild.prefab;
        if (CoolDown.isBuildable)
        {
            if (buildingPrefab?.GetComponent<Character>().characterType != node.tag)
            {
                FloatTipsScript.DisplayTips("無法放置在這裡！");
                speedControl.isForceSlowdown = false;
                return;
            }
            if (PlayerStats.Money < turretToBuild?.cost)
            {
                FloatTipsScript.DisplayTips("熱量不足！");
                speedControl.isForceSlowdown = false;
                return;
            }
            if (builtCount == 0 &&
                GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關")
            {
                if (node.name != "Cube (105)")
                {
                    FloatTipsScript.DisplayTips("無法放置在這裡！");
                    speedControl.isForceSlowdown = false;
                    return;
                }
            }
            if (builtCount < 2 &&
                GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關")
            {
                if(rotation.eulerAngles.y != 270){
                    FloatTipsScript.DisplayTips("嘗試使角色面向右側吧。");
                    speedControl.isForceSlowdown = false;
                    return;
                }
            }
            // 成功建造
            PlayerStats.Money -= turretToBuild.cost;
            GameObject turret = Instantiate(buildingPrefab, node.GetBuildPosition(), Quaternion.Euler(0, -90, 0));
            turret.GetComponent<Character>().firePoint.rotation = rotation;
            node.turret = turret;
            FloatTipsScript.DisplayTips("已派遣角色！剩餘熱量 " + math.floor(PlayerStats.Money).ToString());
            //turret.transform.Find("HealthBarCanvas").rotation = Quaternion.identity;

            //////// 關卡事件 ////////
            {
                if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第一關")
                {
                    if (builtCount == 0)
                    {
                        TextControl.BroadcastControlMessage("tutorial/text2");
                    }
                    if (WaitForFullBuild && PlayerStats.Money <= 50)
                    {
                        TextControl.BroadcastControlMessage("tutorial/text5");
                        WaitForFullBuild = false;
                    }
                }
                else if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關")
                {
                    if (builtCount == 0)
                    {
                        TextControl.BroadcastControlMessage("tutorial2/text2");
                    }
                    else if (builtCount == 1)
                    {
                        TextControl.BroadcastControlMessage("tutorial2/text4");
                    }
                }
            }
            ////////////////////////
            CoolDown.isBuildable = false;
            GameManager.isBuilding = false;
            speedControl.isForceSlowdown = false;
            // 只有在成功建造後才將 DraggingCharacter 設為 null
            DraggingCharacter = null;
            builtCount++;
        }
        else
        {
            FloatTipsScript.DisplayTips("冷卻時間尚未結束！");
            speedControl.isForceSlowdown = false;
        }
    }

    public void BuildByName(string turretName)
    {
        Debug.Log(turretName);
    }
    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
    }
}
