using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileFloor : MonoBehaviour
{
    [SerializeField] private TrembleSource _tremble;
    [SerializeField] private Collider _floorCollider;
    [SerializeField] private Transform _floorMesh;
    [SerializeField] private float _destroyAfterTime = 1;

    private void OnTriggerEnter(Collider other)
    {
        _tremble.TriggerTremble();
        StartCoroutine(DestroyAfter(_destroyAfterTime));
    }

    private IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);

        _floorCollider.enabled = false;
        _floorMesh.gameObject.SetActive(false);

        yield return null;
    }
}
