using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour
{
    private PickupItem _item;
    private int Currentindex;
    [SerializeField] private Button _throw;
    [SerializeField] private Button _split;

    private void Awake()
    {
        _item = FindFirstObjectByType<PickupItem>();
    }

    public void ChangeIndex(int index)
    {
        Currentindex = index;
        _throw.onClick.RemoveAllListeners();
        _throw.onClick.AddListener(() => _item.ThrowItem(Currentindex));
        _throw.onClick.AddListener(() => DisableItemMenu());
    }

    public void DisableItemMenu()
    {
        gameObject.SetActive(false);
    }
}
