using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool MirrorTransforms;

    public Equippable EquippedItem
    {
        get => _equippedItem;
        set
        {
            if (_equippedItem == value)
                return;

            if (_equippedItem != null)
            {
                _equippedItem.transform.parent = null;

                var body = _equippedItem.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.isKinematic = false;
                    body.detectCollisions = true;
                }
            }

            _equippedItem = value;

            if (_equippedItem != null)
            {
                _equippedItem.transform.SetParent(transform, false);

                var body = _equippedItem.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.isKinematic = true;
                    body.detectCollisions = false;
                }

                if (_equippedItem.RelativeTransform != null)
                {
                    _equippedItem.transform.localPosition = Utility.VectorMultiply(_equippedItem.RelativeTransform.localPosition, MirrorTransforms ? new Vector3(-1, 1, 1) : Vector3.one);
                    _equippedItem.transform.localEulerAngles = Utility.VectorMultiply(_equippedItem.RelativeTransform.localEulerAngles, MirrorTransforms ? new Vector3(1, 1, -1) : Vector3.one);
                    _equippedItem.transform.localScale = _equippedItem.RelativeTransform.localScale;
                }
                else
                {
                    _equippedItem.transform.localPosition = Vector3.zero;
                    _equippedItem.transform.localRotation = Quaternion.identity;
                    _equippedItem.transform.localScale = Vector3.one;
                }
            }
        }
    }
    
    private Equippable _equippedItem;
    private Rigidbody equippedItemBody;


    public void TryUseEquippedItem(PlayerController player)
    {
        EquippedItem?.Use(player);
    }
}
