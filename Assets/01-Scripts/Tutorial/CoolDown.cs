using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using System;
using Fungus;

public class CoolDown : MonoBehaviour
{
    private float baseY = 14.11615f;
    public UnityEngine.UI.Image image;

    public GameObject UI;

    public float CoolDownTime = 2;
    public bool isBuildable = true;
    public float timer = 0;
    private UnityEngine.UI.Image MaskImage;
    public KeyCode keyCode;
    [Header("欄位編號 (0-based)")]
    public int slotIndex = 0;
    [Header("派遣限制")]
    private LevelSettings levelSettings;
    public TurretBlueprint assignedCharacterPrefab; // 對應的角色
    public TMP_Text characterCountText; // UI 顯示派遣數量的 Text
    public TMP_Text characterTypeText; // UI 顯示角色類型的 Text
    public void RemoveCoolDown()
    {
        CoolDownTime = 0;
    }

    void Start()
    {
        levelSettings = GameObject.Find("LevelSettings").GetComponent<LevelSettings>();
        if (slotIndex < levelSettings.characterSets.Length)
        {
            assignedCharacterPrefab = levelSettings.characterSets[slotIndex].turretBlueprint;
            transform.Find("DraggableCharacter").GetComponent<UnityEngine.UI.Image>().sprite = assignedCharacterPrefab.prefab.GetComponent<Character>().shopIcon;
            transform.Find("Price").GetComponent<TMP_Text>().text = assignedCharacterPrefab.cost.ToString();
        }
        else
        {
            gameObject.SetActive(false);
        }
        MaskImage = transform.Find("MaskImage").GetComponent<UnityEngine.UI.Image>();
        characterCountText = transform.Find("DispatchLimit").GetComponent<TMP_Text>();
        characterTypeText = transform.Find("DispatchType").GetComponent<TMP_Text>();
        UpdateCharacterCountUI(); // 遊戲開始時更新一次
        UpdateCharacterTypeUI(); // 遊戲開始時更新一次
    }
    public void UpdateCharacterCountUI()
    {
        if (characterCountText == null || assignedCharacterPrefab.prefab == null) return;
        int currentCount = BuildManager.instance.GetCharacterCount(assignedCharacterPrefab.prefab);
        int maxLimit = BuildManager.instance.GetCharacterLimit(assignedCharacterPrefab.prefab);
        string limitText = (maxLimit >= 0) ? $"{maxLimit - currentCount}" : "∞"; // -1 代表無限制
        characterCountText.text = $"{limitText}";
    }
    //更新角色數量 UI
    public void UpdateCharacterTypeUI()
    {
        if (characterTypeText == null || assignedCharacterPrefab.prefab == null) return;
        if (assignedCharacterPrefab.prefab.GetComponent<Character>().characterType == "Road") characterTypeText.text = "地面";
        else if (assignedCharacterPrefab.prefab.GetComponent<Character>().characterType == "Wall") characterTypeText.text = "高台";
    }
    // Update is called once per frame
    void Update()
    {
        if (!isBuildable)
        {
            timer += Time.deltaTime;
            MaskImage.fillAmount = Mathf.Clamp01((CoolDownTime - timer) / CoolDownTime);
            if (timer >= CoolDownTime)
            {
                isBuildable = true;
                MaskImage.fillAmount = 0;
                timer = 0;
            }
        }
        if(characterCountText.text == "0"){
            MaskImage.fillAmount = 1;
        }
        else if (PlayerStats.Money < assignedCharacterPrefab.cost)
        {
            MaskImage.fillAmount = 1;
        }
        else if (isBuildable)
        {
            MaskImage.fillAmount = 0;
        }
    }

    public void OnClick()
    {
        if (!isBuildable) return; // 若仍在冷卻中，則無法點擊

        GameManager.isBuilding = true;
        Time.timeScale = 0.2f;
        string turretToBuild = GameObject.Find("GameControl").GetComponent<BuildManager>().turretToBuild.prefab.name;

        GameObject[] Items = GameObject.FindGameObjectsWithTag("Shop");
        foreach (GameObject Item in Items)
        {
            Vector3 move = Item.transform.position;
            move = new Vector3(move.x, baseY + (turretToBuild == Item.name ? 20f : 0f), move.z);
            Item.transform.position = move;
        }

        // 角色派遣後，開始冷卻
    }
    public void StartCoolDown()
    {
        isBuildable = false;
        timer = 0;
        MaskImage.fillAmount = 1;
    }
}
