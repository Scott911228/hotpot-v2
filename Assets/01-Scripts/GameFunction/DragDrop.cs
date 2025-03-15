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
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!Interactable.GetComponent<CanvasGroup>().interactable) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        ReturnToOriginPos();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
    public void ReturnToOriginPos()
    {
        transform.position = startPosition;
    }
}
