using System.Collections;
using UnityEngine;

public class SpikeResetSystem : MonoBehaviour
{
    [Header("Reset Settings")]
    [SerializeField] private Vector3 respawnPoint;
    [SerializeField] private float deathDelay = 0.2f; // ลด delay เพื่อให้เร็วขึ้น
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private AudioClip deathSound;
    
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Animator animator;
    private Movement movement;
    private bool isDead = false;

    private static readonly int DieTrigger = Animator.StringToHash("Die");

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        movement = GetComponent<Movement>();
        
        if (respawnPoint == Vector3.zero)
        {
            respawnPoint = transform.position;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike") && !isDead)
        {
            Die();
        }
    }
    
    private void Die()
    {
        isDead = true;
        movement.canMove = false;
        
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        rb.gravityScale = 0;

        // เพิ่มความสำคัญในการรัน Animation Die
        if (animator != null)
        {
            animator.CrossFade(DieTrigger, 0f, -1, 0f); // รัน Animation "Die" ทันที
        }
        
        if (deathEffect != null)
        {
            deathEffect.Play();
        }
        
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        // รอให้ Animation Die ทำงาน
        yield return new WaitForSeconds(deathDelay);

        ResetPlayer();
    }

    private void ResetPlayer()
    {
        transform.position = respawnPoint;

        if (animator != null)
        {
            animator.ResetTrigger(DieTrigger);
        }

        rb.isKinematic = false;
        rb.gravityScale = 3;
        movement.canMove = true;
        isDead = false;
    }
    
    public void UpdateRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
}
