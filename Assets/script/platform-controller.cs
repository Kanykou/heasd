using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float standingTime = 1.5f;
    public float respawnTime = 3f;

    [Header("Animations")]
    public string disappearTrigger = "Disappear";
    public string appearTrigger = "Appear";
    
    private Animator animator;
    private bool isDisappearing = false;
    private Vector3 startPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDisappearing && collision.gameObject.CompareTag("Player"))
        {
            isDisappearing = true;
            Invoke("StartDisappearing", standingTime);
        }
    }

    void StartDisappearing()
{
    if (animator != null)
    {
        animator.SetTrigger(disappearTrigger);
        
        // คำนวณระยะเวลาของอนิเมชั่น
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        
        // รอจนกว่าอนิเมชั่นจะจบก่อนปิด GameObject
        Invoke("DeactivatePlatform", animationLength);
    }
    else
    {
        DeactivatePlatform();
    }
}

void DeactivatePlatform()
{
    isDisappearing = false;
    gameObject.SetActive(false);
    Invoke("Respawn", respawnTime);
}

    void Respawn()
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
        
        if (animator != null)
        {
            animator.SetTrigger(appearTrigger);
        }
    }
}