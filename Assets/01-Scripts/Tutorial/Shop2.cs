using UnityEngine;

public class Shop2 : MonoBehaviour
{
    public TurretBlueprint standardTurret2;
    public TurretBlueprint anotherTurret2;

    BuildManager buildmanager2;

    void Start()
    {
        buildmanager2= BuildManager.instance;
    }
    public void SelectStandardTurret()
    {
        Debug.Log("Standard Tuuret Selected");
        buildmanager2.SelectTurretToBuild(standardTurret2);
    }

    public void SelectAnotherStandardTurret()
    {
        Debug.Log("Standard Tuuret 2 Selected");
        buildmanager2.SelectTurretToBuild(anotherTurret2);
    }
}
