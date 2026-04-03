using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSequenceController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D[] targetLights;
    [SerializeField] private ParticleSystem targetParticle;

    [Header("Timing")]
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float globalLightChangeDuration = 1f;
    [SerializeField] private float targetLightChangeDuration = 1f;

    [Header("Light Values")]
    [SerializeField] private float globalLightStartIntensity = 1f;
    [SerializeField] private float globalLightTargetIntensity = 0.3f;

    [SerializeField] private float targetLightStartIntensity = 0f;
    [SerializeField] private float targetLightTargetIntensity = 1.5f;

    [Header("Options")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool disablePlayerDetectorOnStart = true;

    private void Start()
    {
        if (globalLight != null)
            globalLight.intensity = globalLightStartIntensity;

        if (targetLights != null && targetLights.Length > 0)
        {
            foreach (Light2D light in targetLights)
            {
                if (light == null)
                    continue;

                light.intensity = targetLightStartIntensity;

                if (disablePlayerDetectorOnStart)
                {
                    PlayerDetector detector = light.GetComponent<PlayerDetector>();
                    if (detector != null)
                        detector.enabled = false;
                }
            }
        }

        if (playOnStart)
            StartCoroutine(PlaySequence());
    }

    public void StartSequence()
    {
        StopAllCoroutines();
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(startDelay);

        if (targetLights != null && targetLights.Length > 0)
        {
            foreach (Light2D light in targetLights)
            {
                if (light == null)
                    continue;

                PlayerDetector detector = light.GetComponent<PlayerDetector>();
                if (detector != null)
                    detector.enabled = true;

                light.intensity = targetLightStartIntensity;
            }
        }

        if (globalLight != null)
        {
            yield return StartCoroutine(ChangeLightIntensity(
                globalLight,
                globalLight.intensity,
                globalLightTargetIntensity,
                globalLightChangeDuration
            ));
        }

        if (targetLights != null && targetLights.Length > 0)
        {
            foreach (Light2D light in targetLights)
            {
                if (light == null)
                    continue;

                StartCoroutine(ChangeLightIntensity(
                    light,
                    light.intensity,
                    targetLightTargetIntensity,
                    targetLightChangeDuration
                ));
            }

            yield return new WaitForSeconds(targetLightChangeDuration);
        }

        if (targetParticle != null)
            targetParticle.Play();
    }

    private IEnumerator ChangeLightIntensity(Light2D light2D, float from, float to, float duration)
    {
        if (light2D == null)
            yield break;

        if (duration <= 0f)
        {
            light2D.intensity = to;
            yield break;
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            light2D.intensity = Mathf.Lerp(from, to, t);
            yield return null;
        }

        light2D.intensity = to;
    }
}