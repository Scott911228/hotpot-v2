
using System.Collections.Generic;
using System.Linq;
using Fungus;
using Unity.Mathematics;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    SpeedControl speedControl;
    public int DraggingCharacterIndex;
    public bool isBuilding = false;
    private CharacterSet[] characterSets;
    private FloatTips FloatTipsScript;
    public Camera mainCamera;
    public static BuildManager instance;
    public int builtCount = 0;
    public static bool WaitForFullBuild = false;
    public TurretBlueprint turretToBuild;
    private CharacterDispatchLimit[] characterDispatchLimits;
    public List<GameObject> activeCharacters = new List<GameObject>(); // 追蹤當前角色
    
    private PlayerStats playerStats;
    public bool CanBuild { get { return turretToBuild != null; } }
    public bool HasMoney { get { return playerStats.Money >= turretToBuild?.cost; } }

    public int GetCharacterCount(GameObject characterPrefab)
    {
        characterDispatchLimits = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().characterDispatchLimits;
        return activeCharacters.Count(c => c.name.StartsWith(characterPrefab.name));
    }
    public CharacterSet[] GetCharacterSet()
    {
        return characterSets;
    }
    // 獲取該角色的最大派遣數量
    public int GetCharacterLimit(GameObject characterPrefab)
    {
        if (characterDispatchLimits == null)
        {
            //Debug.LogError("GetCharacterLimit(): characterDispatchLimits 為 NULL，請檢查 BuildManager 是否正確初始化！");
            return -1;
        }

        if (characterPrefab == null)
        {
            //Debug.LogError("GetCharacterLimit(): characterPrefab 為 NULL！");
            return -1;
        }

        foreach (CharacterDispatchLimit dispatchLimit in characterDispatchLimits)
        {
            if (dispatchLimit == null)
            {
                //Debug.LogWarning("GetCharacterLimit(): dispatchLimit 為 NULL，跳過此條目");
                continue;
            }

            if (dispatchLimit.character == null)
            {
                //Debug.LogWarning("GetCharacterLimit(): dispatchLimit.character 為 NULL，跳過此條目");
                continue;
            }

            //Debug.Log($"GetCharacterLimit(): 檢查 {dispatchLimit.character.name} 是否匹配 {characterPrefab.name}");

            if (dispatchLimit.character == characterPrefab || dispatchLimit.character.name == characterPrefab.name)
            {
                //Debug.Log($"成功匹配角色 {characterPrefab.name}，上限為 {dispatchLimit.limit}");
                return dispatchLimit.limit;
            }
        }

        //Debug.LogWarning($"GetCharacterLimit(): 找不到角色 {characterPrefab.name}，回傳 -1");
        return -1; // -1 代表無限制
    }
    void Start()
    {
        DraggingCharacterIndex = -1;
        playerStats = GameObject.Find("GameControl").GetComponent<PlayerStats>();
        characterSets = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().characterSets;
        activeCharacters ??= new List<GameObject>();
        characterDispatchLimits = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().characterDispatchLimits;
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
    public void setBuiltCount(int count){
        builtCount = count;
    }
    public void BuildTurretOn(Node node, Quaternion rotation, TurretBlueprint turretBlueprint)
    {
        turretToBuild = turretBlueprint;
        isBuilding = false;
        GameManager.isBuilding = false;
        GameObject buildingPrefab = turretToBuild.prefab;
        CameraBrightnessController brightnessController = FindAnyObjectByType<CameraBrightnessController>();
        brightnessController.SetBrightness(0.0f);
        if (turretToBuild != null)
        {
            foreach (CharacterDispatchLimit dispatchLimit in characterDispatchLimits)
            {
                if (dispatchLimit.character == turretToBuild.prefab)
                {
                    int currentCount = activeCharacters.Count(c => c.name.StartsWith(dispatchLimit.character.name));
                    if (dispatchLimit.limit > 0 && currentCount >= dispatchLimit.limit)
                    {
                        FloatTipsScript.DisplayTips($"此角色最多只能派遣 {dispatchLimit.limit} 名！");
                        speedControl.isForceSlowdown = false;
                        return;
                    }
                }
            }
        }
        CoolDown assignedCoolDown = FindCooldownForCharacter(turretToBuild.prefab);
        if (assignedCoolDown != null && !assignedCoolDown.isBuildable)
        {
            FloatTipsScript.DisplayTips("冷卻時間尚未結束！");
            speedControl.isForceSlowdown = false;
            return;
        }
        else
        {
            if (buildingPrefab?.GetComponent<Character>().characterType != node.tag)
            {
                FloatTipsScript.DisplayTips("無法放置在這裡！");
                speedControl.isForceSlowdown = false;
                return;
            }
            if (playerStats.Money < turretToBuild?.cost)
            {
                FloatTipsScript.DisplayTips("熱量不足！");
                speedControl.isForceSlowdown = false;
                return;
            }
            if (builtCount == 0 &&
                GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關")
            {
                if (node.name != "Storycube1")
                {
                    FloatTipsScript.DisplayTips("無法放置在這裡！");
                    speedControl.isForceSlowdown = false;
                    return;
                }
            }
            if (builtCount < 2 &&
                GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關")
            {
                if (rotation.eulerAngles.y != 270)
                {
                    FloatTipsScript.DisplayTips("嘗試使角色面向右側吧。");
                    speedControl.isForceSlowdown = false;
                    return;
                }
            }
            // 成功建造
            playerStats.Money -= turretToBuild.cost;
            GameObject turret = Instantiate(buildingPrefab, node.GetBuildPosition(), Quaternion.Euler(20, -90, 0));
            turret.GetComponent<Character>().firePoint.rotation = rotation;
            node.turret = turret;
            FloatTipsScript.DisplayTips("已派遣角色！剩餘熱量 " + math.floor(playerStats.Money).ToString());
            //turret.transform.Find("HealthBarCanvas").rotation = Quaternion.identity;

            //////// 關卡事件 ////////
            {
                if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第一關")
                {
                    if (builtCount == 0)
                    {
                        TextControl.BroadcastControlMessage("tutorial/guidestart2");
                    }
                    if (WaitForFullBuild && playerStats.Money <= 50)
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
            ///
            GameManager.isBuilding = false;
            speedControl.isForceSlowdown = false;
            // 只有在成功建造後才將 DraggingCharacterIndex 設為 null
            DraggingCharacterIndex = -1;
            builtCount++;
            // **加入角色清單**
            activeCharacters.Add(turret);
            assignedCoolDown.StartCoolDown();
            // 更新派遣角色數量
            GameStats.Instance.RegisterCharacterDispatch(buildingPrefab);
            // 更新 UI
            UpdateAllCharacterCountUI();
        }
    }
    public void RemoveCharacterFromList(GameObject character)
    {
        if (activeCharacters.Contains(character))
        {
            activeCharacters.Remove(character);
            UpdateAllCharacterCountUI(); // 角色死亡時更新 UI
        }
    }
    public void UpdateAllCharacterCountUI()
    {
        CoolDown[] allCooldowns = FindObjectsByType<CoolDown>(FindObjectsSortMode.None);
        foreach (CoolDown cooldown in allCooldowns)
        {
            cooldown.UpdateCharacterCountUI();
        }
    }
    private CoolDown FindCooldownForCharacter(GameObject characterPrefab)
    {
        if (characterPrefab == null)
        {
            Debug.LogError("FindCooldownForCharacter() 被呼叫時 characterPrefab 為 NULL！");
            return null;
        }

        CoolDown[] allCooldowns = FindObjectsByType<CoolDown>(FindObjectsSortMode.None);
        foreach (CoolDown cooldown in allCooldowns)
        {
            if (cooldown.assignedCharacterPrefab != null && cooldown.assignedCharacterPrefab.prefab == characterPrefab)
            {
                return cooldown;
            }
        }

        Debug.LogWarning($"找不到對應的 CoolDown 物件，角色: {characterPrefab.name}");
        return null;
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
