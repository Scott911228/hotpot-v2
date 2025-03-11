using UnityEngine;

public class PathSettings : MonoBehaviour
{
    [System.Serializable]
    public class PathData
    {
        public Transform spawnPoint; // 出發點
        public int pathIndex; // 路徑編號
        public Transform[] points; // 路徑節點
    }

    public PathData[] pathData;

    // 取得對應的出發點和路線
    public PathData GetPathData(int index)
    {
        if (index < 0 || index >= pathData.Length) return null;
        return pathData[index];
    }
    // 取得對應的路徑節點
    public Transform[] GetPoints(int index)
    {
        if (index < 0 || index >= pathData.Length) return null;
        return pathData[index].points;
    }

    public int GetPathCount()
    {
        return pathData.Length;
    }
}
