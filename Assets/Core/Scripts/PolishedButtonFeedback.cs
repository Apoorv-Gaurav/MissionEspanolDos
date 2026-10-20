using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PolishedButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 baseScale;
    private bool initialized;
    private GamePolishBootstrap audioManager;

    public void Configure(GamePolishBootstrap manager)
    {
        audioManager = manager;
        if (!initialized)
        {
            baseScale = transform.localScale;
            initialized = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized) { baseScale = transform.localScale; initialized = true; }
        StopAllCoroutines();
        StartCoroutine(ScaleTo(baseScale * 1.035f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(baseScale));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioManager == null) audioManager = GamePolishBootstrap.Instance;
        if (audioManager != null) audioManager.PlayClick();
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float elapsed = 0f;
        const float duration = 0.10f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        transform.localScale = target;
    }
}

