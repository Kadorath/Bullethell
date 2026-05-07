using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        audioSource.PlayOneShot(clip, volume); 
    }
}
