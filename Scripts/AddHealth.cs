using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealth : MonoBehaviour
{
    CharacterControl player;
    Audio audioPlayer;

    private void Start() 
    {
        player = FindObjectOfType<CharacterControl>();
        audioPlayer = FindObjectOfType<Audio>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player.PlayerHealth += 50;
            audioPlayer.Health();
            player.explosion.Stop();
            Destroy(gameObject);
        }
    }
}
