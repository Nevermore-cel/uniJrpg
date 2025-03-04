using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Items Data", menuName = "Items Data")]
public class ItemsData : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int rangeX; // Range по X
    public int rangeY; // Range по Y
    public int cost;
    public int damage;
    public ActionType typeAction;
    public string description;
    [HideInInspector] public int quantity;
}