using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Jet objects/item")]
public class item : ScriptableObject
{
    public Sprite icon;
    public int id;
    public string title;
    public string description;
    public int count;
    public int maxCount;
    public Vector2Int occupiedSpace;
    public ItemClass itemClass;

    public item(item it)
    {
        this.icon = it.icon;
        this.id = it.id;
        this.title = it.title;
        this.description = it.description;
        this.count = it.count;
        this.maxCount = it.maxCount;
        this.occupiedSpace = it.occupiedSpace;
        this.itemClass = it.itemClass;
    }
}
