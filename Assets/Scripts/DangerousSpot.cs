using UnityEngine;
using DG.Tweening;

public class DangerousSpot : HidingSpot
{
    public enum DangerType { Break, Fall }

    [Header("Danger Settings")]
    public DangerType dangerType;

    public override bool IsSafe => false;

    public override void OnQuakeEffect()
    {
        base.OnQuakeEffect();
        TriggerCollapse();
    }

    private void TriggerCollapse()
    {
        Debug.Log("DANGER! " + spotName + " break or falls!");

        if (dangerType == DangerType.Break)
        {
            gameObject.SetActive(false); 
        }
        else if (dangerType == DangerType.Fall)
        {
            transform.DORotate(new Vector3(0, 0, 10f), 0.5f);
            
            transform.DOMoveY(transform.position.y - 1f, 0.5f);
        }
    }
}
