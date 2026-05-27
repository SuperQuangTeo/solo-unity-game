using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class EntityAudio : MonoBehaviour
{
    [Header("Sound Library")]
    public List<Sound> sounds;

    private AudioSource myAudioSource;

    void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string soundName)
    {
        Sound s = sounds.Find(sound => sound.name == soundName);

        if (s == null)
        {
            Debug.LogWarning($"The sound '{soundName}' doesn't exist {gameObject.name}");
            return;
        }
        myAudioSource.pitch = s.pitch;
        myAudioSource.volume = s.volume;

        myAudioSource.PlayOneShot(s.clip);
    }

    public void ExecuteAttackSound()
    {
        PlaySound("Attack");
    }
    public void ExecuteRunSound()
    {
        PlaySound("Run");
    }
    public void ExecuteDeathSound()
    {
        PlaySound("Death");
    }
    public void ExecuteHitSound()
    {
        PlaySound("Hit");
    }
    public void ExecuteCastSound()
    {
        PlaySound("Cast");
    }
    public void ExecuteCastAttackSound()
    {
        PlaySound("CastAttack");
    }
}