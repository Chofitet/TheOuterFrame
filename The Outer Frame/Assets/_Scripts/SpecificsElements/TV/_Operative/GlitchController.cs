using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GlitchController : MonoBehaviour
{
    [Header("Referencias a scripts de shader (asignar en Inspector)")]
    public ShaderEffect_Tint tint;
    public ShaderEffect_Scanner scanner;
    public ShaderEffect_BleedingColors bleedingColors;

    [Header("Duración total del glitch (segundos)")]
    [Tooltip("Duración total que durará el comportamiento descrito")]
    public float totalDuration = 3f;

    [Header("Tint - U (oscila rápido)")]
    public float tintU_from = 0.8f;
    public float tintU_to = 1.3f;
    [Range(0.01f, 1f)]
    public float tintU_period = 0.08f; // tiempo de ida (yoyo usará dos veces)

    [Header("Scanner.area (transición lenta en 2 pasos)")]
    public float scanner_area_from = 0f;
    public float scanner_area_mid = 0.3f;
    public float scanner_area_to = 0.6f;
  

   
    [Header("Bleeding Shift (oscila rápido)")]
    public float bleedingShift_from = 0f;
    public float bleedingShift_to = 1f;
    [Range(0.01f, 1f)]
    public float bleedingShift_period = 0.06f;

    // internos
    private Tween tintUTween;
    private Tween bleedingShiftTween;
    private Sequence scannerSequence;
    private Coroutine stopCoroutine;

    // guardamos valores originales para restaurar
    float orig_tintU, orig_tintY, orig_tintV;
    float orig_scannerArea;
    float orig_bleedingIntensity, orig_bleedingShift;

    public void DoGlitch(Component sender, object obj)
    {
        PlayGlitch(totalDuration);
    }

    void Start()
    {
        CacheOriginals();
    }

    void CacheOriginals()
    {
        if (tint != null)
        {
            orig_tintU = tint.u;
            orig_tintY = tint.y;
            orig_tintV = tint.v;
        }

        if (scanner != null)
            orig_scannerArea = scanner.area;

        if (bleedingColors != null)
        {
            orig_bleedingIntensity = bleedingColors.intensity;
            orig_bleedingShift = bleedingColors.shift;
        }
    }

    /// <summary>
    /// Lanza el glitch durante duration segundos. Si ya hay uno activo, lo reinicia.
    /// </summary>
    public void PlayGlitch(float duration)
    {
        // si no hay referencias no hacemos nada
        if (tint == null && scanner == null && bleedingColors == null) return;

        // cancelar cualquier cosa previa
        StopGlitchImmediate();

        CacheOriginals();

        // --- tint.U oscila rápido entre tintU_from <-> tintU_to ---
        if (tint != null)
        {
            // aseguramos valor inicial
            tint.u = tintU_from;
            float singleDuration = tintU_period;
            tintUTween = DOTween.To(() => tint.u, x => tint.u = x, tintU_to, singleDuration)
                                .SetLoops(-1, LoopType.Yoyo)
                                .SetEase(Ease.Linear);
        }

        // --- bleeding.Shift oscila rápido entre from <-> to ---
        if (bleedingColors != null)
        {
            bleedingColors.shift = bleedingShift_from;
            float bsSingle = bleedingShift_period;
            bleedingShiftTween = DOTween.To(() => bleedingColors.shift, x => bleedingColors.shift = x, bleedingShift_to, bsSingle)
                                        .SetLoops(-1, LoopType.Yoyo)
                                        .SetEase(Ease.Linear);

        }

        // --- scanner.area transición lenta (from -> mid -> to) ---
        if (scanner != null)
        {
            // Si scanner_timeFraction == 0 no hacemos nada
            float scannerTotal = Mathf.Clamp01(totalDuration) * Mathf.Max(0.0001f, duration);
            float half = scannerTotal / 2f; // de from->mid y mid->to igual reparto
            scanner.area = scanner_area_from;

            scannerSequence = DOTween.Sequence();
            scannerSequence.Append(DOTween.To(() => scanner.area, x => scanner.area = x, scanner_area_mid, half).SetEase(Ease.Linear));
            scannerSequence.Append(DOTween.To(() => scanner.area, x => scanner.area = x, scanner_area_to, half).SetEase(Ease.Linear));
            // no loop; dura scannerTotal. If scannerTotal < duration, dejará el valor final.
        }

        // programamos el stop/restauración
        if (stopCoroutine != null) StopCoroutine(stopCoroutine);
        stopCoroutine = StartCoroutine(StopAfter(duration));
    }

    IEnumerator StopAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        StopGlitchAndRestore();
    }

    /// <summary>
    /// Para tweens inmediatamente sin restaurar valores.
    /// </summary>
    public void StopGlitchImmediate()
    {
        if (tintUTween != null) { tintUTween.Kill(); tintUTween = null; }
        if (bleedingShiftTween != null) { bleedingShiftTween.Kill(); bleedingShiftTween = null; }
        if (scannerSequence != null) { scannerSequence.Kill(); scannerSequence = null; }
        if (stopCoroutine != null) { StopCoroutine(stopCoroutine); stopCoroutine = null; }
    }

    /// <summary>
    /// Para todo y restaura los valores originales guardados.
    /// </summary>
    public void StopGlitchAndRestore()
    {
        StopGlitchImmediate();

        // Restaurar guardados
        if (tint != null)
        {
            tint.u = orig_tintU;
            tint.y = orig_tintY;
            tint.v = orig_tintV;
        }

        if (scanner != null)
            scanner.area = orig_scannerArea;

        if (bleedingColors != null)
        {
            bleedingColors.intensity = orig_bleedingIntensity;
            bleedingColors.shift = orig_bleedingShift;
        }
    }

    [ContextMenu("Play Glitch (inspector)")]
    void ContextPlay() => PlayGlitch(totalDuration);

    [ContextMenu("Stop Glitch and Restore (inspector)")]
    void ContextStop() => StopGlitchAndRestore();

    void OnDisable()
    {
        StopGlitchImmediate();
    }
}
