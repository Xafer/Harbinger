using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private Camera _playerCamera;

    [SerializeField] private PlayerItemHand _rightHand;

    private Inventory _inventory;
    private Item _heldItem;

    public Transform lantern;

    private Collider _cameraTarget;

    void Start()
    {
        GameData.Instance.PlayerCharacter = this;
        Gravitated = true;
        Grounded = true;

        _inventory = new Inventory();
    }

    private void Update()
    {
        if(CurrentPath == null)
            UpdateEntity();

        UpdateCameraTarget();
    }

    public override float GetXRotation()
    {
        return _playerCamera.transform.eulerAngles.x;
    }

    private void UpdateCameraTarget()
    {
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2, 1 << 8 | 1 << 9 | 1 << 6 | 1 << 11) && hit.transform.parent != transform)
        {

            PlayerRobot pr = hit.transform.GetComponent<PlayerRobot>();
            if (pr != null ||
                hit.transform.gameObject.layer != 6)
            _cameraTarget = hit.collider;
        }
        else
            _cameraTarget = null;
    }

    public void InteractForward()
    {
        if (_cameraTarget == null || _rightHand.IsMoving())
            return;

        int hitLayer = _cameraTarget.transform.gameObject.layer;
        if(hitLayer == 6 && GameData.Instance.PlayerRobotCharacter.CanBePickedUp())
        {
            _inventory.HasPlayerRobot = true;
            GameData.Instance.PlayerRobotCharacter.gameObject.SetActive(false);
        }
        if (hitLayer == 8)
        {
            _cameraTarget.GetComponent<ButtonPanel>().PressButton((SphereCollider)_cameraTarget);
        }
        else if (hitLayer == 9)
        {
            Item item = _cameraTarget.GetComponent<Item>();

            if (item != null)
            {
                if (_heldItem != null)
                    _inventory.AddItem(_heldItem);

                item.transform.parent = transform;

                _rightHand.SetItemPosition(item.transform, PlayerItemHand.HandheldState.Pocket);
                _rightHand.SwitchToItem(item.transform);

                _heldItem = item;
            }
        }
        else if( hitLayer == 11)
        {
            _cameraTarget.GetComponent<Door>().ToggleDoor();
        }
    }

    public void PlaceRobot()
    {
        if(_inventory.HasPlayerRobot)
        {
            GameData.Instance.PlayerRobotCharacter.gameObject.SetActive(true);
            GameData.Instance.PlayerRobotCharacter.PlaceDown(_playerCamera.transform);

            _inventory.HasPlayerRobot = false;
        }
    }

    public override void Move(Vector3 direction, Vector3 eulerRotate)
    {
        float rotationX = _playerCamera.transform.eulerAngles.x + eulerRotate.x;
        rotationX = Mathf.Clamp(Mathf.Abs(rotationX) > 180 ? rotationX - Mathf.Sign(rotationX) * 360 : rotationX, -85, 85);

        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + eulerRotate.y);
        _playerCamera.transform.localEulerAngles = new Vector3(rotationX, 0, 0);
        lantern.eulerAngles = _playerCamera.transform.eulerAngles;

        _running = Input.GetKey(KeyCode.LeftControl);

        base.Move(direction, eulerRotate);
    }

    private void FixedUpdate()
    {

        if (_rightHand.GetCurrentHeldState() == PlayerItemHand.HandheldState.Main)
            _playerCamera.transform.LookAt(_heldItem.transform);
    }

    public void CycleInventory(bool backwards)
    {
        if (_inventory.GetInventoryCount() > 0)
        {
            Item itemToSwitch = backwards ? _inventory.CycleLeft(_heldItem) : _inventory.CycleRight(_heldItem);

            Debug.Log("Cycling to " +itemToSwitch.name);

            _rightHand.SwitchToItem(itemToSwitch.transform);

            _heldItem = itemToSwitch;
        }
    }

    public void ExamineItem(bool zoomIn)
    {
        if (_rightHand.GetCurrentHeldState() != PlayerItemHand.HandheldState.Pocket && _rightHand.HasCurrentItem())
        {
            if (zoomIn)
                _rightHand.MoveCurrentToPosition(PlayerItemHand.HandheldState.Main);
            else
                _rightHand.MoveCurrentToPosition(PlayerItemHand.HandheldState.Side);
        }
    }

    public void UseItem()
    {
        _heldItem.Use();
    }
}
