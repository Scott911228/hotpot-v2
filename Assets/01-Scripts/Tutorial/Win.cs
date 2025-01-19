using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Win : MonoBehaviour
{
    public Text roundsText;

    void Awake()
    {
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {

        roundsText.text = "0";
        int round = 0;

        yield return new WaitForSeconds(.07f);

        while (round < GameObject.Find("GameControl").GetComponent<PlayerStats>().Rounds)
        {
            round++;
            roundsText.text = round.ToString();

            yield return new WaitForSeconds(.05f);
        }

    }
}
