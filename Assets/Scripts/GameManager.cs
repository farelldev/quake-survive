using UnityEngine;
using System.Collections;
using FirstGearGames.SmoothCameraShaker;
using DG.Tweening;

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

    [Header("Status (State Machine)")]
    public GameState currentState = GameState.Intro;
    public enum GameState { Intro, Idle, Quake, Over }

    [Header("Intro Settings")]
    public GameObject blackScreen;
    public SpriteRenderer blacksprite;
    public Transform introSpawnPointStart;
    public Transform introSpawnPointEnd;
    private Vector3 originalPlayerScale;

    [Header("UI")]
    public UIManager uiManager;

    [Header("Dialogue System")]
    public DialogueManager dialogueManager;
    [TextArea(2, 5)] public string[] initialText;
    [TextArea(2, 5)] public string[] quakeText;
    [TextArea(2, 5)] public string[] chooseText;
    [TextArea(2, 5)] public string[] chooseAgainText;

    private bool isDialogueActive = false;

    void Start()
    {
        if (player != null)
        {
            originalPlayerScale = player.transform.localScale;
        }

        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        currentState = GameState.Intro;

        // BLACK SCREEN 
        blackScreen.SetActive(true);
        Color c = blacksprite.color;
        c.a = 1f;
        blacksprite.color = c;
        blacksprite.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.5f); 
        blackScreen.SetActive(false);        

        // 1. SET CHARACTER POSITION
        if (introSpawnPointStart != null){
            player.transform.position = introSpawnPointStart.position;
            player.transform.localScale = introSpawnPointStart.localScale;
            }
        playerRenderer.sortingOrder = 3;

        // Initial dialogue
        if (dialogueManager != null && initialText.Length > 0)
        {
            isDialogueActive = true;
            dialogueManager.StartDialogue(initialText, false, () => { isDialogueActive = false; });
        }

        // Character Walking
        if (playerController != null) playerController.SetWalking(true);
        player.transform.DOMove(introSpawnPointEnd.position, 5f).SetEase(Ease.Linear);
        
        yield return new WaitForSeconds(4.5f); 
        yield return new WaitUntil(() => !isDialogueActive);

        // 2. EARTHQUAKE INTRO
        CameraShakerHandler.Shake(shakeData);
        if(dustParticles != null) dustParticles.Play();

        yield return new WaitForSeconds(0.5f); 

        // Character Is Scared
        if (playerController != null) playerController.SetScared(true);
        if (playerController != null) playerController.SetWalking(false);

        // Intro Quake Dialogue
        if (dialogueManager != null && quakeText.Length > 0)
        {
            isDialogueActive = true;
            dialogueManager.StartDialogue(quakeText, false, () => { isDialogueActive = false; });
            yield return new WaitUntil(() => !isDialogueActive);
        }

        yield return new WaitForSeconds(5f);
        
        // 3. MOVE TO FRONT OF SCREEN
        if (playerController != null) playerController.SetScared(false);
        if (playerController != null) playerController.SetHiding(false);

        player.transform.position = startPosition.position;
        playerRenderer.sortingOrder = 10;

        // Appear from Below
        Vector3 posisiBawah = startPosition.position;
        posisiBawah.y -= 10f;
        player.transform.position = posisiBawah;
        player.transform.DOMove(startPosition.position, 0.5f).SetEase(Ease.OutExpo);
        
        // Choose Spot Dialogue
        if (dialogueManager != null && chooseText.Length > 0)
        {
            isDialogueActive = true;
            dialogueManager.StartDialogue(chooseText, true, null);
        }

        currentState = GameState.Idle;
    }

    public void SelectHidingSpot(HidingSpot hidingSpot)
    {
        if (currentState != GameState.Idle || hidingSpot.hasSelected) return;
        if (dialogueManager != null)
        {
            dialogueManager.CloseAllDialogues();
        }

        StartCoroutine(HideRoutine(hidingSpot));
    }

    IEnumerator HideRoutine(HidingSpot hidingSpot)
    {
        currentState = GameState.Quake;
        hidingSpot.hasSelected = true;

        player.transform.DOKill();

        // 1. CHARACTER TELEPORT
        if (hidingSpot.obstruction != null)
            hidingSpot.obstruction.DOFade(0.3f, 0.5f);

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

        // 2. QUAKE
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

        // 3. POPUP APPEAR
        if (uiManager != null) 
        {
            uiManager.ShowResult(hidingSpot.resultMessage, hidingSpot.IsSafe);
        }

        yield return new WaitForSeconds(2f); 
        
        // 4. CHARACTER RETURN
        player.transform.position = startPosition.position;
        player.transform.localScale = originalPlayerScale;
        player.transform.DOScaleY(transform.localScale.y * 1.03f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        playerRenderer.sortingOrder = 10;

        if (playerController != null) playerController.SetHiding(false);
        if (hidingSpot.obstruction != null)
            hidingSpot.obstruction.DOFade(1f, 0.5f);

        Vector3 posisiBawah = startPosition.position;
        posisiBawah.y -= 10f;
        player.transform.position = posisiBawah;

        player.transform.DOMove(startPosition.position, 0.5f).SetEase(Ease.OutExpo);

        yield return new WaitForSeconds(0.7f);

        // 5. CHECK ALL SPOTS
        HidingSpot[] allSpots = FindObjectsByType<HidingSpot>(FindObjectsSortMode.None);
        bool allDone = true;
        foreach (HidingSpot spot in allSpots)
        {
            if (!spot.hasSelected)
            {
                allDone = false;
                break;
            }
        }

        if (allDone)
        {
            Debug.Log("All spot have been selected.");
            currentState = GameState.Over; 
        }
        else
        {
            if (dialogueManager != null && chooseAgainText.Length > 0)
            {
                isDialogueActive = true;
                dialogueManager.StartDialogue(chooseAgainText, true, null);
            }
            
            currentState = GameState.Idle; 
        }
    }
}