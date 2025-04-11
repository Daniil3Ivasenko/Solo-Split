using UnityEngine;

[System.Serializable]
public class Slot : MonoBehaviour
{
    public Item item;
    public int amount;

    public bool isEmpty => item == null;

    public void ClearSlot()
    {
        amount = 0;
        item = null;
    }
}
