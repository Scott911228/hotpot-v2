using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UIElements;

public class CharacterDragDrop : MonoBehaviour, IMouseInteractable
{
    Vector3 mousePosition;

    public GameObject dragPreviewPrefab;
    private GameObject dragPreviewInstance;
    private bool isDragging = false;
    private SpeedControl speedControl;
    private GameObject gameElement;
    public GameObject removeCharacterPanel;

    private GraphicRaycaster uiRaycaster;
    private EventSystem eventSystem;
    private Vector3 mouseDownPosition;
    private bool pointerDown = false;
    private bool dragStarted = false;
    public float dragThreshold = 10f;
    private Vector3 originalScale;
    private float hoverScaleMultiplier = 1.1f; // 碰到時的放大倍率
    private bool isHovering = false;
    void Start()
    {
        originalScale = transform.localScale;
        speedControl = GameObject.Find("SpeedControl").GetComponent<SpeedControl>();
        gameElement = GameObject.Find("GameElement");
        removeCharacterPanel = GameObject.Find("InteractableGuide/RemoveCharacterPanel");

        // 從 Canvas 上取得 GraphicRaycaster（假設你的 UI 在 Canvas 下）
        uiRaycaster = GameObject.Find("GameControl/InteractableGuide/RemoveCharacterPanel/Canvas").GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }
    private void OnMouseDown()
    {
        pointerDown = true;
        dragStarted = false;
        mouseDownPosition = Input.mousePosition;
    }

    public void StartDragging()
    {
        if (dragPreviewPrefab == null) return;

        isDragging = true;
        dragPreviewInstance = Instantiate(dragPreviewPrefab);
        dragPreviewInstance.GetComponent<SpriteRenderer>().sprite = GetComponent<Character>().shopIcon;

        GameManager.isBuilding = true;
        speedControl.isForceSlowdown = true;
        gameElement.GetComponent<CanvasGroup>().alpha = 0f;
        GameObject.Find("GameControl").GetComponent<GameManager>().setRemoveCharacterPanelActive(true);
        SlideIn();
    }
    void Update()
    {
        if (!isDragging) // 偵測是否碰到游標並且未被點擊
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            bool hitSelf = false;

            foreach (var hit in hits)
            {
                if (hit.transform == transform)
                {
                    hitSelf = true;
                    break;
                }
            }
            if (hitSelf)
            {
                if (!isHovering)
                {
                    OnMouseEnterCustom();
                    isHovering = true;
                }
            }
            else
            {
                if (isHovering)
                {
                    OnMouseExitCustom();
                    isHovering = false;
                }
            }

        }
        if (pointerDown && !dragStarted)
        {
            float distance = Vector3.Distance(Input.mousePosition, mouseDownPosition);
            if (distance > dragThreshold)
            {
                // 真正啟動拖曳行為
                dragStarted = true;
                StartDragging();
            }
        }

        if (isDragging && dragPreviewInstance != null)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                dragPreviewInstance.transform.position = hit.point;
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }

        // 點擊結束
        if (pointerDown && Input.GetMouseButtonUp(0) && !dragStarted)
        {
            pointerDown = false;
            OnClick(); // 觸發點擊事件
        }
    }
    public void OnMouseEnterCustom()
    {
        if (isDragging) return; // 拖曳中不要放大
        transform.DOScale(originalScale * hoverScaleMultiplier, 0.2f).SetEase(Ease.OutBack);
    }
    public void OnMouseExitCustom()
    {
        if (isDragging) return; // 拖曳中不要縮回
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }
    public void OnMouseUpCustom()
    { }
    private void OnClick()
    {
        Debug.Log("角色被點擊了！");
        CharacterSkill characterSkill = GetComponent<CharacterSkill>();
        if (characterSkill.currentMP >= characterSkill.targetMP)
        {
            characterSkill.RunSkill();
        }
        ;
        // 你可以在這裡打開說明、播放動畫、彈出提示等
    }

    public void SlideIn()
    {
        GameObject go = GameObject.Find("GameControl/InteractableGuide/RemoveCharacterPanel/Canvas/Display");

        if (go != null)
        {
            RectTransform display = go.GetComponent<RectTransform>();
            if (display != null)
            {
                // 要*0.05是因為放置時的遊戲會放慢
                display.DOAnchorPos(new Vector2(0, -505), 1f * 0.05f).SetEase(Ease.OutCubic);
            }
            else
            {
                Debug.LogWarning("找不到 RectTransform，請確認 Display 是 UI 元件");
            }
        }
        else
        {
            Debug.LogWarning("找不到 Display，請確認路徑是否正確");
        }
    }
    public void SlideOut()
    {
        GameObject go = GameObject.Find("GameControl/InteractableGuide/RemoveCharacterPanel/Canvas/Display");

        if (go != null)
        {
            RectTransform display = go.GetComponent<RectTransform>();
            if (display != null)
            {
                display.DOAnchorPos(new Vector2(0, -787), 1f).SetEase(Ease.OutCubic)
            .OnComplete(() => go.SetActive(true));
            }
            else
            {
                Debug.LogWarning("找不到 RectTransform，請確認 Display 是 UI 元件");
            }
        }
        else
        {
            Debug.LogWarning("找不到 Display，請確認路徑是否正確");
        }
    }
    void EndDrag()
    {
        bool hitRemovePanel = false;

        // 使用 UI 射線檢測是否點到 removeCharacterPanel
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Transform current = result.gameObject.transform;

            // 一層層往上找是否包含 RemoveCharacterPanel
            while (current != null)
            {
                if (current.name == "RemoveCharacterPanel")
                {
                    hitRemovePanel = true;
                    break;
                }
                current = current.parent;
            }

            if (hitRemovePanel) break;
        }
        if (hitRemovePanel)
        {
            Debug.Log("命中 RemoveCharacterPanel → 移除角色");
            Destroy(gameObject); // 真正移除角色
        }
        else
        {
            Debug.Log("未命中 RemoveCharacterPanel → 取消拖曳");
        }

        // 拖曳結束處理
        if (dragPreviewInstance != null)
        {
            Destroy(dragPreviewInstance);
        }

        isDragging = false;
        GameManager.isBuilding = false;
        speedControl.isForceSlowdown = false;
        gameElement.GetComponent<CanvasGroup>().alpha = 1.0f;
        SlideOut();
    }

}
