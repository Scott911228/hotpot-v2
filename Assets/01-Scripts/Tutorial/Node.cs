using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Node : MonoBehaviour
{
    SpeedControl speedControl;
    private FloatTips FloatTipsScript;
    public Color hovercolor;
    public Vector3 positionOffset;

    [Header("Optional")]
    public GameObject turret;
    public Renderer rend;
    private Color startcolor;
    TurretBlueprint turretBlueprint;
    BuildManager buildmanager;
    public Quaternion turretRotation = Quaternion.identity;

    void Start()
    {
        speedControl = GameObject.Find("SpeedControl").GetComponent<SpeedControl>();
        FloatTipsScript = GameObject.Find("FloatTips").transform.Find("FloatTipsBase").GetComponent<FloatTips>();
        rend = GetComponent<Renderer>();
        startcolor = rend.material.color;
        buildmanager = BuildManager.instance;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

    IEnumerator SelectTurretRotation(Node node)
    {
        FloatTipsScript.DisplayTips("移動游標設定效果範圍，右鍵確認");
        GameObject Guide = GameObject.Find("Guide");
        if (Guide != null) Guide.SetActive(false);

        Node targetNode = node;
        BuildManager buildManager = GameObject.Find("GameControl").GetComponent<BuildManager>();
        turretBlueprint = GetTurretBlueprintDragging();
        buildManager.isBuilding = true;

        GameObject turret = Instantiate(turretBlueprint.prefab, targetNode.GetBuildPosition(), Quaternion.identity);
        CanvasGroup canvasGroup = turret.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f; // 隱藏血條（暫時）

        turret.GetComponent<Character>().isPaused = true;
        turret.GetComponent<Character>().tag = "Untagged";

        bool isRotating = true;
        Character character = turret.GetComponent<Character>();
        character.showAttackRange = true; // 顯示攻擊範圍
        Vector3 AttackRotation = new Vector3(0, -90, 0);
        Vector3 ForceCharacterRotation = new Vector3(0, -90, 0); // 配置時角色始終向右
        InteractCharacterModeEnter();
        while (isRotating)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane p = new Plane(Vector3.up, turret.transform.position);
            if (p.Raycast(mouseRay, out float hitDist))
            {
                Vector3 hitPoint = mouseRay.GetPoint(hitDist);
                turret.transform.LookAt(hitPoint);
                turret.transform.eulerAngles = new Vector3(0, Mathf.Round((turret.transform.eulerAngles.y - 90) / 90) * 90, 0);
                // 更新攻擊範圍顯示
                AttackRotation = DisplayAttackRange(character);
                turret.transform.eulerAngles = ForceCharacterRotation;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Node NewNode = GetPositionNode(Input.mousePosition);
                if (NewNode)
                {
                    Character draggingCharacter = turretBlueprint.prefab?.GetComponent<Character>();
                    if (NewNode.turret != null)
                    {
                        FloatTipsScript.DisplayTips("無法放置在這裡！");
                    }
                    else if (draggingCharacter.characterType != NewNode.tag)
                    {
                        FloatTipsScript.DisplayTips("此角色無法放置在這裡！");
                    }
                    else
                    {
                        targetNode = NewNode;
                        turret.transform.position = NewNode.GetBuildPosition();
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) // 確認方向
            {
                turretRotation = Quaternion.Euler(0, AttackRotation.y, 0);
                isRotating = false;
            }
            yield return null;
        }
        turret.GetComponent<Character>().isPaused = false;
        turret.GetComponent<Character>().tag = "Character";
        Debug.Log("角色成功放置");
        character.showAttackRange = false;
        Destroy(turret);
        InteractCharacterModeExit();
        buildmanager.BuildTurretOn(targetNode, turretRotation, turretBlueprint);
    }


    private Vector3 DisplayAttackRange(Character character)
    {
        // 更新攻擊範圍
        Transform EffectDisplay = character.transform.Find("EffectRange");
        EffectDisplay.gameObject.SetActive(true);
        //Vector3 colliderStartPos = character.firePoint.position;
        Vector3 effectDisplayStartPos = character.firePoint.position;
        switch (character.transform.eulerAngles.y)
        {
            case 0.0f:
                //colliderStartPos.x += character.attackLength;
                effectDisplayStartPos.z -= character.attackLength;
                break;
            case 90.0f:
                //colliderStartPos.z -= character.attackLength;
                effectDisplayStartPos.z += character.attackLength;
                break;
            case 180.0f:
                //colliderStartPos.x -= character.attackLength;
                effectDisplayStartPos.z -= character.attackLength;
                break;
            case 270.0f:
                //colliderStartPos.z += character.attackLength;
                effectDisplayStartPos.z += character.attackLength;
                break;
            default:
                break;
        }
        effectDisplayStartPos.y = 1.5f;
        EffectDisplay.transform.localRotation = character.firePoint.rotation;
        EffectDisplay.transform.position = effectDisplayStartPos;
        EffectDisplay.transform.localScale = new Vector3(character.attackLength * (2.5f / character.transform.localScale.x), 1f, character.attackWidth * (2.5f / character.transform.localScale.z));
        // 舊版範圍顯示
        /* 
        colliderStartPos.y -= 2.5f;
        Collider[] hitColliders = Physics.OverlapBox(
            colliderStartPos,
            new Vector3(character.attackWidth, character.attackHeight, character.attackLength),
            character.firePoint.rotation);
        */
        return character.transform.eulerAngles;
    }

    TurretBlueprint GetTurretBlueprintDragging()
    {
        BuildManager buildManager = GameObject.Find("GameControl").GetComponent<BuildManager>();
        switch (buildManager.DraggingCharacter)
        {
            case "DraggableCharacter1":
                return buildManager.character1;
            case "DraggableCharacter2":
                return buildManager.character2;
            case "DraggableCharacter3":
                return buildManager.character3;
            case "DraggableCharacter4":
                return buildManager.character4;
            case "DraggableCharacter5":
                return buildManager.character5;
            default:
                return null;
        }
    }


    void InteractCharacterModeEnter()
    {
        GameObject Interactable = GameObject.Find("GameControl/GameElement/Interactable");
        Interactable.GetComponent<CanvasGroup>().alpha = 0.4f;
        Interactable.GetComponent<CanvasGroup>().interactable = false;
    }

    void InteractCharacterModeExit()
    {
        GameObject Interactable = GameObject.Find("GameControl/GameElement/Interactable");
        Interactable.GetComponent<CanvasGroup>().alpha = 1.0f;
        Interactable.GetComponent<CanvasGroup>().interactable = true;
    }

    void ChooseNodeExit()
    {
        GameObject.Find("GameControl").GetComponent<BuildManager>().DraggingCharacter = null;
        speedControl.isForceSlowdown = false;
    }

    void OnMouseEnter()
    {
        turretBlueprint = GetTurretBlueprintDragging();
        if (turretBlueprint != null)
        {
            Character draggingCharacter = turretBlueprint.prefab?.GetComponent<Character>();
            if (tag == draggingCharacter?.characterType)
            {
                rend.material.color = Color.green; // 可以放置
            }
            else
            {
                rend.material.color = Color.red; // 不可放置
            }
        }
        else
        {
            Color color = Color.yellow;
            if (tag == "Untagged") color.a = 0.4f;
            rend.material.color = color; // 未在拖曳角色時的高光顯示
        }
        if (!buildmanager.CanBuild)
            return;
    }

    void OnMouseExit()
    {
        rend.material.color = startcolor;
    }

    void OnMouseUp()
    {
        BuildManager buildManager = GameObject.Find("GameControl")?.GetComponent<BuildManager>();
        TurretBlueprint turretBlueprint = GetTurretBlueprintDragging();
        bool isBuilding = buildManager.isBuilding;
        string buildingTurretname = buildManager?.DraggingCharacter;

        if (isBuilding)
        {
            Debug.Log("Now building. Cancelling build.");
            return;
        }
        if (turretBlueprint?.prefab == null)
        {
            Debug.Log("No turret blueprint found for " + buildingTurretname + ". Cancelling build.");
            return;
        }
        if (string.IsNullOrEmpty(turretBlueprint.prefab.name))
        {
            Debug.Log("No character is being dragged. Cancelling build.");
            return;
        }
        else
        {
            Debug.Log("DraggingCharacter: " + turretBlueprint.prefab.name);
        }

        Node node = GetPositionNode(Input.mousePosition);
        if (node == null)
        {
            Debug.Log("Failed to find target node. Cancelling build.");
            return;
        }

        if (node.turret != null)
        {
            FloatTipsScript.DisplayTips("無法放置在這裡！");
            ChooseNodeExit();
            return;
        }
        else if (PlayerStats.Money < turretBlueprint.cost)
        {
            FloatTipsScript.DisplayTips("熱量不足！");
            ChooseNodeExit();
            return;
        }
        else if (turretBlueprint.prefab.GetComponent<Character>().characterType != node.tag)
        {
            FloatTipsScript.DisplayTips("此角色無法放置在這裡！");
            ChooseNodeExit();
            return;
        }

        // 檢查是否可以跳過方向選擇
        Character character = turretBlueprint.prefab.GetComponent<Character>();
        if (character.canSkipDirectionSelection)
        {
            // 如果可以跳過方向選擇，直接放置角色
            ChangeNodeColor(node, Color.green); // 將該 Node 設為綠色
            buildManager.BuildTurretOn(node, Quaternion.identity, turretBlueprint);
            Debug.Log("角色已放置，跳過方向選擇");
        }
        else
        {
            // 否則，進行方向選擇
            ChangeNodeColor(node, Color.green); // 將該 Node 設為綠色
            StartCoroutine(SelectTurretRotation(node));
        }
    }


    Node GetPositionNode(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            return hit.transform.gameObject.GetComponent<Node>();
        }
        else return null;
    }

    public void ChangeNodeColor(Node node, Color color)
    {
        if (node.tag == "Untagged") color.a = 0.2f;
        node.rend.material.color = color; // 改變 Node 顏色
    }

    private void RestoreNodeColor(Node node)
    {
        node.rend.material.color = node.startcolor; // 恢復為初始顏色
    }
}
