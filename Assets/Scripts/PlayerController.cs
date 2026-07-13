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
}