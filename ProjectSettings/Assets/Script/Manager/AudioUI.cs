using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUI : MonoBehaviour
{
    [SerializeField] AudioClip hover, click;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Check if audioSource is null
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from this GameObject.");
        }
    }

    public void SoundOnHover()
    {
        // Ensure audioSource is not null and hover clip is assigned
        if (audioSource != null && hover != null)
        {
            audioSource.PlayOneShot(hover);
        }
        else
        {
            Debug.LogError("AudioSource or hover AudioClip is not assigned.");
        }
    }

    public void SoundOnClick()
    {
        // Ensure audioSource is not null and click clip is assigned
        if (audioSource != null && click != null)
        {
            audioSource.PlayOneShot(click);
        }
        else
        {
            Debug.LogError("AudioSource or click AudioClip is not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
