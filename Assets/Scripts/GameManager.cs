using UnityEngine;
using System.Collections;
using FirstGearGames.SmoothCameraShaker;

public class GameManager : MonoBehaviour
{
    [Header("Character")]
    public GameObject player;
    public PlayerController playerController;
    public SpriteRenderer playerRenderer;
    public Transform startPosition;

    [Header("Effects")]
    public ShakeData shakeData;
    public ParticleSystem dustParticles;

    [Header("Status")]
    public bool isBusy = false;
    private Vector3 originalPlayerScale;

    [Header("UI")]
    public UIManager uiManager;

    void Start()
    {
        if (player != null)
        {
            originalPlayerScale = player.transform.localScale;
        }
    }

    public void SelectHidingSpot(HidingSpot hidingSpot)
    {
        if (isBusy || hidingSpot.hasSelected) return;

        StartCoroutine(HideRoutine(hidingSpot));
    }

    IEnumerator HideRoutine(HidingSpot hidingSpot)
    {
        isBusy = true;
        hidingSpot.hasSelected = true;

        if (hidingSpot.hidingSpot != null)
        {
            player.transform.position = hidingSpot.hidingSpot.position;
            player.transform.localScale = hidingSpot.hidingSpot.localScale;
        }
        else
        {
            player.transform.position = hidingSpot.transform.position;
        }
        playerRenderer.sortingOrder = 3;

        if(playerController != null) playerController.SetHiding(true);

        Debug.Log("Player is hiding at " + hidingSpot.spotName);
        yield return new WaitForSeconds(1f);

        Debug.Log("QUAKE!");
        CameraShakerHandler.Shake(shakeData);
        if(dustParticles != null) dustParticles.Play();

        yield return new WaitForSeconds(2f);
        hidingSpot.OnQuakeEffect();

        if (!hidingSpot.IsSafe && playerController != null)
        {
            playerController.PlayHurtAnimation(hidingSpot.PlayerHurtTrigger);
        }

        yield return new WaitForSeconds(2f);

        if (uiManager != null) 
        {
            uiManager.ShowResult(hidingSpot.resultMessage, hidingSpot.IsSafe);
        }

        yield return new WaitForSeconds(2f); 
        
        player.transform.position = startPosition.position;
        player.transform.localScale = originalPlayerScale;
        playerRenderer.sortingOrder = 10;

        if (playerController != null) playerController.SetHiding(false);

        CheckGameEnd();
    }

    void CheckGameEnd()
    {
        HidingSpot[] allSpots = FindObjectsByType<HidingSpot>(FindObjectsSortMode.None);
        bool allDone = true;

        foreach (var spot in allSpots)
        {
            if (!spot.hasSelected)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            Debug.Log("GAME OVER: All hiding spots has being tested! Simulation ended.");
            isBusy = true;
        }
        else
        {
            Debug.Log("Please select another hiding spot...");
            isBusy = false;
        }
    }
}