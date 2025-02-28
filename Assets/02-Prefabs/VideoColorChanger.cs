using UnityEngine;

public class VideoColorChanger : MonoBehaviour
{
    private Material videoMaterial;
    private Color colorTint = Color.white;

    void Start()
    {
        videoMaterial = GetComponent<MeshRenderer>().materials[0];
    }
    void Update()
    {
        colorTint = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().FlavorColor;
        videoMaterial.SetColor("_ColorTint", colorTint);
    }
}
