using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterControl : MonoBehaviour
{
    [Header("Prefab Assign")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Camera shootCamera;
    public ParticleSystem explosion;
    public Material hitMaterial;

    [Header("Chart")]
    [SerializeField] float jumpHeight = 5;
    [SerializeField] float speedWalk = 2;
    [SerializeField] float speedSideWalk = 1.5f;
    [SerializeField] float speedRun = 3;
    [SerializeField] float speedRotate = 50;
    [SerializeField] float shootRange = 25;

    [Header("Status")]
    public int playerShootDamage = 5;
    float currentSpeed = 1;
    private int ammoBullet = 200;
    public int playerHealth = 100;
    public bool playerIsAlive = true;
    bool isJumping = false;

    Animator animator;
    Audio audioPlayer;
    CameraFollow cameraFollow;
    Rigidbody rb;
    float timeCounter;
    bool isChangingMaterial = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioPlayer = FindObjectOfType<Audio>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(playerIsAlive == false) return;
        Shooting();
        UpdateHealth();
        Dead();
        ExitApp();
    }

    void FixedUpdate() 
    {
        Walk();
        Jump();
        Rotation();
    }

    public int PlayerHealth
    {
        get{return playerHealth;}
        set{playerHealth = Mathf.Clamp(value, 0, 500);}
    }

    void UpdateHealth()
    {
        if(PlayerHealth < 100)
        {
            StartCoroutine(AddTheHealth());
        }
    }

    IEnumerator AddTheHealth()
    {
        yield return new WaitForSeconds(5);
        if(PlayerHealth < 100)
        {
            timeCounter += Time.deltaTime;
            if(timeCounter >= 1)
            {
                PlayerHealth += 3;
                timeCounter -= 1f;
            } 
        }
    }

    void Walk()
    {
        if(playerIsAlive == false) return;

        float moveX = Input.GetAxis("Horizontal") * speedSideWalk;
        float moveZ = Input.GetAxis("Vertical") * speedWalk * currentSpeed;

        // Moving Forward
        if (moveZ > 0)
        {
            animator.SetBool("isWalkingForward", true);
            animator.SetBool("isWalkingBackward", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isIdling", false);
            animator.SetBool("isShooting", false);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalkingForward", false);
                currentSpeed = speedRun;
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalkingForward", true);
                currentSpeed = 1;
            }
        }
        else if (moveZ < 0) // Moving Backward
        {
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", true);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isIdling", false);
            animator.SetBool("isShooting", false);
        }
        else if (moveX < 0) // Moving Left
        {
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", false);
            animator.SetBool("isWalkingLeft", true);
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isIdling", false);
            animator.SetBool("isShooting", false);
        }
        else if (moveX > 0) // Moving Right
        {
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isWalkingRight", true);
            animator.SetBool("isIdling", false);
            animator.SetBool("isShooting", false);
        }
        else // Idle state
        {
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdling", true);
            currentSpeed = 1;
        }

        // Move the character
        Vector3 movement = transform.TransformDirection(new Vector3(moveX, 0, moveZ)) * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    void Jump()
    {
        if(playerIsAlive == false) return;

        if(Input.GetButtonDown("Jump") && isJumping == false)
        {
            animator.SetTrigger("isJumping");
            animator.SetBool("isIdling", false);
        }
    }

    void Rotation()
    {
        if(playerIsAlive == false) return;

        float rotateBody = Input.GetAxis("Mouse X") * Time.deltaTime * speedRotate;
        Quaternion turn = Quaternion.Euler(0, rotateBody, 0);
        rb.MoveRotation(rb.rotation * turn);
    }

    void Shooting()
    {
        if(playerIsAlive == false) return;
        
        if(Input.GetKey(KeyCode.Mouse0) && AmmoBullet > 0)
        {
            animator.SetBool("isShooting", true);
            animator.SetBool("isIdling", false);
            animator.SetBool("isWalkingForward", false);
            animator.SetBool("isWalkingBackward", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isRunning", false);
            speedWalk = 0;
            speedSideWalk = 0;
            audioPlayer.MachineGun();
            muzzleFlash.Play();
 
            timeCounter += Time.deltaTime;
            if(timeCounter > 0.1)
            {
                AmmoBullet -= 1;
                timeCounter -= 0.1f;
            }

            RaycastHit hit;
            if(Physics.Raycast(shootCamera.transform.position, shootCamera.transform.forward, out hit, shootRange))
            { 
                AIEnemies[] enemies = hit.transform.GetComponents<AIEnemies>();
                foreach(AIEnemies enemy in enemies)
                {   
                    timeCounter += Time.deltaTime;
                    if(timeCounter > 0.1)
                    {
                        enemy.enemyHealth -= playerShootDamage;

                        if(enemy.enemyIsAlive == true && isChangingMaterial == false)
                        {
                            StartCoroutine(ChangeMaterialTemporarily(enemy, hitMaterial, 0.05f)); //At first it work. But, eventually the enemy will not go back to its original material. How to fix it?
                        }
                        
                        timeCounter -= 0.1f;
                    }
                    enemy.isProvoked = true;

                    float randomHit = Random.value;
                    if(randomHit < 0.4)
                    {
                        enemy.HitEnemy();
                    }  
                }
            }
            else
            {
                return;
            }
            
            // Rotation for Y
            float rotateBody = Input.GetAxis("Mouse Y") * Time.deltaTime * speedRotate;
            Quaternion turn = Quaternion.Euler(-rotateBody, 0, 0);
            rb.MoveRotation(rb.rotation * turn);

        }
        else if(Input.GetKeyUp(KeyCode.Mouse0) || AmmoBullet <= 0)
        {
            animator.SetBool("isShooting", false);
            speedWalk = 2;
            speedSideWalk = 0.9f;
            audioPlayer.StopMachineGun();

            // reset rotation
            Vector3 currentRotation = transform.rotation.eulerAngles;;
            currentRotation.x = 0;
            currentRotation.z = 0;
            transform.rotation = Quaternion.Euler(currentRotation);
        }
    }

    public int AmmoBullet
    {
        get{return ammoBullet;}
        set{ammoBullet = Mathf.Clamp(value, 0, 500);}
    }

    IEnumerator ChangeMaterialTemporarily(AIEnemies enemy, Material newMaterial, float duration)
    {
        isChangingMaterial = true;
        Renderer renderer = enemy.GetComponentInChildren<Renderer>();
        Material originalMaterial = renderer.material;
        renderer.material = newMaterial;
        yield return new WaitForSeconds(duration);
        renderer.material = originalMaterial;
        isChangingMaterial = false;
    }

    void Dead()
    {
        if(PlayerHealth <= 0)
        {
            animator.SetTrigger("isDead");
            playerIsAlive = false;
            StartCoroutine(RestartLevel());
        }
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSecondsRealtime(3);
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    // ASSIGN METHOD TO ANIMATION
    
    void StartJumping()
    {
        rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
        isJumping = true;
    }

    void OnJumping()
    {
        isJumping = false;
        audioPlayer.HeavyFootstep();
    }

    void CameraShakeFoot()
    {
        StartCoroutine(CameraShake());
        audioPlayer.HeavyFootstep();
    }

    IEnumerator CameraShake()
    {
        float elapsed = 0.0f;

        CinemachineVirtualCamera virtualCamera = cameraFollow.GetComponent<CinemachineVirtualCamera>();
        CinemachineFramingTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        while(elapsed < 0.1f)
        {
            transposer.m_TrackedObjectOffset.x = 0 + Random.Range(-0.6f,0.6f);
            transposer.m_TrackedObjectOffset.y = 1.54f + Random.Range(-0.6f,0.6f);
            elapsed += Time.deltaTime;
            yield return null; // perlu untuk menunggu frame selanjutnya
        }
    }

    void WalkingFootstepAudio()
    {
        audioPlayer.Footstep();
    }

    // Exit
    void ExitApp()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
