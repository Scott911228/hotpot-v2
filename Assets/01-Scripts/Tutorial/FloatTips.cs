using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatTips : MonoBehaviour
{
    public GameObject FloatTipsBase;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void DisplayTips(string text)
    {
        GameObject temp = (GameObject)Instantiate(FloatTipsBase, GameObject.Find("FloatTips").transform);
        Text theText = temp.transform.GetComponent<Text>();
        theText.text = text;
        temp.SetActive(true);
    }
}
