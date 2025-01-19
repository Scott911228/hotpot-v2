using UnityEngine;

public class LineRendererUI : MonoBehaviour
{
    public LineRenderer lineRenderer;

    void Start()
    {
        // 確保 LineRenderer 已經設置
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // 設置點的數量
        lineRenderer.positionCount = 2;

        // 設置線的顏色
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 設置線的起點和終點
        SetLinePoints(new Vector2(0, 0), new Vector2(200, 100));
    }

    public void SetLinePoints(Vector2 start, Vector2 end)
    {
        // 將 UI 坐標轉換為世界坐標
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
