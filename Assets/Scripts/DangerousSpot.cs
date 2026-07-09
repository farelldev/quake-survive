using UnityEngine;

public class DangerousSpot : HidingSpot
{
    public override bool IsSafe => false;

    public override void OnQuakeEffect()
    {
        base.OnQuakeEffect();
        TriggerCollapse();
    }

    private void TriggerCollapse()
    {
        Debug.Log("DANGER! " + spotName + " collapses and falls!");
    }
}
