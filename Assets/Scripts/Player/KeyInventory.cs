using System.Collections.Generic;
using UnityEngine;

public class KeyInventory : MonoBehaviour
{
    public List<KeyData> keys = new List<KeyData>();

    public bool HasKey(KeyData key)
    {
        return keys.Contains(key);
    }

    public void AddKey(KeyData key)
    {
        if (!HasKey(key))
        {
            keys.Add(key);
            Debug.Log("Key added: " + key.keyName);
        }
        else
        {
            Debug.Log("Key already in inventory: " + key.keyName);
        }
    }
}