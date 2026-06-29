using UnityEngine;
using System.Collections;
using FirstGearGames.SmoothCameraShaker;

public class GameManager : MonoBehaviour
{
    [Header("Character")]
    public GameObject player;
    public SpriteRenderer playerRenderer;
    public Transform startPosition;

    [Header("Effects")]
    public ShakeData shakeData;
    public ParticleSystem dustParticles;

    [Header("Status")]
    public bool isBusy = false;

    public void SelectHidingSpot(HidingSpot hidingSpot)
    {
        if (isBusy) return;

        StartCoroutine(HideRoutine(hidingSpot));
    }

    IEnumerator HideRoutine(HidingSpot hidingSpot)
    {
        isBusy = true;

        player.transform.position = hidingSpot.transform.position;
        
        playerRenderer.sortingOrder = -1;

        Debug.Log("Player is hiding under " + hidingSpot.spotName);
        yield return new WaitForSeconds(1f);

        Debug.Log("QUAKE!");
        CameraShakerHandler.Shake(shakeData);
        if(dustParticles != null) dustParticles.Play();
        
        yield return new WaitForSeconds(5f); 

        if (hidingSpot.isSafe)
        {
            Debug.Log("POPUP: You Survived! " + hidingSpot.spotName + " successfully held up against the debris");
        }
        else
        {
            Debug.Log("POPUP: DANGER! " + hidingSpot.spotName + " is fragile and collapsed!");
        }

        yield return new WaitForSeconds(3f); 
        
        player.transform.position = startPosition.position;
        playerRenderer.sortingOrder = 5; 
        
        Debug.Log("Please select another hiding spot...");
        isBusy = false; 
    }
}