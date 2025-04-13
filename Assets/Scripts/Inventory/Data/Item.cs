using UnityEngine;

[CreateAssetMenu(fileName ="ItemData", menuName ="Item/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string Id;
    public Sprite Icon;
    public int MaxStack = 1;
    public float Weight = 1;

    public GameObject ScenePrefab;

    public virtual void Use()
    {
        Debug.Log("Использован предмет: " + Name);
    }
}
