using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHand : MonoBehaviour
{
    [System.Serializable]
    public struct HandheldPoint
    {
        public Vector3 Rotation;
        public Vector3 LocalPosition;
    }

    [SerializeField] private List<HandheldPoint> _heldPositions = new List<HandheldPoint>();
    public enum HandheldState { Pocket, Side, Main }

    private HandheldState _heldState;
    private bool _handHeldAnimating;

    private Transform _currentItem;

    private HandheldPoint _startPosition;

    private float _movementProgression;

    private void Start()
    {
        _heldState = HandheldState.Pocket;
        _movementProgression = 0;
    }

    private void Update()
    {
        if(_currentItem != null && _movementProgression < 1)
        {
            _movementProgression += Time.deltaTime * 2;

            HandheldPoint endPosition = _heldPositions[StateToInt(_heldState)];

            _currentItem.localPosition = Vector3.Lerp(_startPosition.LocalPosition, endPosition.LocalPosition, _movementProgression);
            _currentItem.localRotation = Quaternion.Lerp(Quaternion.Euler(_startPosition.Rotation), Quaternion.Euler(endPosition.Rotation), _movementProgression);

            if (_movementProgression >= 1)
            {
                _handHeldAnimating = false;
            }
        }
    }

    public void MoveToPosition(Transform item, HandheldState state)
    {
        _currentItem = item;

        _startPosition = new HandheldPoint { LocalPosition = _currentItem.localPosition, Rotation = _currentItem.localRotation.eulerAngles };
        _movementProgression = 0;
        _heldState = state;
        _handHeldAnimating = true;
    }

    public void MoveCurrentToPosition(HandheldState state)
    {
        MoveToPosition(_currentItem, state);
    }

    public void SwitchToItem(Transform item)
    {
        if(_currentItem != null)PocketItem(_currentItem);

        item.gameObject.SetActive(true);

        SetItemPosition(item, HandheldState.Pocket);
        MoveToPosition(item, HandheldState.Side);
    }

    public void PocketItem(Transform item)
    {

        StartCoroutine(PocketItemCoroutine(item));
    }

    public IEnumerator PocketItemCoroutine(Transform item)
    {
        float i = 0;

        Vector3 itemPos = item.localPosition;
        Quaternion itemRot = item.localRotation;

        while(i < 1)
        {
            i += Time.deltaTime * 2;

            HandheldPoint endPosition = _heldPositions[0];

            item.localPosition = Vector3.Lerp(itemPos, endPosition.LocalPosition, i);
            item.localRotation = Quaternion.Lerp(itemRot, Quaternion.Euler(endPosition.Rotation), i);

            if(i >= 1)
                item.gameObject.SetActive(false);

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    public bool IsMoving()
    {
        return _handHeldAnimating;
    }

    public HandheldState GetCurrentHeldState()
    {
        return _heldState;
    }

    public HandheldPoint GetPointData(HandheldState state)
    {
        return _heldPositions[StateToInt(state)];
    }

    public void SetItemPosition(Transform item, HandheldState state)
    {

        HandheldPoint point = _heldPositions[StateToInt(state)];

        item.transform.localPosition = point.LocalPosition;
        item.transform.localRotation = Quaternion.Euler(point.Rotation);
    }

    private int StateToInt(HandheldState state)
    {

        return      state == HandheldState.Pocket ? 0 :
                    state == HandheldState.Side ?   1 :
                                                    2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        HandheldPoint a = _heldPositions[0];
        HandheldPoint b = _heldPositions[1];
        HandheldPoint c = _heldPositions[2];

        Gizmos.DrawLine(a.LocalPosition + transform.position, Quaternion.Euler(a.Rotation) * transform.up + a.LocalPosition + transform.position);
        Gizmos.DrawLine(b.LocalPosition + transform.position, Quaternion.Euler(b.Rotation) * transform.up + b.LocalPosition + transform.position);
        Gizmos.DrawLine(c.LocalPosition + transform.position, Quaternion.Euler(c.Rotation) * transform.up + c.LocalPosition + transform.position);
    }

    public bool HasCurrentItem()
    {
        return _currentItem != null;
    }
}
