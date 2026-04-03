using UnityEngine;

public class HeartbeatEffect : MonoBehaviour
{
    [Header("Scale Ayarları")]
    [SerializeField] private Vector3 baseScale = Vector3.one;
    [SerializeField] private float beatScaleMultiplier = 1.15f;

    [Header("Zaman Ayarları")]
    [SerializeField] private float firstBeatDuration = 0.08f;
    [SerializeField] private float secondBeatDuration = 0.1f;
    [SerializeField] private float pauseAfterBeat = 0.5f;

    private float timer;
    private int phase = 0;

    private void Start()
    {
        transform.localScale = baseScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        switch (phase)
        {
            // İlk küçük vurgu
            case 0:
                transform.localScale = Vector3.Lerp(baseScale, baseScale * beatScaleMultiplier, timer / firstBeatDuration);
                if (timer >= firstBeatDuration)
                {
                    timer = 0f;
                    phase = 1;
                }
                break;

            // Tekrar normale dön
            case 1:
                transform.localScale = Vector3.Lerp(baseScale * beatScaleMultiplier, baseScale, timer / firstBeatDuration);
                if (timer >= firstBeatDuration)
                {
                    timer = 0f;
                    phase = 2;
                }
                break;

            // İkinci vurgu
            case 2:
                transform.localScale = Vector3.Lerp(baseScale, baseScale * (beatScaleMultiplier + 0.08f), timer / secondBeatDuration);
                if (timer >= secondBeatDuration)
                {
                    timer = 0f;
                    phase = 3;
                }
                break;

            // Tekrar normale dön
            case 3:
                transform.localScale = Vector3.Lerp(baseScale * (beatScaleMultiplier + 0.08f), baseScale, timer / secondBeatDuration);
                if (timer >= secondBeatDuration)
                {
                    timer = 0f;
                    phase = 4;
                }
                break;

            // Kısa bekleme
            case 4:
                if (timer >= pauseAfterBeat)
                {
                    timer = 0f;
                    phase = 0;
                }
                break;
        }
    }
}