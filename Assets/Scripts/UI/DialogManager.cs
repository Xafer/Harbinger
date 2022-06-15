using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Current;

    [SerializeField] private List<Dialog> _dialogs;

    [SerializeField] private Text _dialogBox;
    [SerializeField] private Text _nameBox;

    [SerializeField] private float _textSpeed = 4;

    private Dialog _currentDialog;
    private int _currentDialogString;
    private int _currentDialogLetter;

    private int _dialogIndex;

    private void Awake()
    {
        Current = this;
        _dialogIndex = 0;
    }

    public void CycleDialogs()
    {
        SelectDialog(_dialogIndex);

        _dialogIndex++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            NextString();

        if (Input.GetKeyDown(KeyCode.Q))
            SelectDialog(0);
    }

    public void SelectDialog(int dialogIndex)
    {
        _currentDialog = _dialogs[dialogIndex];
        _currentDialogString = 0;
        _currentDialogLetter = 0;

        StartCoroutine(AnimateText());
    }

    public void NextString()
    {
        if (_currentDialog == null)
            return;

        int dialogLength = _currentDialog.GetLine(_currentDialogString).Length - 1;
        if (_currentDialogLetter < dialogLength)
            _currentDialogLetter = dialogLength;
        else if(_currentDialogString < _currentDialog.GetDialogCount()-1)
        {
            _currentDialogString++;
            StartCoroutine(AnimateText());
        }
        else
        {
            _currentDialog.EndText();
            _currentDialog = null;

            _dialogBox.text = "";
            _nameBox.text = "";
        }
    }

    public IEnumerator AnimateText()
    {
        _currentDialogLetter = 0;
        string text = _currentDialog.GetLine(_currentDialogString);

        _nameBox.text = _currentDialog.GetName() + " :";

        while(_currentDialogLetter < text.Length)
        {
            _currentDialogLetter++;

            _dialogBox.text = text.Substring(0, _currentDialogLetter);

            yield return new WaitForSeconds(0.1f / _textSpeed);
        }

        yield return null;
    }
}
