using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMan : MonoBehaviour
{
    // Fields
    public static AudioMan instance;
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        // Ensures only 1 instance of audio manager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Creates sound objects
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
            sound.source.ignoreListenerPause = sound.ignoreListenerPause;
        }
    }

    // Plays any audio clip
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
        }
    }

    // Stops any audio clip
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null && s.source.isPlaying == true)
        {
            s.source.Stop();
        }
    }

    // Plays single instance of the sound
    public void PlaySingleInstance(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null && s.source.isPlaying == false)
        {
            s.source.Play();
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;

    public bool ignoreListenerPause;

    [HideInInspector]
    public AudioSource source;
}
