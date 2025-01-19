
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    bool isGameOver = GameManager.isGameOver;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isGameOver)
        {
            SetVolume(0.1f);
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            // 將音量限制在 0.0 到 1.0 之間
            audioSource.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        }
    }
}
