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
    public enum GameState { Intro, Idle, Quake, Selesai }

    [Header("Pengaturan Intro")]
    public Transform introSpawnPointStart;
    public Transform introSpawnPointEnd;
    private Vector3 originalPlayerScale;

    [Header("UI")]
    public UIManager uiManager;

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

        playerRenderer.sortingOrder = 3;

        if (introSpawnPointStart != null){
            player.transform.position = introSpawnPointStart.position;
            player.transform.localScale = introSpawnPointStart.localScale;
            }

        if (playerController != null) playerController.SetWalking(true);

        player.transform.DOMove(introSpawnPointEnd.position, 5f).SetEase(Ease.Linear);
        
        yield return new WaitForSeconds(5f); 

        player.transform.position = startPosition.position;
        if (playerController != null) playerController.SetWalking(false);
        if (playerController != null) playerController.SetHiding(false);
        playerRenderer.sortingOrder = 10;
        
        currentState = GameState.Idle;
        Debug.Log("Intro selesai, siap memilih tempat sembunyi.");
    }

    public void SelectHidingSpot(HidingSpot hidingSpot)
    {
        if (currentState != GameState.Idle || hidingSpot.hasSelected) return;

        StartCoroutine(HideRoutine(hidingSpot));
    }

    IEnumerator HideRoutine(HidingSpot hidingSpot)
    {
        currentState = GameState.Quake;
        hidingSpot.hasSelected = true;

        player.transform.DOKill();

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
            Debug.Log("GAME OVER: Semua tempat sudah dites! Simulasi selesai.");
            currentState = GameState.Selesai; // Kunci game permanen
        }
        else
        {
            Debug.Log("Silakan pilih barang lain yang belum dites...");
            currentState = GameState.Idle; // Buka kunci lagi untuk barang selanjutnya
        }
    }
}