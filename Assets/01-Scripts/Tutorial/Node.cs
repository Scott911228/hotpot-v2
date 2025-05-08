using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using Fungus;

public class Node : MonoBehaviour, IMouseInteractable
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
    private int highlightLayer;
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();
    private RendererHighlightManager rendererHighlightManager;
    void Start()
    {
        speedControl = GameObject.Find("SpeedControl").GetComponent<SpeedControl>();
        FloatTipsScript = GameObject.Find("FloatTips").transform.Find("FloatTipsBase").GetComponent<FloatTips>();
        rend = GetComponent<Renderer>();
        startcolor = rend.material.color;
        buildmanager = BuildManager.instance;
        highlightLayer = LayerMask.NameToLayer("HighlightLayer");
        rendererHighlightManager = FindAnyObjectByType<RendererHighlightManager>();
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

    IEnumerator SelectTurretRotation(Node node)
    {
        FloatTipsScript.DisplayTips("移動游標設定效果範圍，右鍵確認");
        if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關") TipsText.Instance.ChangeText("移動游標設定效果範圍，右鍵確認");
        RendererHighlightManager rendererHighlightManager = FindAnyObjectByType<RendererHighlightManager>();

        speedControl.isForceSlowdown = true;
        GameObject Guide = GameObject.Find("Guide");
        if (Guide != null) Guide.SetActive(false);

        Node targetNode = node;
        BuildManager buildManager = GameObject.Find("GameControl").GetComponent<BuildManager>();
        turretBlueprint = GetTurretBlueprintDragging();
        buildManager.isBuilding = true;

        GameObject turret = Instantiate(turretBlueprint.prefab, targetNode.GetBuildPosition(), Quaternion.identity);
        CanvasGroup canvasGroup = turret.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f; // 隱藏血條（暫時）
        rendererHighlightManager.HighlightObjectOnly(turret);
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
                AttackRotation = UpdateNodeColors(character);
                turret.transform.eulerAngles = ForceCharacterRotation;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Character draggingCharacter = turretBlueprint.prefab?.GetComponent<Character>();
                Camera camera = GameObject.FindGameObjectWithTag(draggingCharacter.characterType)?.GetComponent<Camera>();
                Node NewNode = GetPositionNode(Input.mousePosition, camera);
                if (NewNode)
                {

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
        // 右鍵確認後將 Node 恢復為原本顏色
        ResetAllNodeColors();
        rendererHighlightManager.ResetTaggedObjectsLayer("Character");
        rendererHighlightManager.ResetTaggedObjectsLayer("Enemy");
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
                effectDisplayStartPos.z -= character.attackWidth;
                break;
            case 90.0f:
                //colliderStartPos.z -= character.attackLength;
                effectDisplayStartPos.z += character.attackLength;
                break;
            case 180.0f:
                //colliderStartPos.x -= character.attackLength;
                effectDisplayStartPos.z -= character.attackWidth;
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
        if (buildManager.DraggingCharacterIndex < 0) return null;
        else return buildManager.GetCharacterSet()[buildManager.DraggingCharacterIndex].turretBlueprint;
    }

    private Vector3 UpdateNodeColors(Character character)
    {
        Vector3 colliderStartPos = character.firePoint.position;
        switch (character.transform.eulerAngles.y)
        {
            case 0.0f:
                colliderStartPos.x += character.attackLength;
                break;
            case 90.0f:
                colliderStartPos.z -= character.attackLength;
                break;
            case 180.0f:
                colliderStartPos.x -= character.attackLength;
                break;
            case 270.0f:
                colliderStartPos.z += character.attackLength;
                break;
        }
        colliderStartPos.y -= 2.5f;
        Collider[] hitColliders = Physics.OverlapBox(
            colliderStartPos,
            new Vector3(character.attackWidth, character.attackHeight, character.attackLength),
            character.firePoint.rotation);

        ResetAllNodeColors();
        rendererHighlightManager.ResetTaggedObjectsLayer("Character");
        rendererHighlightManager.ResetTaggedObjectsLayer("Enemy");

        foreach (Collider hitCollider in hitColliders)
        {
            Node targetNode = hitCollider.GetComponent<Node>();
            if (targetNode != null)
            {
                if (character.characterType != "Wall")
                {
                    if (targetNode.tag == character.characterType)
                    {
                        ChangeNodeColor(targetNode, new Color(0.953f, 1f, 0.78f, 1f));
                    }
                }
                else
                {
                    if (true)
                    {
                        ChangeNodeColor(targetNode, new Color(0.953f, 1f, 0.78f, 1f));
                    }
                }
            }
            else
            {

                if (character.characterClass == "Attacker")
                {
                    Enemies targetEnemy = hitCollider.GetComponent<Enemies>();
                    if (targetEnemy != null)
                    {
                        rendererHighlightManager.HighlightObjectOnly(targetEnemy.gameObject);
                    }
                }
                else if (character.characterClass == "Healer")
                {
                    Character targetCharacter = hitCollider.GetComponent<Character>();
                    if (targetCharacter != null)
                    {
                        rendererHighlightManager.HighlightObjectOnly(targetCharacter.gameObject);
                    }
                }
            }
        }
        return character.transform.eulerAngles;
    }


    private void ResetAllNodeColors()
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        foreach (Node node in allNodes)
        {
            RestoreNodeColor(node);
        }
    }

    private void RestoreHighlightedNodes(Character character)
    {
        // 將已點亮的 Node 恢復為原本顏色
        Collider[] hitColliders = Physics.OverlapBox(
            character.firePoint.position,
            new Vector3(character.attackWidth / 2, character.attackHeight / 2, character.attackLength / 2),
            character.firePoint.rotation);

        foreach (var hitCollider in hitColliders)
        {
            Node node = hitCollider.GetComponent<Node>();
            if (node != null)
            {
                RestoreNodeColor(node); // 恢復為初始顏色
            }
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
        GameObject.Find("GameControl").GetComponent<BuildManager>().DraggingCharacterIndex = -1;
        speedControl.isForceSlowdown = false;
    }

    public void OnMouseEnterCustom()
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
    }

    public void OnMouseExitCustom()
    {
        rend.material.color = startcolor; // 恢復原色
    }
    public void OnMouseUpCustom()
    {
        BuildManager buildManager = GameObject.Find("GameControl")?.GetComponent<BuildManager>();
        TurretBlueprint turretBlueprint = GetTurretBlueprintDragging();
        RendererHighlightManager rendererHighlightManager = FindAnyObjectByType<RendererHighlightManager>();
        Camera highlightCamera = GameObject.FindGameObjectWithTag("HighlightCamera")?.GetComponent<Camera>();
        if (buildManager.isBuilding || turretBlueprint?.prefab == null || string.IsNullOrEmpty(turretBlueprint.prefab.name))
        {
            ResetHighlights(rendererHighlightManager, turretBlueprint);
            return;
        }
        Node node = GetPositionNode(Input.mousePosition, highlightCamera);

        if (node == null)
        {
            Debug.LogWarning("Failed to find target node. Cancelling build.");
            ResetHighlights(rendererHighlightManager, turretBlueprint);
            return;
        }
        rendererHighlightManager.ResetObject(GameObject.FindGameObjectsWithTag(node.tag));

        Character character = turretBlueprint.prefab.GetComponent<Character>();

        if (node.turret != null)
        {
            ShowError("無法放置在這裡！", rendererHighlightManager, turretBlueprint);
            return;
        }

        if (PlayerStats.Money < turretBlueprint.cost)
        {
            ShowError("熱量不足！", rendererHighlightManager, turretBlueprint);
            return;
        }

        if (character.characterType != node.tag)
        {
            ShowError("此角色無法放置在這裡！", rendererHighlightManager, turretBlueprint);
            return;
        }

        // 調整亮度並檢查方向選擇
        FindAnyObjectByType<CameraBrightnessController>()?.SetBrightness(-2.0f);
        ChangeNodeColor(node, Color.green);

        if (character.canSkipDirectionSelection)
        {
            buildManager.BuildTurretOn(node, Quaternion.identity, turretBlueprint);
        }
        else
        {
            StartCoroutine(SelectTurretRotation(node));
        }
    }

    private void ResetHighlights(RendererHighlightManager manager, TurretBlueprint blueprint)
    {
        if (blueprint?.prefab != null)
        {
            manager.ResetObject(GameObject.FindGameObjectsWithTag(blueprint.prefab.GetComponent<Character>().characterType));
        }
    }

    private void ShowError(string message, RendererHighlightManager manager, TurretBlueprint blueprint)
    {
        FloatTipsScript.DisplayTips(message);
        ResetHighlights(manager, blueprint);
        ChooseNodeExit();
    }

    Node GetPositionNode(Vector3 position, Camera camera = null, string highlightLayerName = "HighlightLayer")
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        // 獲取 HighlightLayer 的 Layer Index
        int highlightLayer = LayerMask.NameToLayer(highlightLayerName);
        if (highlightLayer == -1)
        {
            Debug.LogError($"Layer '{highlightLayerName}' 未找到，請確認已建立該 Layer！");
            return null;
        }

        Ray ray = camera.ScreenPointToRay(position);

        // 使用 RaycastAll 獲取所有命中的物件
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

        if (hits.Length > 0)
        {
            RaycastHit? highlightHit = null;
            RaycastHit? normalHit = null;

            // 遍歷所有命中的物件，優先選 HighlightLayer
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject.layer == highlightLayer)
                {
                    highlightHit = hit;
                    break; // 直接選 HighlightLayer，不再遍歷
                }
                else if (normalHit == null)
                {
                    normalHit = hit; // 如果沒有 HighlightLayer，記錄普通物件
                }
            }

            // 優先回傳 HighlightLayer 的物件，如果沒有則回傳普通物件
            RaycastHit finalHit = highlightHit ?? normalHit.Value;

            // 嘗試獲取 Node 組件
            Node node = finalHit.transform.gameObject.GetComponent<Node>();
            if (node != null)
            {
                return node;
            }
        }

        return null;
    }

    public void ChangeNodeColor(Node node, Color color)
    {
        if (node.tag == "Untagged") color.a = 0.2f;
        node.rend.material.color = color;
        SetLayerRecursively(node.gameObject, highlightLayer);
    }

    private void RestoreNodeColor(Node node)
    {
        node.rend.material.color = node.startcolor;
        RestoreLayerRecursively(node.gameObject);
    }
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            if (!originalLayers.ContainsKey(t.gameObject))
            {
                originalLayers[t.gameObject] = t.gameObject.layer;
            }
            t.gameObject.layer = newLayer;
        }
    }

    private void RestoreLayerRecursively(GameObject obj)
    {
        if (obj == null) return;
        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            if (originalLayers.TryGetValue(t.gameObject, out int originalLayer))
            {
                t.gameObject.layer = originalLayer;
            }
        }
    }
}
