using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class TurretBlueprint 
{   
    public GameObject prefab;
    public int cost;

    public static implicit operator GameObject(TurretBlueprint v)
    {
        throw new NotImplementedException();
    }
}
