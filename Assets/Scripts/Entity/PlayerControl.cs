using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Entity ControlledEntity;

    private Vector3 _movement;
    private Vector2 _mouseMovement;
    private bool _jump;

    [SerializeField] private float _mouseSensitivity = 300;

    void Start()
    {
        ToggleCursorLock();
    }

    // Universal player control
    void Update()
    {
        UpdateInput();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleCursorLock();

        float vertical =    Input.GetKey(KeyCode.Space)?        1 :
                            Input.GetKey(KeyCode.LeftControl)?  -1 :
                                                                0;
        _movement.Set(Input.GetAxis("Horizontal"), vertical, Input.GetAxis("Vertical"));

        if (ControlledEntity.Gravitated && Input.GetKeyDown(KeyCode.Space))
            _jump = true;


        _mouseMovement = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        if (Input.GetKeyDown(KeyCode.T))
            GameData.Instance.PlayerCharacter.UseItem();

        if (Input.GetMouseButtonDown(1))
        {
            GameData.Instance.PlayerCharacter.ExamineItem(true);
            GameData.Instance.SetControlledCharacter(false);
        }

        if (Input.GetMouseButtonUp(1))
        {
            GameData.Instance.PlayerCharacter.ExamineItem(false);
            GameData.Instance.SetControlledCharacter(true);
        }

        if (Input.GetKeyDown(KeyCode.Q))
            GameData.Instance.PlayerCharacter.CycleInventory(true);

        if (Input.GetKeyDown(KeyCode.E))
            GameData.Instance.PlayerCharacter.CycleInventory(false);

        if (Input.GetKeyDown(KeyCode.Z))
            GameData.Instance.PlayerCharacter.PlaceRobot();

        if (Input.GetKeyDown(KeyCode.R))
            GameData.Instance.PlayerCharacter.ClingToPath();

        if (Input.GetMouseButton(0))
            GameData.Instance.PlayerCharacter.InteractForward();
    }

    private void UpdateMovement()
    {
        if (_jump)
            Jump();

        Vector3 eulerRotation = new Vector3(_mouseMovement.y,_mouseMovement.x,0) * Time.deltaTime * _mouseSensitivity;

        ControlledEntity.Move(_movement, eulerRotation);
    }

    private void Jump()
    {
        ControlledEntity.Jump();
        _jump = false;
    }

    private void ToggleCursorLock()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
