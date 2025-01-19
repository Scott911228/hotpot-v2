using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint character1;
    public TurretBlueprint character2;
    public TurretBlueprint character3;

    BuildManager buildmanager;

    void Start()
    {
        buildmanager = BuildManager.instance;
    }
    public void SelectCharacter1()
    {
        Debug.Log("Standard Tuuret Selected");
        buildmanager.SelectTurretToBuild(character1);
    }

    public void SelectCharacter2()
    {
        Debug.Log("Standard Tuuret 2 Selected");
        buildmanager.SelectTurretToBuild(character2);
    }
    public void SelectCharacter3()
    {
        Debug.Log("Standard Tuuret 3 Selected");
        buildmanager.SelectTurretToBuild(character3);
    }
}
