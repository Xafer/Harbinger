using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dialog : MonoBehaviour
{
    [SerializeField] private List<string> _dialogs;

    [SerializeField] private UnityEvent _dialogEndEvent;

    [SerializeField] private string _dialogName;

    private int _dialogProgress;

    void Awake()
    {
        if (_dialogEndEvent == null)
            _dialogEndEvent = new UnityEvent();
    }

    public string GetLine(int i)
    {
        return _dialogs[i % (_dialogs.Count)];
    }

    public int GetDialogCount()
    {
        return _dialogs.Count;
    }

    public string GetName() { return _dialogName; }

    public void EndText()
    {
        _dialogEndEvent.Invoke();
    }
}
