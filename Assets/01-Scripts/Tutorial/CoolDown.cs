using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CoolDown : MonoBehaviour
{
    private float baseY = 14.11615f;
    public UnityEngine.UI.Image image;

    public GameObject UI;

    public float CoolDownTime = 2;

    public float timer = 0;

    public static bool isBuildable = true;
    private UnityEngine.UI.Image MaskImage;

    public KeyCode keyCode;
    // Start is called before the first frame update
    public void RemoveCoolDown()
    {
        CoolDownTime = 0;
    }

    void Start()
    {
        MaskImage = transform.Find("MaskImage").GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBuildable)
        {
            timer += Time.deltaTime;
            MaskImage.fillAmount = (CoolDownTime - timer) / CoolDownTime;
        }
        else
        {
            timer = 0;
            MaskImage.fillAmount = 0;
        }
        if (timer >= CoolDownTime)
        {
            isBuildable = true;
            MaskImage.fillAmount = 0;
            timer = 0;
        }
    }

    public void OnClick()
    {
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
    }
}
