using UnityEngine;
using DG.Tweening; // Add this!

public class RingTrigger : MonoBehaviour
{
    [Header("Fade Settings")]
    public RingManager ringManager;
    public float fadeDuration = 0.5f; // How long the fade takes
    public Ease fadeEase = Ease.OutQuad; // Smooth easing

    private CanvasGroup canvasGroup; // For UI elements
    private Renderer ringRenderer; // For 3D objects
    private Material ringMaterial; // Cached material for alpha

    void Start()
    {
        // Cache components for performance
        SetupFadeComponents();
    }

    void Update()
    {
        transform.Rotate(0, 30 * Time.deltaTime, 0);
    }

    void SetupFadeComponents()
    {
        // Try CanvasGroup first (for UI Canvas elements)
        canvasGroup = GetComponent<CanvasGroup>();

        // For 3D objects, get renderer and material
        ringRenderer = GetComponent<Renderer>();
        if (ringRenderer != null && ringRenderer.material != null)
        {
            ringMaterial = ringRenderer.material;
            // Ensure material supports transparency
            if (!ringMaterial.HasProperty("_Color"))
                Debug.LogWarning("Ring material may not support alpha fading!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && ringManager != null)
        {
            FadeAndDestroy(); // Fade out instead of instant deactivate
            ringManager.OnRingPassed();
        }
    }

    void FadeAndDestroy()
    {
        // Kill any existing tweens to avoid conflicts
        DOTween.Kill(transform);

        if (canvasGroup != null)
        {
            // UI CanvasGroup fade (simplest)
            canvasGroup.DOFade(0f, fadeDuration)
                .SetEase(fadeEase)
                .OnComplete(() => gameObject.SetActive(false));
        }
        else if (ringRenderer != null && ringMaterial != null)
        {
            // 3D Renderer fade
            Color originalColor = ringMaterial.color;
            ringMaterial.DOFade(0f, fadeDuration)
                .SetEase(fadeEase)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    // Optional: Restore original color if reused
                    ringMaterial.color = originalColor;
                });
        }
        else
        {
            // Fallback: Scale down + fade alpha if possible
            transform.DOScale(0f, fadeDuration)
                .SetEase(fadeEase)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}