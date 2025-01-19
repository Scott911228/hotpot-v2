using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SlidingNumber : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public float animationTime = 1.5f;

    private float desiredNumber, initialNumber, currentNumber;

    public void SetNumber(float number)
    {
        initialNumber = currentNumber;
        desiredNumber = number;
    }

    public void AddToNumber(float number)

    {
        initialNumber = currentNumber;
        desiredNumber += number;
    }

    public void Update() 
    {
        if(currentNumber != desiredNumber)
        {
            if(initialNumber < desiredNumber)
            {
                currentNumber += (animationTime * Time.deltaTime) * (desiredNumber - initialNumber);
                if(currentNumber > desiredNumber)
                {
                    currentNumber = desiredNumber;
                }
            }
            else
            {
                currentNumber -= (animationTime * Time.deltaTime) * (initialNumber - desiredNumber);
                if(currentNumber <= desiredNumber)
                {
                    currentNumber = desiredNumber;
                }
            }

            numberText.text = Math.Round(currentNumber).ToString();
        }
    }
}
