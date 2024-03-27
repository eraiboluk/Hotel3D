using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackroundSounds : MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(startPlaying());
    }

    void Play()
    {
        ShuffleSounds();

        audioSource.PlayOneShot(sounds[0]);

        Invoke("RepeatAtEndOfSound", sounds[0].length);
    }

    void RepeatAtEndOfSound()
    {
        int index = Random.Range(0, sounds.Length);

        audioSource.PlayOneShot(sounds[index]);

        Invoke("RepeatAtEndOfSound", sounds[index].length);
    }

    void ShuffleSounds()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            AudioClip temp = sounds[i];
            int randomIndex = Random.Range(i, sounds.Length);
            sounds[i] = sounds[randomIndex];
            sounds[randomIndex] = temp;
        }
    }
    IEnumerator startPlaying(){
        yield return new WaitUntil(() => Time.timeScale == 1);
        Play();
    }
}
