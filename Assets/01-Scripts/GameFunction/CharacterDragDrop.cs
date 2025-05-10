using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
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
    private static CharacterDragDrop currentHovered; // 靜態變量，唯一放大的物件
    void Start()
    {
        originalScale = transform.localScale;
        speedControl = GameObject.Find("SpeedControl").GetComponent<SpeedControl>();
        gameElement = GameObject.Find("GameElement");
        removeCharacterPanel = GameObject.Find("InteractableGuide/RemoveCharacterPanel");

        // 從 Canvas 上取得 GraphicRaycaster（假設你的 UI 在 Canvas 下）
        uiRaycaster = GameObject.Find("GameControl/InteractableGuide/RemoveCharacterPanel/Canvas").GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        if (isDragging) return;

        // **重置縮放狀態，防止放大遺留**
        ResetScale();
    }
    private void OnMouseDown()
    {
        pointerDown = true;
        dragStarted = false;
        mouseDownPosition = Input.mousePosition;
    }

    public void StartDragging()
    {
        if (!GameObject.Find("LevelSettings").GetComponent<LevelSettings>().isCharacterRemoveAvailable) return;
        if (dragPreviewPrefab == null) return;

        isDragging = true;

        // 縮回當前放大的物件
        if (currentHovered != null)
        {
            currentHovered.ResetScale();
            currentHovered = null;
        }

        dragPreviewInstance = Instantiate(dragPreviewPrefab);
        dragPreviewInstance.GetComponent<SpriteRenderer>().sprite = GetComponent<Character>().shopIcon;

        GameManager.isBuilding = true;
        speedControl.isForceSlowdown = true;
        gameElement.GetComponent<CanvasGroup>().alpha = 0f;

        SlideIn();
    }
    void EndDrag()
    {
        bool hitRemovePanel = false;

        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Transform current = result.gameObject.transform;

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
            Destroy(gameObject);
        }
        else
        {
            // **放置角色並立即重置縮放狀態**
            GameObject newCharacter = Instantiate(dragPreviewPrefab, transform.position, Quaternion.identity);
            CharacterDragDrop newDragDrop = newCharacter.GetComponent<CharacterDragDrop>();

            if (newDragDrop != null)
            {
                newDragDrop.ResetScale();

                // 更新 currentHovered 狀態為新生成物件
                currentHovered = newDragDrop;
            }
        }

        // **縮回當前放大的物件**
        if (currentHovered != null)
        {
            currentHovered.ResetScale();
            currentHovered = null;
        }

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

    void Update()
    {
        if (pointerDown && !dragStarted)
        {
            float distance = Vector3.Distance(Input.mousePosition, mouseDownPosition);
            if (distance > dragThreshold)
            {
                dragStarted = true;
                StartDragging();
            }
        }

        if (isDragging)
        {
            if (dragPreviewInstance != null)
            {
                Vector3 mousePos = Input.mousePosition;
                Ray dragRay = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(dragRay, out RaycastHit hit))
                {
                    dragPreviewInstance.transform.position = hit.point;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }

            return; // 拖曳中不進行縮放檢查
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        RaycastHit? closestHit = hits.OrderBy(hit => hit.distance).FirstOrDefault();
        CharacterDragDrop hovered = null;

        if (closestHit.HasValue)
        {
            hovered = closestHit.Value.transform.GetComponent<CharacterDragDrop>();
        }

        if (hovered != null && hovered != currentHovered)
        {
            if (currentHovered != null)
            {
                currentHovered.ResetScale();
            }

            currentHovered = hovered;
            currentHovered.OnMouseEnterCustom();
        }
        else if (hovered == null && currentHovered != null)
        {
            currentHovered.ResetScale();
            currentHovered = null;
        }

        if (pointerDown && Input.GetMouseButtonUp(0) && !dragStarted)
        {
            pointerDown = false;
            OnClick();
        }
    }

    public void OnMouseEnterCustom()
    {
        if (isDragging) return;
        transform.DOScale(originalScale * hoverScaleMultiplier, 0.2f).SetEase(Ease.OutBack);
    }

    public void ResetScale()
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnMouseExitCustom()
    {
        if (isDragging) return; // 拖曳中不要縮回

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        // 判斷游標下是否還有當前物件
        bool isStillHovering = hits.Any(hit => hit.transform == transform);

        // 當前物件不再位於游標下方時，才執行縮回
        if (!isStillHovering)
        {
            transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
        }
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

}
