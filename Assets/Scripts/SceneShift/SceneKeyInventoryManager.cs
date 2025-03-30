using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SceneKeyInventoryManager : MonoBehaviour
{
    private KeyInventory keyInventory;

    void Start()
    {
        keyInventory = GetComponent<KeyInventory>();
        if (keyInventory == null)
        {
            Debug.LogError("KeyInventory component not found on this GameObject!");
            return;
        }

        LoadKeyInventory();
        CheckDestroyedKeys();
    }

    private void LoadKeyInventory()
    {
        string sceneName = gameObject.scene.name;
        if (SceneData.KeyInventories.ContainsKey(sceneName))
        {
            List<string> keyNames = SceneData.KeyInventories[sceneName];
            if (keyNames != null)
            {
                foreach (string keyName in keyNames)
                {
                    KeyData key = FindKeyDataByName(keyName);
                    if (key != null)
                    {
                        keyInventory.AddKey(key);
                    }
                    else
                    {
                        Debug.LogWarning($"KeyData with name {keyName} not found in scene {sceneName}");
                    }
                }
            }
        }
    }
     void OnDestroy()
    {
        SaveKeyInventory();
    }
    private void SaveKeyInventory()
    {
        string sceneName = gameObject.scene.name;
        List<string> keyNames = keyInventory.keys.Select(key => key.keyName).ToList();
        SceneData.KeyInventories[sceneName] = keyNames;
    }

    private KeyData FindKeyDataByName(string keyName)
    {
        //  Ищи в Resources, чтобы скрипт не зависел от порядка загрузки сцен
        KeyData[] allKeys = Resources.FindObjectsOfTypeAll<KeyData>();
        foreach (KeyData key in allKeys)
        {
            if (key.keyName == keyName)
            {
                return key;
            }
        }
        return null;
    }
    private void CheckDestroyedKeys()
    {
        string sceneName = gameObject.scene.name;
        KeyGrabInteract[] keys = FindObjectsOfType<KeyGrabInteract>();

        foreach (KeyGrabInteract key in keys)
        {
            if (key.GetComponent<KeyGrabInteract>() == null) continue;
            KeyData keyData = key.GetComponent<KeyGrabInteract>().keyToGrab;
            if (keyData != null && SceneData.IsObjectDestroyed(sceneName, keyData.keyColor))
            {
                Destroy(key.gameObject); // Уничтожаем ключ, если он уже был подобран
            }
        }
    }
}