using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonPanel : MonoBehaviour
{
    protected List<SphereCollider> _buttonPanel;

    protected void AssignButtonColliders()
    {
        _buttonPanel = new List<SphereCollider>(GetComponents<SphereCollider>());
    }
    
    public abstract void PressButton(SphereCollider col);
}
