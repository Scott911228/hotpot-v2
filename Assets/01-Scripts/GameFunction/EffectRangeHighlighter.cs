// EffectRangeHighlighter.cs（支援子物件正確高亮與還原版）
using UnityEngine;
using System.Collections.Generic;

public class EffectRangeHighlighter : MonoBehaviour
{
    public GameObject effectRange; // 指定你的 effectrange 物件
    public string targetTag1 = "Node";
    public string targetTag2 = "Character";
    public string highlightLayerName = "HighlightLayer";

    private int highlightLayer;
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

    void Start()
    {
        highlightLayer = LayerMask.NameToLayer(highlightLayerName);
    }

    void Update()
    {
        if (effectRange == null) return;

        Vector3 center = effectRange.transform.position;
        Vector3 halfExtents = effectRange.transform.localScale / 2f;

        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag(targetTag1));
        targets.AddRange(GameObject.FindGameObjectsWithTag(targetTag2));

        HashSet<GameObject> currentlyInside = new HashSet<GameObject>();

        foreach (var obj in targets)
        {
            if (obj == null) continue;

            Vector3 pos = obj.transform.position;
            bool isInside =
                Mathf.Abs(pos.x - center.x) <= halfExtents.x &&
                Mathf.Abs(pos.z - center.z) <= halfExtents.z;

            if (isInside)
            {
                currentlyInside.Add(obj);

                if (!originalLayers.ContainsKey(obj))
                {
                    SaveOriginalLayersRecursively(obj);
                    SetLayerRecursively(obj, highlightLayer);
                }
            }
        }

        List<GameObject> toRestore = new List<GameObject>();
        foreach (var pair in originalLayers)
        {
            if (!currentlyInside.Contains(pair.Key))
            {
                RestoreLayerRecursively(pair.Key);
                toRestore.Add(pair.Key);
            }
        }

        foreach (var obj in toRestore)
        {
            originalLayers.Remove(obj);
        }
    }

    private void SaveOriginalLayersRecursively(GameObject obj)
    {
        if (obj == null) return;

        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            GameObject child = t.gameObject;
            if (!originalLayers.ContainsKey(child))
            {
                originalLayers[child] = child.layer;
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = newLayer;
        }
    }

    private void RestoreLayerRecursively(GameObject obj)
    {
        if (obj == null) return;

        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            GameObject child = t.gameObject;
            if (originalLayers.TryGetValue(child, out int originalLayer))
            {
                child.layer = originalLayer;
            }
        }
    }

}
