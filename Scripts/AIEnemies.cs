using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemies : MonoBehaviour
{
    [Header("Prefab Assign")]
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletSpawnPoint;

    [Header("Boundary")]
    [SerializeField] float onSight_Ally;
    [SerializeField] float onSight_Player;
    public float avoidBoundary = 5;
    public float attackRangeEnemy = 2f;

    [Header("Stats")]
    [SerializeField] float lookAtSpeed;
    [SerializeField] float walkSpeed;
    public int enemyHealth = 100;
    public int enemyDamage = 5;
    public int enemyDamageProjectile = 2;
    
    [Header("Category")]
    public bool enemyIsAlive = true;
    public bool isProvoked = false;
    public bool longRangedEnemy = false;

    AIAlly[] allies;
    AIAlly target_Ally;
    Animator animator;
    Audio audioPlayer;
    
    UnityEngine.AI.NavMeshAgent agent;
    ParticleSystem bloodPlayer;
    CharacterControl player;
    Rigidbody fireRb;
    Transform target_Player;
    bool triggerExplosion = false;
    public bool isMoving = false;
    
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        allies = FindObjectsOfType<AIAlly>();
        animator = GetComponent<Animator>();
        audioPlayer = FindObjectOfType<Audio>();
        
        player = FindObjectOfType<CharacterControl>();
        target_Player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if(enemyIsAlive == false) return;
        SelectTarget();
        Strike();
        Walk();
        DeadEnemy();

        Avoid();
    }

    void SelectTarget()
    {
        float minDist = Mathf.Infinity;
        target_Ally = null;

        foreach(AIAlly ally in allies)
        {
            if(ally.allyIsAlive)
            {
                float distToAlly = Vector3.Distance(ally.transform.position, transform.position);
                if(distToAlly < minDist)
                {
                    minDist = distToAlly;
                    target_Ally = ally;
                }
            }
        }
    }

    void Walk()
    {
        if(target_Ally != null && Vector3.Distance(target_Ally.transform.position, transform.position) < onSight_Ally)
        {
            MoveTowardsTarget(target_Ally.transform);
        }
        else if((player.playerIsAlive == true && Vector3.Distance(target_Player.transform.position, transform.position) < onSight_Player) || isProvoked == true)
        {
            MoveTowardsTarget(target_Player);
        }
        else
        {
            animator.SetBool("isWalk", false);
            isMoving = false; 
        }
    }

    void MoveTowardsTarget(Transform target)
    {
        Vector3 distToTarget = target.position - transform.position;
        if(distToTarget.magnitude > attackRangeEnemy)
        {
            animator.SetBool("isWalk", true);
            isMoving = true;

            if(animator.GetBool("isWalk") == true)
            {
                Quaternion lookTowards = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookTowards, Time.deltaTime*lookAtSpeed);
                transform.position += Time.deltaTime * walkSpeed * distToTarget.normalized;
            }
        }
        else
        {
            animator.SetBool("isWalk", false);
            isMoving = false;
        }
    }

    void Strike()
    {
        bool isAttacking = false;
        foreach(AIAlly ally in allies)
        {
            Vector3 distToPlayer = target_Player.transform.position - transform.position;
            Vector3 distToAlly = ally.transform.position - transform.position;
            
            if ((isMoving == false && player.playerIsAlive == true && distToPlayer.magnitude <= attackRangeEnemy) || (isMoving == false && ally.allyIsAlive == true && distToAlly.magnitude <= attackRangeEnemy))
            {
                isAttacking = true;

                if(ally.allyIsAlive == true && distToAlly.magnitude <= attackRangeEnemy)
                {
                    transform.LookAt(ally.transform);
                }
                else if(player.playerIsAlive == true && distToPlayer.magnitude <= attackRangeEnemy)
                {
                    transform.LookAt(player.transform);
                }
            }
        }
        if (isAttacking)
        {
            if(longRangedEnemy == true)
            {
                animator.SetBool("isShooting1", true);
            }
            else
            {
                animator.SetTrigger("isAttack1");
            }
        }
        else
        {
            if(longRangedEnemy == true)
            {
                animator.SetBool("isShooting1", false);
            }
            else
            {
                animator.ResetTrigger("isAttack1");
            }
        }
    }

    public void HitEnemy()
    {
        animator.SetTrigger("isHit");
    }

    void StopMoving()
    {
        walkSpeed = 0;
    }

    void BackToMove()
    {
        walkSpeed = 6;
    }

    void Flee(Vector3 location, float fleeDistance)
    {
        Vector3 fleeDirection = (transform.position - location).normalized;
        agent.SetDestination(transform.position + fleeDirection * fleeDistance);
    }

    void Avoid()
    {
        if (longRangedEnemy == true)
        {
            Vector3 avoidDistance = player.transform.position - this.transform.position;
            float lookAhead = 0f; // Initialize lookAhead to 0

            if (avoidDistance.magnitude <= avoidBoundary)
            {
                // Calculate lookAhead only when within avoidBoundary
                lookAhead = avoidDistance.magnitude;
            }

            Flee(player.transform.position, lookAhead);
        }
    }

    void DeadEnemy()
    {
        if(enemyHealth <= 0)
        {
            enemyIsAlive = false;
            animator.SetTrigger("isDead");
        }
    }

    void DamageEnemy()
    {
        foreach(AIAlly ally in allies)
        {
            Vector3 distToAlly = ally.transform.position - transform.position;

            if(distToAlly.magnitude <= attackRangeEnemy + 0.3f)
            {
                ally.allyHealth -= enemyDamage;
                ally.HitAlly();
            }

            if(bloodPlayer != null)
            {
                Destroy(bloodPlayer.gameObject, 2);
            }
        }

        Vector3 distToPlayer = target_Player.transform.position - transform.position;

        if(distToPlayer.magnitude <= attackRangeEnemy + 0.3)
        {
            player.PlayerHealth -= enemyDamage;

            player.explosion.Play();
            StartCoroutine(StopExplosion(0.5f));
            
            audioPlayer.HitPlayer();
        }

        if(bloodPlayer != null)
        {
            Destroy(bloodPlayer.gameObject, 2);
        }
    }

    IEnumerator StopExplosion(float duration)
    {
        yield return new WaitForSeconds(duration);
        player.explosion.Stop();
    }

    void FireElectricity()
    {
        audioPlayer.LaserGun();

        Vector3 fireLocation = new(bulletSpawnPoint.transform.position.x, bulletSpawnPoint.transform.position.y, bulletSpawnPoint.transform.position.z);
        Quaternion fireRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        GameObject fire = Instantiate(bullet, fireLocation, fireRotation);

        fireRb = fire.GetComponent<Rigidbody>();
        fireRb.velocity = transform.forward * 20f;

        Destroy(fire.gameObject, 2);
    }
}
