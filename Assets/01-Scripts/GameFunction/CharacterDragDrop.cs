using UnityEngine;

public class CharacterDragDrop : MonoBehaviour
{
    Vector3 mousePosition;
    private void OnMouseDown()
    {
        Debug.Log("test");
        BuildManager buildManager = GameObject.Find("GameControl").GetComponent<BuildManager>();
        buildManager.isBuilding = true;

    }
}
