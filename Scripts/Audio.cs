using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    [Header("Heavy Footstep")]
    [SerializeField] AudioClip heavyFootstep;
    [SerializeField] [Range(0,1)] float volumeHeavyFootstep;

    [Header("Footstep")]
    [SerializeField] AudioClip footstep;
    [SerializeField] [Range(0,1)] float volumeFootstep;

    [Header("Machine Gun")]
    [SerializeField] AudioClip machineGun;
    [SerializeField] [Range(0,1)] float volumeMachineGun;

    [Header("Laser Gun")]
    [SerializeField] AudioClip laserGun;
    [SerializeField] [Range(0,1)] float volumeLaserGun;

    [Header("Hit Player")]
    [SerializeField] AudioClip hitPlayer;
    [SerializeField] [Range(0,1)] float volumeHitPlayer;

    [Header("Reload")]
    [SerializeField] AudioClip reload;
    [SerializeField] [Range(0,1)] float volumeReload;

    [Header("Health")]
    [SerializeField] AudioClip health;
    [SerializeField] [Range(0,1)] float volumeHealth;

    AudioSource audioSource;

    void Start() 
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void HeavyFootstep()
    {
        PlaySound(heavyFootstep, volumeHeavyFootstep);
    }

    public void Footstep()
    {
        PlaySound(footstep, volumeFootstep);
    }

    public void MachineGun()
    {
        if (audioSource.clip == machineGun && audioSource.isPlaying)
        {
            // It's already playing, no need to start again
            return;
        }

        audioSource.clip = machineGun;
        audioSource.volume = volumeMachineGun;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopMachineGun()
    {
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
    }

    public void HitPlayer()
    {
        PlaySound(hitPlayer, volumeHitPlayer);
    }

    public void LaserGun()
    {
        PlaySound(laserGun, volumeLaserGun);
    }

    public void Reload()
    {
        PlaySound(reload, volumeReload);
    }

    public void Health()
    {
        PlaySound(health, volumeHealth);
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
        }
    }
}
