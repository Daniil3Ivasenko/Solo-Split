using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float rayDistance;
    [SerializeField] private TMP_Text itemText;
    public float RayDistance => rayDistance;
    PlayerInput input;
    void Awake()
    {
        input = new PlayerInput();
        input.Player.Interact.performed += context => Interact();
    }

    private void Update()
    {
        CheckInteract();
    }

    void Interact()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.TryGetComponent(out SceneITem sceneitem))
            {
                if (inventory.AddItem(sceneitem.item))
                Destroy(hit.collider.gameObject);
            }
        }
        Debug.DrawRay(transform.position, transform.forward, Color.green, rayDistance);
    }

    void CheckInteract()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.TryGetComponent(out SceneITem _sceneitem))
            {
                itemText.text = _sceneitem.item.name;
            }
            else
            {
                itemText.text = null;
            }
        }
        else
        {
            itemText.text = null;
        }
    }

    public void ThrowItem(int index)
    {
        Vector3 SpawnPosition = new Vector3();
        for (int i = 0; i < inventory.inventory[index].amount; i++)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, RayDistance))
                SpawnPosition = hit.point + hit.normal;
            else
                SpawnPosition = ray.origin + ray.direction * RayDistance;
            Instantiate(inventory.inventory[index].item.ScenePrefab, SpawnPosition, transform.rotation);
        }
        inventory.inventory[index].ClearSlot();
        ActionManager.ItemChanged?.Invoke();
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }
}
