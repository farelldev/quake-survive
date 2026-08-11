using UnityEngine;

public class DoorExit : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameManager.currentState == GameManager.GameState.Outro && collision.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.FinishGame();
            }
        }
    }
}