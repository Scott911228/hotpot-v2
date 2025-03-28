using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private GameObject Interactable;
    private FloatTips FloatTipsScript;
    public int shopIndex;
    SpeedControl speedControl;
    BuildManager buildmanager;
    [SerializeField] private Canvas canvas;
    Vector3 startPosition;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    void Start()
    {
        Interactable = GameObject.Find("GameControl/GameElement/Interactable");
        FloatTipsScript = GameObject.Find("FloatTips").transform.Find("FloatTipsBase").GetComponent<FloatTips>();
        startPosition = transform.position;
        buildmanager = GameObject.Find("GameControl").GetComponent<BuildManager>();
        speedControl = GameObject.Find("SpeedControl").GetComponent<SpeedControl>();
    }
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!Interactable.GetComponent<CanvasGroup>().interactable) return;
        if (name.Length >= 0) buildmanager.DraggingCharacterIndex = shopIndex;
        else
        {
            buildmanager.DraggingCharacterIndex = -1;
        }
        ;

        speedControl.isForceSlowdown = true;
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        CameraBrightnessController brightnessController = FindAnyObjectByType<CameraBrightnessController>();
        brightnessController.SetBrightness(-2.0f);
        RendererHighlightManager rendererHighlightManager = FindAnyObjectByType<RendererHighlightManager>();
        string tag = buildmanager.GetCharacterSet()[shopIndex].turretBlueprint.prefab?.GetComponent<Character>().characterType;
        rendererHighlightManager.HighlightObject(GameObject.FindGameObjectsWithTag(tag));
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!Interactable.GetComponent<CanvasGroup>().interactable) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        ReturnToOriginPos();
        speedControl.isForceSlowdown = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        CameraBrightnessController brightnessController = FindAnyObjectByType<CameraBrightnessController>();
        brightnessController.SetBrightness(0.0f);
        RendererHighlightManager rendererHighlightManager = FindAnyObjectByType<RendererHighlightManager>();
        string tag = buildmanager.GetCharacterSet()[shopIndex].turretBlueprint.prefab?.GetComponent<Character>().characterType;
        Camera highlightCamera = GameObject.FindGameObjectWithTag("HighlightCamera")?.GetComponent<Camera>();
        Node node = GetPositionNode(Input.mousePosition, highlightCamera);

        if (node == null) // 如果放開的地方沒有任何Node
        {
            rendererHighlightManager.ResetObject(GameObject.FindGameObjectsWithTag(tag));

            return;
        }
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

    public void ReturnToOriginPos()
    {
        transform.position = startPosition;
    }
}
