using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver;
    public static bool isBuilding;
    public GameObject SpeedControl;
    public GameObject GameOverUI;
    public GameObject CompleteLevelUI;
    public GameObject InfoPanel;
    public GameObject InteractableSubs;
    public string characterTag = "Character";
    public string enemyTag = "Enemy";
    public GameObject damageTextPrefab;
    public bool isGamePlaying = true;
    private GameObject removeCharacterPanel;
    private GameObject broadcastMessagePanel;
    bool isWaiting = false;
    public static bool isRestarted = false;
    private PlayerStats playerStats;
    public void DoStageStoryMustPlayCheck()
    {
        if (isRestarted)
        {
            TextControl.BroadcastControlMessage("retry");
        }
        else
        {
            TextControl.BroadcastControlMessage("start");
        }
    }
    public void setRemoveCharacterPanelActive(bool visiblility)
    {
        //removeCharacterPanel.SetActive(visiblility);
    }
    public void DisplayDamage(GameObject characterInstance, string textToDisplay, float textSize, Color32 textColor)
    {
        GameObject DamageTextInstance = Instantiate(damageTextPrefab, Random.insideUnitSphere * 2f + characterInstance.transform.position, characterInstance.transform.rotation);
        DamageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(textToDisplay);
        DamageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize = textSize;
        DamageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().outlineColor = textColor;
    }

    public void DisplayFloatingText(GameObject characterInstance, string textToDisplay, float textSize, Color32 textColor)
    {
        GameObject FloatingTextInstance = Instantiate(damageTextPrefab, characterInstance.transform.position, characterInstance.transform.rotation);
        FloatingTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(textToDisplay);
        FloatingTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().fontSize = textSize;
        FloatingTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().outlineColor = textColor;
    }
    public void RemoveAllCharacter()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag(characterTag);
        foreach (GameObject character in characters)
        {
            Destroy(character);
        }
    }
    public void RemoveCharacter(string name)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag(characterTag);
        foreach (GameObject character in characters)
        {
            if (character.name == name) Destroy(character);
        }
    }
    void Start()
    {
        broadcastMessagePanel = GameObject.Find("GameControl/GameElement/BroadcastMessage");
        removeCharacterPanel = GameObject.Find("GameControl/InteractableGuide/RemoveCharacterPanel");
        playerStats = GameObject.Find("GameControl").GetComponent<PlayerStats>();
        setRemoveCharacterPanelActive(false);
        SpeedControl.GetComponent<SpeedControl>().isForceNoSpeed = false;
        InvokeRepeating("WaitingPauseEffect", 0f, 0.1f);
        isGameOver = false;
        string entryMessage = GameObject.Find("LevelSettings").GetComponent<LevelSettings>().StageEntryMessage;
        if (!string.IsNullOrEmpty(entryMessage)) BroadcastMessage(entryMessage);
    }
    public void BroadcastMessage(string text, float slideInTime = 1, float stayTime = 2, float slideOutTime = 1)
    {
        broadcastMessagePanel.GetComponentInChildren<Text>().text = text;
        broadcastMessagePanel.transform.DOLocalMove(new Vector3(0, 192, 0), slideInTime).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(stayTime, () =>
                        {
                            broadcastMessagePanel.transform.DOLocalMove(new Vector3(0, 258, 0), slideOutTime).SetEase(Ease.InOutSine);
                        });
                    });
    }
    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        if (PlayerStats.Life <= 0)
        {
            LoseLevel();
            GameObject[] Enemies = GameObject.FindGameObjectsWithTag(enemyTag);

            foreach (GameObject Enemy in Enemies)
            {
                Enemy.GetComponent<Enemies>().isPaused = true;
                Enemy.GetComponent<EnemyAttack>().isPaused = true;
            }
        }
    }

    public void StartDialog()
    {
        isGamePlaying = false;
        SpeedControl.GetComponent<SpeedControl>().isForceNoSpeed = true;
        InteractableSubs.GetComponent<FadeInOut>().fadeIn = false;
        InteractableSubs.GetComponent<FadeInOut>().fadeOut = true;
        InteractableSubs.GetComponent<FadeInOut>().enabled = true;
        InteractableSubs.GetComponent<CanvasGroup>().interactable = false;
        InfoPanel.GetComponent<FadeInOut>().fadeIn = false;
        InfoPanel.GetComponent<FadeInOut>().fadeOut = true;
        InfoPanel.GetComponent<FadeInOut>().enabled = true;
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject[] Characters = GameObject.FindGameObjectsWithTag(characterTag);

        foreach (GameObject Enemy in Enemies)
        {
            Enemy.GetComponent<Enemies>().isPaused = true;
            Enemy.GetComponent<EnemyAttack>().isPaused = true;
        }
        foreach (GameObject Character in Characters)
        {
            Character.GetComponent<Character>().isPaused = true;
        }
    }
    public void EndDialog()
    {
        isGamePlaying = true;
        SpeedControl.GetComponent<SpeedControl>().isForceNoSpeed = false;
        InteractableSubs.GetComponent<FadeInOut>().fadeIn = true;
        InteractableSubs.GetComponent<FadeInOut>().fadeOut = false;
        InteractableSubs.GetComponent<FadeInOut>().enabled = true;
        InteractableSubs.GetComponent<CanvasGroup>().interactable = true;
        InfoPanel.GetComponent<FadeInOut>().fadeIn = true;
        InfoPanel.GetComponent<FadeInOut>().fadeOut = false;
        InfoPanel.GetComponent<FadeInOut>().enabled = true;
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject[] Characters = GameObject.FindGameObjectsWithTag(characterTag);

        foreach (GameObject Enemy in Enemies)
        {
            Enemy.GetComponent<Enemies>().isPaused = false;
            Enemy.GetComponent<EnemyAttack>().isPaused = false;
        }
        foreach (GameObject Character in Characters)
        {
            Character.GetComponent<Character>().isPaused = false;
        }
    }

    public void AddMoney(int money)
    {
        playerStats.Money += money;
    }
    public void SetMoney(int money)
    {
        playerStats.Money = money;
    }
    public void WaitingPauseEffect()
    {
        if (!isWaiting) return;
        isGamePlaying = false;
        GameObject[] Characters = GameObject.FindGameObjectsWithTag(characterTag);
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject Enemy in Enemies)
        {
            Enemy.GetComponent<Enemies>().isPaused = true;
            Enemy.GetComponent<EnemyAttack>().isPaused = true;
        }
        foreach (GameObject Character in Characters)
        {
            Character.GetComponent<Character>().isPaused = true;
        }
    }
    public void WaitForBuild()
    {
        isGamePlaying = false;
        isWaiting = true;
        BuildManager.WaitForFullBuild = true;
    }
    public void AfterBuild()
    {
        isGamePlaying = true;
        isWaiting = false;
        BuildManager.WaitForFullBuild = false;
    }

    public void EndLevel()
    {
        isGamePlaying = false;
        if (!isGameOver)
        {
            AchievementManager.Instance.CheckAchievements();
            isGameOver = true;
            SpeedControl.GetComponent<SpeedControl>().isForceNoSpeed = true;
        }

    }
    public void LoseLevel()
    {
        EndLevel();
        GameOverUI.SetActive(true);
    }
    public void WinLevel()
    {
        GameStats.Instance.LevelCleared = true;
        isWaiting = true;
        EndLevel();
        if (PlayerStats.Life <= 0)
        {
            GameOverUI.SetActive(true);
        }
        else
        {
            CompleteLevelUI.SetActive(true);
        }
    }
}
