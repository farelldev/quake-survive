using Unity.VisualScripting;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Masukkan Empty GameObject di sini untuk menentukan posisi & ukuran akhir pemain")]
    public Transform hidingSpot;
    public Transform standSpot;
    public bool hasSelected = false;
    public virtual bool IsSafe => true;
    public virtual string PlayerHurtTrigger => "IsHurt";
    
    public string spotName;

    [Header("UI Text")]
    [TextArea(3, 5)]
    public string resultMessage;

    [Header("Object VFX")]
    public SpriteRenderer obstruction;

    public GameObject shineEffect;
    private SpriteRenderer shineRenderer; 
    public float pulsePace = 3f;
    public float fadePace = 3f;

    private float fadeAlpha = 0f;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (shineEffect != null)
        {
            shineRenderer = shineEffect.GetComponent<SpriteRenderer>();

            if (shineRenderer != null)
            {
                Color c = shineRenderer.color;
                c.a = 0f;
                shineRenderer.color = c;
            }
        }
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

    void Update()
    {
        if(shineEffect != null || shineRenderer == null){
            bool mustHide = hasSelected || gameManager.currentState != GameManager.GameState.Idle;
            float targetFade = mustHide ? 0f : 1f;
            
            fadeAlpha = Mathf.MoveTowards(fadeAlpha, targetFade, Time.deltaTime * fadePace);
            
            if (fadeAlpha <= 0f)
            {
                if (shineEffect.activeSelf) shineEffect.SetActive(false);
            }
            else {
                shineEffect.SetActive(true);

                if (shineRenderer != null)
                {
                    float wave = (Mathf.Sin(Time.time * pulsePace) + 1f) / 2f;
                    
                    float pulseAlpha = Mathf.Lerp(0.5f, 1f, wave);
                    
                    Color shineColor = shineRenderer.color;
                    shineColor.a = pulseAlpha * fadeAlpha;
                    shineRenderer.color = shineColor;
                }
            }
        }
    }
}