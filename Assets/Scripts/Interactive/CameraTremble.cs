using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTremble : MonoBehaviour
{
    private Vector3 _originalPosition;

    private void Start()
    {
        _originalPosition = transform.localPosition;
    }

    public void Tremble(TrembleSource source)
    {
        StartCoroutine(TrembleAnimate(source));
    }

    private IEnumerator TrembleAnimate(TrembleSource source)
    {
        float startTime = Time.time;
        float stopTime = Time.time + source.GetDuration();
        float radius = source.GetRadius();

        while(Time.time < stopTime)
        {
            float timeFadeout = 1 - ((Time.time - startTime) / source.GetDuration());
            float distanceFadeout = 1 - (source.transform.position - transform.position).magnitude / radius;
            float intensity = timeFadeout * Mathf.Clamp(timeFadeout * distanceFadeout, 0, 1) * source.GetIntensity();

            transform.localPosition = _originalPosition + new Vector3(Random.Range(-intensity,intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));

            yield return new WaitForFixedUpdate();
        }

        transform.localPosition = _originalPosition;

        yield return null;
    }
}
