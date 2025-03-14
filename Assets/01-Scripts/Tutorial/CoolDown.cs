using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class CoolDown : MonoBehaviour
{
    private float baseY = 14.11615f;
    public UnityEngine.UI.Image image;

    public GameObject UI;

    public float CoolDownTime = 2;

    public float timer = 0;
    public bool isBuildable = true;
    private UnityEngine.UI.Image MaskImage;

    public KeyCode keyCode;
    // Start is called before the first frame update
    [Header("派遣限制")]
    public GameObject assignedCharacterPrefab; // 對應的角色
    public TMP_Text characterCountText; // UI 顯示派遣數量的 Text
    public void RemoveCoolDown()
    {
        CoolDownTime = 0;
    }

    void Start()
    {
        MaskImage = transform.Find("MaskImage").GetComponent<UnityEngine.UI.Image>();
        UpdateCharacterCountUI(); // 遊戲開始時更新一次
    }

    //更新角色數量 UI
    public void UpdateCharacterCountUI()
    {
        if (characterCountText == null || assignedCharacterPrefab == null) return;

        int currentCount = BuildManager.instance.GetCharacterCount(assignedCharacterPrefab);
        int maxLimit = BuildManager.instance.GetCharacterLimit(assignedCharacterPrefab);

        string limitText = (maxLimit >= 0) ? $"{maxLimit - currentCount}" : "∞"; // -1 代表無限制
        characterCountText.text = $"{limitText}";
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
