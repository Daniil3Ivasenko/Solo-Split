using UnityEngine;

public class SceneITem : MonoBehaviour
{
    [SerializeField]private Item _item;
    public Item item => _item;
}
