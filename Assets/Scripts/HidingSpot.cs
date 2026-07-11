using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Masukkan Empty GameObject di sini untuk menentukan posisi & ukuran akhir pemain")]
    public Transform hidingSpot;
    public bool hasSelected = false;
    public virtual bool IsSafe => true;
    
    public string spotName;

    [Header("UI Text")]
    [TextArea(3, 5)]
    public string resultMessage;


    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnMouseDown()
    {
        if (hasSelected) return;

        if(gameManager != null)
        {
            gameManager.SelectHidingSpot(this);
        }
    }

    public virtual void OnQuakeEffect()
    {
        Debug.Log(spotName + " is shaking slightly but remains intact.");
    }
}