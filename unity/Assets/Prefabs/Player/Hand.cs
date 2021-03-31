using UnityEngine;

public class Hand : MonoBehaviour
{
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
                    _equippedItem.transform.localPosition = _equippedItem.RelativeTransform.localPosition;
                    _equippedItem.transform.localRotation = _equippedItem.RelativeTransform.localRotation;
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
