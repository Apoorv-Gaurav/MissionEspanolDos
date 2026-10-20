using UnityEngine;

/// <summary>Subtle idle motion for static decorative character sprites.
/// Do not attach to player-controlled objects or objects with an Animator.</summary>
public class GentleCharacterIdle : MonoBehaviour
{
    public float bobHeight = 0.035f;
    public float bobSpeed = 1.7f;
    public float scalePulse = 0.008f;

    private Vector3 startPosition;
    private Vector3 startScale;
    private float phase;

    private void Awake()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
        phase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float wave = Mathf.Sin(Time.time * bobSpeed + phase);
        transform.localPosition = startPosition + Vector3.up * (wave * bobHeight);
        float pulse = 1f + Mathf.Sin(Time.time * bobSpeed * 0.7f + phase) * scalePulse;
        transform.localScale = new Vector3(startScale.x * pulse, startScale.y * pulse, startScale.z);
    }
}
