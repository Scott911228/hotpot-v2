using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraBrightnessController : MonoBehaviour
{
    public Volume postProcessingVolume; // 指定場景中的 Volume
    private ColorAdjustments colorAdjustments; // 用來調整亮度

    void Start()
    {
        // 嘗試獲取 Color Adjustments 效果
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.Log("Color Adjustments 元件已找到");
        }
        else
        {
            Debug.LogError("無法找到 Color Adjustments，請確認 Volume 設置是否正確！");
        }
    }

    // 調整亮度的方法，負數為變暗，正數為變亮
    public void SetBrightness(float brightness)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = brightness;
            Debug.Log($"亮度設定為: {brightness}");
        }
        else
        {
            Debug.LogWarning("Color Adjustments 尚未初始化");
        }
    }
}
