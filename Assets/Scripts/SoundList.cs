using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundList : MonoBehaviour
{
    public List<AudioClip> ClipsList;
    private AudioSource AudioSource;
    
    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void PlayRandom()
    {
        AudioSource.PlayOneShot(ClipsList[Random.Range(0, ClipsList.Count)]);
    }
}
