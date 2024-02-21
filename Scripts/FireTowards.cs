using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTowards : MonoBehaviour
{
    AIEnemies enemy;
    Audio audioPlayer;
    CharacterControl player;

    void Start()
    {
        audioPlayer = FindObjectOfType<Audio>();
        enemy = FindObjectOfType<AIEnemies>();
        player = FindObjectOfType<CharacterControl>();
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player.PlayerHealth -= enemy.enemyDamageProjectile;
            
            player.explosion.Play();
            StartCoroutine(StopExplosion(0.5f));

            audioPlayer.HitPlayer();
        }
        Destroy(gameObject);
    }
    
    IEnumerator StopExplosion(float duration)
    {
        yield return new WaitForSeconds(duration);
        player.explosion.Stop();
    }
}
