using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetHiding(bool isHiding)
    {
        if(anim != null) anim.SetBool("isHiding", isHiding);
    }

    public void PlayHurtAnimation(string trigger)
    {
        if (anim != null)
        {
            anim.SetTrigger(trigger);
        }
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

    public void triggerStanding(string trigger)
    {
        if (anim != null)
        {
            anim.SetTrigger(trigger);
        }
    }
}