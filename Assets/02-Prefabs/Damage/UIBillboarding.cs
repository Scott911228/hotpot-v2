using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboarding : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera cam;
    private void Awake()
    {
        cam = Camera.main;

    }
    private void Update()
    {
        transform.forward = cam.transform.forward;
    }
}
