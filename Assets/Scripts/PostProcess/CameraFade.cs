using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour
{
    [SerializeField] private Material _cameraMaterial;

    [SerializeField] private float _fadeSpeed = 1;

    [SerializeField] private float _fadeAmount = 0;

    // Start is called before the first frame update
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _cameraMaterial);
    }

    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float start = _fadeAmount;

        float n = 0;

        while (n < 1)
        {
            n += Time.fixedDeltaTime * _fadeSpeed;

            _fadeAmount = start <= 0 ? n : 1 - n;

            _cameraMaterial.SetFloat("_FadeFactor", _fadeAmount);

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
}
