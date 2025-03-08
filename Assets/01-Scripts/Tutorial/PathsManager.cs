using UnityEngine;

public class PathsManager : MonoBehaviour
{
    [System.Serializable]
    public class PathData
    {
        public Transform spawnPoint; // 出發點
        public Paths path; // 對應的路線
    }

    public PathData[] pathData;

    // 取得對應的出發點和路線
    public PathData GetPathData(int index)
    {
        if (index < 0 || index >= pathData.Length) return null;
        return pathData[index];
    }

    public int GetPathCount()
    {
        return pathData.Length;
    }
}
