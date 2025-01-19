using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private SlidingNumber slidingNumber;
    public Animator animator1;
    public Animator animator2;

    void Start()
    {
        GameObject StageClearCanvas = GameObject.Find("StageClearCanvas");
        animator1 = GetComponent<Animator>();
        animator2 = GetComponent<Animator>();
        StartCoroutine(PlayAnimationsSequentially());
    }

    IEnumerator PlayAnimationsSequentially()
    {
        // 播放第一個動畫

        animator1.Play("StarFall_3");
        
        // 等待第一個動畫播放完成
        while (animator1.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return new WaitForSeconds(1.0f);
        }

        // 播放第二個動畫
        animator2.Play("FadeIn");

        slidingNumber = GetComponent<SlidingNumber>();
        slidingNumber.AddToNumber(1000);
    }

}



    