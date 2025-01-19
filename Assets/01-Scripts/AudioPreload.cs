using UnityEngine;

public class AudioPreload : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip.LoadAudioData();
    }

}
