using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionDisplay : MonoBehaviour
{
    public string DisplayEmotionType;
    protected Animator animator;
    private Animation anim;
    // Start is called before the first frame update
    public void DisplayEmotion(string DisplayEmotionType)
    {
        animator = GetComponent<Animator>();
        anim = gameObject.GetComponent<Animation>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Emotions/anim_controller/emo_{DisplayEmotionType}_ss_0");
        gameObject.SetActive(true);
    }
    public void HideEmotion()
    {
        gameObject.SetActive(false);
    }
}
