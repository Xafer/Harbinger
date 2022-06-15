using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorControl : ButtonPanel
{
    [SerializeField] private Elevator _controlledElevator;

    [SerializeField] private bool _toggleButton = false;

    void Start()
    {
        AssignButtonColliders();
    }
    public override void PressButton(SphereCollider col)
    {
        int index = _buttonPanel.IndexOf(col);

        if (_toggleButton)
            _controlledElevator.ChangeTarget(index == 1);
        else
            _controlledElevator.Move(index == 1);
    }
}
