using UnityEngine;

public class EffectRangeDisplay : MonoBehaviour
{
    private float ScrollSpeed = 0.04f;
    void Update()
    {
        // 攻擊範圍的材質捲動速度
        float offset = Time.time * ScrollSpeed * 1 / Time.timeScale;
        GetComponent<Renderer>().material.SetTextureOffset("_BaseMap", new Vector2(offset, 0));
    }
}
