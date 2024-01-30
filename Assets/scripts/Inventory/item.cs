using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class item : ScriptableObject
{
    public Sprite icon;
    public static int id;
    public static string title;
    public static string description;
    public static int MaxCount = 10;
    public static Vector2Int occupiedSpace;
}
