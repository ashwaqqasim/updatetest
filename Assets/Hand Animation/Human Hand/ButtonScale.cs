using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float scaleFactor = 1.2f; // äÓÈÉ ÇáÊßÈíÑ ÚäÏ ÇááãÓ
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleFactor; // íßÈÑ ÇáÒÑ ÚäÏ ÇáÖÛØ
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale; // íÑÌÚ ááÍÌã ÇáØÈíÚí ÚäÏ ÑİÚ ÇáÅÕÈÚ
    }
}
