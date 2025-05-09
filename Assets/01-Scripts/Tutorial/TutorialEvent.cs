using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialEvent : MonoBehaviour
{
    public GameObject imageObject;
    public Sprite[] sprites;
    private bool canClickToHide = false;
    private int _index = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (canClickToHide && Input.GetMouseButtonDown(0))
        {
            hide();
        }
    }
    public void show(int index)
    {
        _index = index;
        imageObject.GetComponent<Image>().sprite = sprites[index];
        imageObject.transform.localScale = Vector3.zero;
        imageObject.transform.DOScale(5.5f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            canClickToHide = true; // 彈出完畢才允許關閉
        });
    }

    // Update is called once per frame
    public void hide()
    {
        if (!canClickToHide) return;
        canClickToHide = false; // 防止重複觸發
        imageObject.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第一關")
            {
                if (_index <= 2) show(_index + 1);
                else if (_index == 3) TextControl.BroadcastControlMessage("tutorial/guideclose1");
                else if (_index == 4) TextControl.BroadcastControlMessage("tutorial/text2");
                else if (_index == 5) TextControl.BroadcastControlMessage("tutorial/guideclose3");
            }
            else if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第二關")
            {
                if (_index == 0) TextControl.BroadcastControlMessage("tutorial2/guideclose");
            }
            else if (GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageName == "第三關")
            {
                if (_index == 0) show(_index + 1);
                else if (_index == 1) TextControl.BroadcastControlMessage("level3/guideclose");
            }
        });
    }
}
