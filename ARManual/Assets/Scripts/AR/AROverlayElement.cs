using System.Collections;
using UnityEngine;

public class AROverlayElement : MonoBehaviour
{
    public enum OverlayType
    {
        Arrow,
        Label,
        Highlight,
        Animation
    }
    
    [SerializeField] private OverlayType type;
    [SerializeField] private string targetPoint;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseSpeed = 1.0f;
    [SerializeField] private bool animate = true;
    [SerializeField] private Color color = Color.white;
    
    private Vector3 originalScale;
    private Material material;
    
    private void Awake()
    {
        originalScale = transform.localScale;
        
        // Get material if available
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            if (material != null)
            {
                material.color = color;
            }
        }
    }
    
    private void OnEnable()
    {
        if (animate)
        {
            StartCoroutine(AnimateOverlay());
        }
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        
        // Reset scale
        transform.localScale = originalScale;
    }
    
    private IEnumerator AnimateOverlay()
    {
        switch (type)
        {
            case OverlayType.Arrow:
            case OverlayType.Highlight:
                yield return PulseAnimation();
                break;
                
            case OverlayType.Label:
                yield return FloatAnimation();
                break;
                
            case OverlayType.Animation:
                // Custom animation would be implemented here
                break;
        }
    }
    
    private IEnumerator PulseAnimation()
    {
        while (true)
        {
            // Pulse scale up and down
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                float scale = 1f + (pulseScale - 1f) * Mathf.Sin(t * Mathf.PI);
                transform.localScale = originalScale * scale;
                yield return null;
            }
        }
    }
    
    private IEnumerator FloatAnimation()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 0.05f, 0);
        
        while (true)
        {
            // Float up and down
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 0.5f;
                transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Sin(t * Mathf.PI));
                yield return null;
            }
        }
    }
    
    public void SetColor(Color newColor)
    {
        color = newColor;
        
        if (material != null)
        {
            material.color = color;
        }
    }
}