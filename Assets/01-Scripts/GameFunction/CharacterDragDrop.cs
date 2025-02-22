using UnityEngine;

public class CharacterDragDrop : MonoBehaviour
{
    Vector3 mousePosition;
    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
    private void OnMouseDown()
    {
        Debug.Log("test");
        mousePosition = Input.mousePosition - GetMousePos();
    }
    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }
}
