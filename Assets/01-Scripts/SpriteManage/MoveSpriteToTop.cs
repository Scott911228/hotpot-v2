using UnityEngine;

public class MoveSpriteToTop : MonoBehaviour
{
    public int topSortingOrder = 10; // 設定目標的排序值

    public void MoveToTop()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = topSortingOrder;
        }
    }
}
