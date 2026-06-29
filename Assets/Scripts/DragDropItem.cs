using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public Transform targetBag; 

    private Vector3 startPosition;
    private bool isOverBag = false;
    private Collider2D myCollider;

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.DOKill(); 
        transform.localScale = Vector3.one; 
        
        startPosition = transform.position;
        isOverBag = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isOverBag && targetBag != null)
        {
            AnimateToBag();
        }
        else
        {
            if(isOverBag && targetBag == null) 
            {
                Debug.LogError("ERROR: 'Target Bag' reference is missing! Please assign it in the Inspector.");
            }

            transform.DOMove(startPosition, 0.3f).SetEase(Ease.OutBack);
        }
    }

    void AnimateToBag()
    {
        if(myCollider != null) myCollider.enabled = false;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOJump(targetBag.position, 1.5f, 1, 0.5f));
        sequence.Join(transform.DOScale(Vector3.zero, 0.5f));

        sequence.OnComplete(() => 
        {
            gameObject.SetActive(false);
            if(myCollider != null) myCollider.enabled = true;
        });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bag"))
        {
            isOverBag = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bag"))
        {
            isOverBag = false;
        }
    }
}