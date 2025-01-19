using UnityEngine;

public class CharacterPlacer : MonoBehaviour
{
    public GameObject[] characterPrefabs; // 角色預製件
    public Transform[] spawnPoints; // 角色放置點

    public void PlaceCharacter(int index)
    {
        if (index < 0 || index >= characterPrefabs.Length) return; // 確保索引有效

        GameObject characterPrefab = characterPrefabs[index];
        Vector3 spawnPosition = spawnPoints[index].position;

        // 檢查放置位置的標籤
        Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.1f); // 檢查附近的碰撞體

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Node") && characterPrefab.CompareTag("FloorCharacter"))
            {
                // 放置在地板上
                Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
                return;
            }
            else if (collider.CompareTag("Wall") && characterPrefab.CompareTag("PlatformCharacter"))
            {
                // 放置在高台上
                Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
                return;
            }
        }

        Debug.Log("無法在此位置放置角色");
    }
}
