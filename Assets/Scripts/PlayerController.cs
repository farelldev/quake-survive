using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    [Header("Manual Movement Settings")]
    public float moveSpeed = 4f;
    private bool canMoveManually = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (canMoveManually)
        {
            HandleManualMovement();
        }
    }

    public void EnableManualControl(bool enable)
    {
        canMoveManually = enable;
        if (!enable && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            SetRunning(false);
        }
    }

    private void HandleManualMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }

        if (moveDirection != Vector2.zero)
        {
            SetRunning(true);

            if (moveX > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (moveX < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            SetRunning(false);
        }
    }

    public void TriggerAnim(string trigger)
    {
        if (anim != null)
        {
            anim.SetTrigger(trigger);
        }
    }

    public void SetHiding(bool isHiding)
    {
        if(anim != null) anim.SetBool("isHiding", isHiding);
    }

    public void SetWalking(bool isWalking)
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", isWalking); 
        }
    }

    public void SetScared(bool isScared)
    {
        if (anim != null)
        {
            anim.SetBool("isScared", isScared); 
        }
    }

    public void SetRunning(bool isRunning)
    {
        if (anim != null)
        {
            anim.SetBool("isRunning", isRunning); 
        }
    }
}