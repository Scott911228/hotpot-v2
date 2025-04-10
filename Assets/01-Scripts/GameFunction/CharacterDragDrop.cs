using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterDragDrop : MonoBehaviour
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

    void Start()
    {
        speedControl = GameObject.Find("SpeedControl").GetComponent<SpeedControl>();
        gameElement = GameObject.Find("GameElement");
        removeCharacterPanel = GameObject.Find("InteractableGuide/RemoveCharacterPanel");

        // 從 Canvas 上取得 GraphicRaycaster（假設你的 UI 在 Canvas 下）
        uiRaycaster = GameObject.Find("GameControl/InteractableGuide/RemoveCharacterPanel/Canvas").GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    private void OnMouseDown()
    {
        StartDragging();
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
    }

    void Update()
    {
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
        GameObject.Find("GameControl").GetComponent<GameManager>().setRemoveCharacterPanelActive(false);
    }
}
