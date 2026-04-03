using System.Collections;
using UnityEngine;

public class PlayerDissolve : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Shader Properties")]
    [SerializeField] private string dissolveAmountProperty = "_DissolveAmount";

    [Header("Dissolve Settings")]
    [SerializeField] private float dissolveDuration = 1f;
    [SerializeField] private float startValue = 0f;
    [SerializeField] private float endValue = 1f;
    [SerializeField] private bool disableObjectAfterDissolve = false;

    private Material runtimeMaterial;
    private bool isInitialized = false;
    private bool isDissolving = false;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized)
            return;

        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        if (targetRenderer == null)
        {
            Debug.LogError($"{nameof(PlayerDissolve)} -> SpriteRenderer bulunamadı.", this);
            return;
        }

        runtimeMaterial = targetRenderer.material;
        if (runtimeMaterial == null)
        {
            Debug.LogError($"{nameof(PlayerDissolve)} -> Material bulunamadı.", this);
            return;
        }

        if (!runtimeMaterial.HasProperty(dissolveAmountProperty))
        {
            Debug.LogError(
                $"{nameof(PlayerDissolve)} -> Material içinde '{dissolveAmountProperty}' parametresi yok.",
                this
            );
            return;
        }

        runtimeMaterial.SetFloat(dissolveAmountProperty, startValue);
        isInitialized = true;
    }

    public void ResetDissolve()
    {
        Initialize();

        if (!isInitialized)
            return;

        runtimeMaterial.SetFloat(dissolveAmountProperty, startValue);
    }

    public IEnumerator PlayDissolve()
    {
        Initialize();

        if (!isInitialized)
            yield break;

        if (isDissolving)
            yield break;

        isDissolving = true;

        float elapsed = 0f;
        runtimeMaterial.SetFloat(dissolveAmountProperty, startValue);

        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dissolveDuration);
            float value = Mathf.Lerp(startValue, endValue, t);

            runtimeMaterial.SetFloat(dissolveAmountProperty, value);
            yield return null;
        }

        runtimeMaterial.SetFloat(dissolveAmountProperty, endValue);

        if (disableObjectAfterDissolve)
            gameObject.SetActive(false);

        isDissolving = false;
    }
   
}