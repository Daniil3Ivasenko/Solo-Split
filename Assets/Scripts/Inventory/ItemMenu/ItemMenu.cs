using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour
{
    private PickupItem _item;
    private int Currentindex;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Button _throw;
    [SerializeField] private Button _split;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject _splitPanel;

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
        _slider.maxValue = _inventory.inventory[index].amount;
        _splitPanel.SetActive(false);
    }

    public void OnSliderValueChanged()
    {
        text.text = _slider.value.ToString();
    }
    public void OnSplit()
    {
        if(_slider.value != 0)
        {
            if (_inventory.AddItem(_inventory.inventory[Currentindex].item, _slider.value.ConvertTo<int>(), true))
            {
                _inventory.RemoveItem(Currentindex, _slider.value.ConvertTo<int>());
            }
        }
        DisableItemMenu();
    }

    public void DisableItemMenu()
    {
        gameObject.SetActive(false);
    }
}
