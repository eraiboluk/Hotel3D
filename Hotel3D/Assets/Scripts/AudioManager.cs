using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public int playCount = 0;
    public int stopCount = 0;

    public static AudioManager instance;

    void Awake () {

        if(instance == null)
            instance = this;
        else{

            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    public void Toggle (string name, int a){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
            return;
        if(a == 0){
            s.source.Stop();
            stopCount++;
        }
        else{
            s.source.Play();
            playCount++;
        } 
    }
    public bool isPlay (string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
            return false;
        if(s.source.isPlaying)
            return true;
        else
            return false;
    }
}
