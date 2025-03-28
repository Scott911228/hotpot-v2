using UnityEngine;

public class MouseEnterSimulator : MonoBehaviour
{
    public Camera mainCamera;
    public string highlightLayerName = "HighlightLayer";
    private IMouseInteractable currentInteractable = null;
    private int highlightLayer;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 獲取 HighlightLayer 的 Layer Index
        highlightLayer = LayerMask.NameToLayer(highlightLayerName);
        if (highlightLayer == -1)
        {
            Debug.LogError($"Layer '{highlightLayerName}' 未找到，請確認已建立該 Layer！");
        }
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // 使用 RaycastAll 獲取所有命中的物件
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

        if (hits.Length > 0)
        {
            // 優先選擇 HighlightLayer 的物件
            RaycastHit? highlightHit = null;
            RaycastHit? normalHit = null;

            foreach (var hit in hits)
            {
                IMouseInteractable interactable = hit.transform.GetComponent<IMouseInteractable>();

                if (interactable != null)
                {
                    if (hit.transform.gameObject.layer == highlightLayer)
                    {
                        highlightHit = hit;
                        break; // 優先選中 HighlightLayer 的物件
                    }
                    else if (normalHit == null)
                    {
                        normalHit = hit; // 如果沒有 HighlightLayer，選擇普通物件
                    }
                }
            }

            // 優先處理 HighlightLayer
            RaycastHit finalHit = highlightHit ?? normalHit.Value;
            IMouseInteractable finalInteractable = finalHit.transform.GetComponent<IMouseInteractable>();

            if (finalInteractable != null)
            {
                // OnMouseEnterCustom
                if (currentInteractable != finalInteractable)
                {
                    if (currentInteractable != null)
                    {
                        currentInteractable.OnMouseExitCustom();
                    }
                    currentInteractable = finalInteractable;
                    currentInteractable.OnMouseEnterCustom();
                }
            }
        }
        else
        {
            // OnMouseExitCustom
            if (currentInteractable != null)
            {
                currentInteractable.OnMouseExitCustom();
                currentInteractable = null;
            }
        }

        // OnMouseUpCustom
        if (Input.GetMouseButtonUp(0)) // 0 代表左鍵
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnMouseUpCustom();
            }
        }
    }
}
