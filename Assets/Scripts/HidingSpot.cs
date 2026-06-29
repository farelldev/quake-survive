using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public bool isSafe;
    public string spotName;
    
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnMouseDown()
    {
        if(gameManager != null)
        {
            gameManager.SelectHidingSpot(this);
        }
    }
}