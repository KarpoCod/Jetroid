using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "item")]
public class item : ScriptableObject
{
    public Sprite icon;
    public int id;
    public string title;
    public string description;
    public int count;
    public int maxCount;
    public Vector2Int occupiedSpace;
}
