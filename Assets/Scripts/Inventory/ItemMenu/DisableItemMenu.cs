using UnityEngine;
using UnityEngine.EventSystems;

public class DisableItemMenu : MonoBehaviour, IPointerClickHandler
{
    private ItemMenu itemMenu;

    void Awake()
    {
        itemMenu = FindFirstObjectByType<ItemMenu>(FindObjectsInactive.Include);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        itemMenu.gameObject.SetActive(false);
    }
}
