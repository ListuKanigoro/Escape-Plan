using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAlly : MonoBehaviour
{
    [Header("Prefab Assign")]
    public ParticleSystem explosionAlly;

    [Header("Boundary")]
    [SerializeField] float allyAttackRange;
    [SerializeField] float boundary;
    [SerializeField] float onSight_Ally;
    [SerializeField] float onSight_Enemy;

    [Header("Stats")]
    [SerializeField] float lookAtSpeed;
    [SerializeField] float walkSpeed;
    public int allyHealth = 100;
    public int allyDamage = 5;

    [Header("Category")]
    public bool allyIsAlive = true;

    AIEnemies[] enemies;
    AIEnemies target_enemy;
    Animator animator;
    Audio audioPlayer;
    ParticleSystem bloodPlayer;
    CharacterControl player;
    Transform ally;

    void Start()
    {
        ally = GameObject.FindWithTag("Player").transform;

        animator = GetComponent<Animator>();
        audioPlayer = FindObjectOfType<Audio>();
        enemies = FindObjectsOfType<AIEnemies>();
        player = FindObjectOfType<CharacterControl>();
    }

    void Update()
    {
        if(allyIsAlive == false) return;
        SelectTarget();
        Walk();
        Strike();
        DeadAlly();
    }

    void SelectTarget()
    {
        float minDist = Mathf.Infinity;
        target_enemy = null;
        
        foreach(AIEnemies enemy in enemies)
        {
            if(enemy.enemyIsAlive)
            {
                float distToEnemy = Vector3.Distance(enemy.transform.position, transform.position);
                if(distToEnemy < minDist)
                {
                    minDist = distToEnemy;
                    target_enemy = enemy;
                }
            }
        }
    }

    void Walk()
    {
        if(target_enemy != null && Vector3.Distance(target_enemy.transform.position, transform.position) < onSight_Enemy && Vector3.Distance(ally.transform.position, transform.position) > allyAttackRange)
        {
            MoveTowardsTarget(target_enemy.transform);
        }
        else if(player.playerIsAlive == true && Vector3.Distance(ally.transform.position, transform.position) < onSight_Ally && Vector3.Distance(ally.transform.position, transform.position) > boundary)
        {
            MoveTowardsTarget(ally);
        }
        else
        {
            //supaya berhenti ketika player menjauh. Atau di luar jangkauan onSight
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdling", true);
        }
    }

    void MoveTowardsTarget(Transform target)
    {
        Vector3 distToTarget = target.position - transform.position;

        animator.SetBool("isRunning", true);
        animator.SetBool("isIdling", false);

        if(animator.GetBool("isRunning") == true)
        {
            Quaternion lookTowards = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookTowards, Time.deltaTime*lookAtSpeed);
            transform.position += Time.deltaTime * walkSpeed * distToTarget.normalized;
        }
    }

    void Strike()
    {
        bool isAttacking = false;

        foreach(AIEnemies enemy in enemies)
        {
            Vector3 distEnemy = enemy.transform.position - transform.position;
            if(enemy.enemyIsAlive == true && distEnemy.magnitude<=allyAttackRange)
            {
                isAttacking = true;
            }
        }

        if(isAttacking == true)
        {
            float attackTrigger = Random.value;
            animator.SetBool("isShooting", true);
        }
        else
        {
            animator.SetBool("isShooting", false);
        }
        
    }

    public void HitAlly()
    {
        explosionAlly.Play();
        audioPlayer.HitPlayer();
    }

    void DeadAlly()
    {
        if(allyHealth <= 0)
        {
            allyIsAlive = false;
            animator.SetTrigger("isDead");
        }
    }

    void DamageAlly()
    {
        foreach(AIEnemies enemy in enemies)
        {
            Vector3 distEnemy = enemy.transform.position - transform.position;

            if(distEnemy.magnitude<=allyAttackRange+0.5f)
            {
                enemy.enemyHealth -= allyDamage;

                StartCoroutine(ChangeEnemyColor(enemy.GetComponent<Renderer>(), Color.red, 0.1f));

                float randomHit = Random.value;
                if(randomHit < 0.6)
                {
                    enemy.HitEnemy();
                }
            }

            if(bloodPlayer != null)
            {
                Destroy(bloodPlayer.gameObject, 2);
            }
        }
    }

    IEnumerator ChangeEnemyColor(Renderer enemyRenderer, Color newColor, float duration)
    {
        Color originalColor = enemyRenderer.material.color;
        enemyRenderer.material.color = newColor;
        yield return new WaitForSeconds(duration);
        enemyRenderer.material.color = originalColor;
    }

    void StopMoving()
    {
        walkSpeed = 0;
    }

    void BackToMove()
    {
        walkSpeed = 1.6f;
    }
}
