using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandheldUse : ItemUse
{
    [SerializeField] Material _robotCameraRT;
    [SerializeField] Material _radarCameraRT;

    [SerializeField] private Transform _handheldScreen;

    private bool _material;

    public override void UseItem()
    {
        _material = !_material;

        _handheldScreen.GetComponent<MeshRenderer>().material = _material ? _robotCameraRT : _radarCameraRT;
    }
}
