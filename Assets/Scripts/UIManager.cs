using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public GameObject dimBackground;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Image statusIcon1;
    public Image statusIcon2;

    [Header("Status Sprites")]
    public Sprite safeSprite;
    public Sprite dangerSprite;

    [Header("End Game UI")]
    public GameObject endGamePanel;

    void Start()
    {
        if (popupPanel != null)
        {
            dimBackground.SetActive(false);
            popupPanel.SetActive(false);
        }
    }

    public void ShowResult(string message, bool isSafe)
    {
        messageText.text = message;

        if (isSafe)
        {
            titleText.text = "KAMU AMAN!";
            statusIcon1.sprite = safeSprite;
            statusIcon2.sprite = safeSprite;
        }
        else
        {
            titleText.text = "BAHAYA!";
            statusIcon1.sprite = dangerSprite;
            statusIcon2.sprite = dangerSprite;
        }

        dimBackground.SetActive(true);
        popupPanel.SetActive(true);

        popupPanel.transform.localScale = Vector3.zero;
        popupPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

        Time.timeScale = 0f; 
    }

    public void ShowEndGamePopup()
    {
        if (endGamePanel != null) 
        {
            dimBackground.SetActive(true);
            endGamePanel.SetActive(true);

            endGamePanel.transform.localScale = Vector3.zero;
            endGamePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClosePopup()
    {
        dimBackground.SetActive(false);
        popupPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}