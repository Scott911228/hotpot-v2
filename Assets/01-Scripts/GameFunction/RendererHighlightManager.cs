using UnityEngine;
using System.Collections.Generic;

public class RendererHighlightManager : MonoBehaviour
{
    public Camera overlayCamera;
    public string highlightLayerName = "HighlightLayer";
    public string tempLayerName = "TempLayer";
    public string defaultLayerName = "Default";

    private int highlightLayer;
    private int tempLayer;
    private int defaultLayer;
    private Dictionary<Renderer, int> originalLayers = new Dictionary<Renderer, int>();

    void Start()
    {
        highlightLayer = LayerMask.NameToLayer(highlightLayerName);
        tempLayer = LayerMask.NameToLayer(tempLayerName);
        defaultLayer = LayerMask.NameToLayer(defaultLayerName);

        if (highlightLayer == -1)
        {
            Debug.LogError($"Layer '{highlightLayerName}' 未找到，請確認已建立該 Layer！");
        }

        if (tempLayer == -1)
        {
            Debug.LogError($"Layer '{tempLayerName}' 未找到，請確認已建立該 Layer！");
        }
    }

    // 高亮所有 Renderer 並將 Character/Enemy 移入 TempLayer
    public void HighlightObject(params GameObject[] objects)
    {
        if (objects == null || objects.Length == 0) return;
        // 高亮指定物件
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (!originalLayers.ContainsKey(renderer))
                {
                    originalLayers[renderer] = renderer.gameObject.layer;
                }
                renderer.gameObject.layer = highlightLayer;
            }
        }

        // 把所有 Tag 為 Character 或 Enemy 的物件移入 TempLayer
        MoveTaggedObjectsToLayer("Character", tempLayer);
        MoveTaggedObjectsToLayer("Enemy", tempLayer);
    }

    // 僅高亮所有 Renderer
    public void HighlightObjectOnly(params GameObject[] objects)
    {
        if (objects == null || objects.Length == 0) return;
        // 高亮指定物件
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (!originalLayers.ContainsKey(renderer))
                {
                    originalLayers[renderer] = renderer.gameObject.layer;
                }
                renderer.gameObject.layer = highlightLayer;
            }
        }
    }

    // 恢復所有 Renderer 並還原 Character/Enemy 的 Layer
    public void ResetObject(GameObject[] objects)
    {
        if (objects == null || objects.Length == 0) return;
        // 還原所有 Tag 為 Character 或 Enemy 的物件 Layer
        ResetTaggedObjectsLayer("Character");
        ResetTaggedObjectsLayer("Enemy");
        // 恢復指定物件的原始 Layer
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (originalLayers.ContainsKey(renderer))
                {
                    renderer.gameObject.layer = originalLayers[renderer];
                    originalLayers.Remove(renderer);
                }
            }
        }
    }
    public void ResetObjectToDefault(GameObject obj)
    {
        if (obj == null) return;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.gameObject.layer = defaultLayer;
        }
    }

    // 移入 TempLayer
    private void MoveTaggedObjectsToLayer(string tag, int layer)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in taggedObjects)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (!originalLayers.ContainsKey(renderer))
                {
                    originalLayers[renderer] = renderer.gameObject.layer;
                }
                renderer.gameObject.layer = layer;
            }
        }
    }

    // 還原 Layer
    public void ResetTaggedObjectsLayer(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in taggedObjects)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (originalLayers.ContainsKey(renderer))
                {
                    renderer.gameObject.layer = originalLayers[renderer];
                    originalLayers.Remove(renderer);
                }
            }
        }
    }
}
