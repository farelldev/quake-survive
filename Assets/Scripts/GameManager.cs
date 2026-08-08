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
    [TextArea(2, 5)] public string[] quakeSubdsideText;
    private bool isDialogueActive = false;
    private int spotSelected = 0;
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

        // Character Walking
        float walkDuration = 7f;
        if (playerController != null) playerController.SetWalking(true);
        player.transform.DOMove(introSpawnPointEnd.position, walkDuration).SetEase(Ease.Linear);

        // "Pada suatu pagi yang cerah, ..."
        if (dialogueManager != null && initialText.Length > 0)
        {
            isDialogueActive = true;
            string[] firstText = new string[] { initialText[0] }; 
            dialogueManager.StartDialogue(firstText, false, () => { isDialogueActive = false; });
        }

        float timer1 = 0f;
        float maxWait1 = 5f;

        while (timer1 < maxWait1)
        {
            timer1 += Time.deltaTime;
            yield return null;
        }

        if (isDialogueActive)
        {
            if (dialogueManager != null) dialogueManager.CloseAllDialogues();
            isDialogueActive = false;
        }

        // "Namun, tiba-tiba..."
        if (dialogueManager != null && initialText.Length > 1)
        {
            isDialogueActive = true;
            string[] secondText = new string[] { initialText[1] };
            dialogueManager.StartDialogue(secondText, false, () => { isDialogueActive = false; });
        }

        float timer2 = 0f;
        float maxWait2 = 1.5f;

        while (timer2 < maxWait2)
        {
            timer2 += Time.deltaTime;
            yield return null;
        }

        if (isDialogueActive)
        {
            if (dialogueManager != null) dialogueManager.CloseAllDialogues();
            isDialogueActive = false;
        }

        // 2. EARTHQUAKE INTRO
        CameraShakerHandler.Shake(shakeData);
        if(dustParticles != null) dustParticles.Play();

        yield return new WaitForSeconds(0.5f); 

        // Character Is Scared
        if (playerController != null) playerController.SetScared(true);
        if (playerController != null) playerController.SetWalking(false);

        // Intro Quake Dialogue
        float shakeInterval = 4.5f; 
        float shakeTimer = 0f;
        
        // Jeda waktu menunggu SETELAH teks selesai diketik (Misal: 1.5 detik)
        float maxWaitAfterTyping = 1.5f; 

        if (dialogueManager != null && quakeText != null && quakeText.Length > 0)
        {
            for (int i = 0; i < quakeText.Length; i++)
            {
                isDialogueActive = true;
                
                string[] currText = new string[] { quakeText[i] };
                dialogueManager.StartDialogue(currText, false, () => { isDialogueActive = false; });

                float postTypingTimer = 0f;

                while (isDialogueActive)
                {
                    shakeTimer += Time.deltaTime;
                    if (shakeTimer >= shakeInterval)
                    {
                        CameraShakerHandler.Shake(shakeData);
                        dustParticles.Play();
                        shakeTimer = 0f;
                    }

                    if (!dialogueManager.isTyping) 
                    {
                        postTypingTimer += Time.deltaTime;
                        
                        if (postTypingTimer >= maxWaitAfterTyping)
                        {
                            dialogueManager.CloseAllDialogues();
                            isDialogueActive = false;
                        }
                    }

                    yield return null; 
                }
            }
        }

        CameraShakerHandler.FadeOut(1f);
        if (dustParticles != null) dustParticles.Stop();
        yield return new WaitForSeconds(0.5f);
        if (playerController != null) playerController.SetScared(false);
        
        // 3. MOVE TO FRONT OF SCREEN
        if (playerController != null) playerController.SetScared(false);
        if (playerController != null) playerController.SetHiding(false);

        player.transform.position = startPosition.position;
        playerRenderer.sortingOrder = 10;

        // Appear from Below
        Vector3 belowPosition = startPosition.position;
        belowPosition.y -= 10f;
        player.transform.position = belowPosition;
        player.transform.DOMove(startPosition.position, 0.5f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(0.5f);
        
        // Choose Spot Dialogue
        if (dialogueManager != null && chooseText.Length > 0)
        {
            isDialogueActive = true;
            string[] currText = new string[] { chooseText[0] + " (" + spotSelected + "/4)" };
            dialogueManager.StartDialogue(currText, true, null);
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

        spotSelected++;

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
        
        // 4. CHECK ALL SPOTS
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

        // 5. DECISION BRANCH
        if (allDone)
        {
            Debug.Log("Semua spot sudah dites. Masuk ke Fase Outro dari tempat sembunyi.");
            StartCoroutine(OutroRoutine());
        }
        else
        {
            player.transform.position = startPosition.position;
            player.transform.localScale = originalPlayerScale;
            player.transform.DOScaleY(transform.localScale.y * 1.03f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            playerRenderer.sortingOrder = 10;

            if (playerController != null) playerController.SetHiding(false);
            if (hidingSpot.obstruction != null)
                hidingSpot.obstruction.DOFade(1f, 0.5f);

            Vector3 belowPosition = startPosition.position;
            belowPosition.y -= 10f;
            player.transform.position = belowPosition;

            player.transform.DOMove(startPosition.position, 0.5f).SetEase(Ease.OutExpo);

            yield return new WaitForSeconds(0.7f);

            if (dialogueManager != null && chooseAgainText.Length > 0)
            {
                isDialogueActive = true;
                string[] currText = new string[] { chooseAgainText[0] + " (" + spotSelected + "/4)" };
                dialogueManager.StartDialogue(currText, true, () => { isDialogueActive = false; });
            }
            
            currentState = GameState.Idle; 
        }
    }

    IEnumerator OutroRoutine()
    {
        currentState = GameState.Over;
        
        yield return new WaitForSeconds(1f);

        if (playerController != null) 
        {
            playerController.triggerStanding("standing");
            playerController.SetHiding(false);
        }

        if (dialogueManager != null && quakeSubdsideText != null && quakeSubdsideText.Length > 0)
        {
            isDialogueActive = true;
            dialogueManager.StartDialogue(quakeSubdsideText, false, () => { isDialogueActive = false; });
            
            yield return new WaitUntil(() => !isDialogueActive);
        }
    }
}