using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class AbilityItemCreator
{
    private Transform abilityPanel;
    private GameObject abilityInfoPrefab;
    private float verticalSpacing;
    private Color defaultColor;
    private Color selectedColor;
    private GameObject currentlySelectedAbility = null;
    private GameObject currentlySelectedItem = null;
    public Dictionary<int, Dictionary<int, GameObject>> unitAbilities = new Dictionary<int, Dictionary<int, GameObject>>();
   public Dictionary<int, Dictionary<int, GameObject>> unitItems = new Dictionary<int, Dictionary<int, GameObject>>();

    // Делегаты для Unity методов (передаются из MonoBehaviour)
    private System.Func<GameObject, Transform, GameObject> instantiateDelegate;
    private System.Action<GameObject> destroyDelegate;

    // Делегат для обработки нажатия кнопки способности
    private System.Action<AbilityData, GameObject> onAbilityButtonClickedDelegate;

    // Делегат для обработки нажатия кнопки предмета
    private System.Action<ItemData, GameObject> onItemButtonClickedDelegate;
    public AbilityItemCreator(Transform panel, GameObject prefab, float spacing, Color defaultCol, Color selectedCol, System.Func<GameObject, Transform, GameObject> instantiate, System.Action<GameObject> destroy)
    {
        abilityPanel = panel;
        abilityInfoPrefab = prefab;
        verticalSpacing = spacing;
        defaultColor = defaultCol;
        selectedColor = selectedCol;

        //Сохраняем ссылки на методы Instantiate и Destroy
        instantiateDelegate = instantiate;
        destroyDelegate = destroy;
    }

    public void SetUnitAbilities(List<AbilityData> abilities, int unitId, System.Action<AbilityData, GameObject> onButtonClicked)
    {
        onAbilityButtonClickedDelegate = onButtonClicked;
        if (abilities == null || abilities.Count == 0)
        {
            Debug.LogWarning($"No abilities found for unit {unitId}");
            ClearAbilityInfoElements(unitId);
            return;
        }
        if (!unitAbilities.ContainsKey(unitId))
        {
            CreateAbilityInfoElements(abilities, unitId);
        }
        else
        {
            UpdateAbilityInfoElements(abilities, unitId);
        }
    }

    public void SetUnitItems(List<ItemData> items, int unitId, UnitData unitData, System.Action<ItemData, GameObject> onButtonClicked)
    {
        onItemButtonClickedDelegate = onButtonClicked;
        if (items == null || items.Count == 0)
        {
            Debug.LogWarning($"No items found for unit {unitId}");
            ClearItemInfoElements(unitId);
            return;
        }

        if (!unitItems.ContainsKey(unitId))
        {
            CreateItemInfoElements(items, unitId, unitData);
        }
        else
        {
            UpdateItemInfoElements(items, unitId, unitData);
        }
    }

    public void ShowAbilitiesInternal()
    {
        foreach (var kvp in unitAbilities)
        {
            foreach (var go in kvp.Value.Values)
            {
                go.SetActive(true);
            }
        }
    }

    public void ShowItemsInternal()
    {
        foreach (var kvp in unitItems)
        {
            foreach (var go in kvp.Value.Values)
            {
                go.SetActive(true);
            }
        }
    }

    public void HideAbilities()
    {
        foreach (var kvp in unitAbilities)
        {
            foreach (var go in kvp.Value.Values)
            {
                go.SetActive(false);
            }
        }
    }

    public void HideItems()
    {
        foreach (var kvp in unitItems)
        {
            foreach (var go in kvp.Value.Values)
            {
                go.SetActive(false);
            }
        }
    }
    public void ResetSelection()
    {
        if (currentlySelectedAbility != null)
        {
            currentlySelectedAbility.GetComponent<Image>().color = defaultColor;
            currentlySelectedAbility = null;
        }
        if (currentlySelectedItem != null)
        {
            currentlySelectedItem.GetComponent<Image>().color = defaultColor;
            currentlySelectedItem = null;
        }
    }

    public void ClearAllAbilityInfoElements()
    {
        foreach (var kvp in unitAbilities)
        {
            foreach (var go in kvp.Value.Values)
            {
                destroyDelegate(go); // Используем переданный делегат
            }
        }
        unitAbilities.Clear();
        foreach (var kvp in unitItems)
        {
            foreach (var go in kvp.Value.Values)
            {
                destroyDelegate(go); // Используем переданный делегат
            }
        }
        unitItems.Clear();
    }

    private void CreateAbilityInfoElements(List<AbilityData> abilities, int unitId)
    {
        RectTransform panelRect = abilityPanel.GetComponent<RectTransform>();
        if (panelRect == null) { Debug.LogError("abilityPanel missing RectTransform"); return; }

        unitAbilities[unitId] = new Dictionary<int, GameObject>();
        float yOffset = 0;
        float abilityHeight = 20f;

        for (int i = 0; i < abilities.Count; i++)
        {
            GameObject abilityInfo = instantiateDelegate(abilityInfoPrefab, abilityPanel); // Используем переданный делегат
            abilityInfo.SetActive(false);
            RectTransform rectTransform = abilityInfo.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError($"AbilityInfoPrefab missing RectTransform");
                destroyDelegate(abilityInfo); // Используем переданный делегат
                continue;
            }
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0, yOffset);

            yOffset -= (abilityHeight + verticalSpacing);

            Button button = abilityInfo.GetComponent<Button>();
            TextMeshProUGUI nameText = abilityInfo.transform.Find("AbilityName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = abilityInfo.transform.Find("AbilityCost").GetComponent<TextMeshProUGUI>();

            if (button == null || nameText == null || costText == null)
            {
                Debug.LogError("AbilityInfoPrefab missing Button or TextMeshProUGUI components");
                destroyDelegate(abilityInfo); // Используем переданный делегат
                continue;
            }
            nameText.text = abilities[i].abilityName;
            costText.text = abilities[i].cost.ToString();
            button.GetComponent<Image>().color = defaultColor;

            AbilityData currentAbility = abilities[i];
            button.onClick.AddListener(() => onAbilityButtonClickedDelegate?.Invoke(currentAbility, abilityInfo));

            int instanceID = abilityInfo.GetInstanceID();
            if (!unitAbilities[unitId].TryAdd(instanceID, abilityInfo))
            {
                Debug.LogError($"Error Duplicate ability instance ID for unit {unitId}!");
            }
        }
    }

      private void CreateItemInfoElements(List<ItemData> items, int unitId, UnitData unitData)
    {
        RectTransform panelRect = abilityPanel.GetComponent<RectTransform>();
        if (panelRect == null) { Debug.LogError("abilityPanel missing RectTransform"); return; }

        unitItems[unitId] = new Dictionary<int, GameObject>();
        float yOffset = 0;
        float itemHeight = 20f;

    
        UnitData playerUnitData = unitData.playerUnitData != null ? unitData.playerUnitData : unitData;

        for (int i = 0; i < items.Count; i++)
        {
            GameObject itemInfo = instantiateDelegate(abilityInfoPrefab, abilityPanel); // Используем переданный делегат
            itemInfo.SetActive(false);
            RectTransform rectTransform = itemInfo.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError($"ItemInfoPrefab missing RectTransform");
                destroyDelegate(itemInfo); // Используем переданный делегат
                continue;
            }
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0, yOffset);

            yOffset -= (itemHeight + verticalSpacing);

            Button button = itemInfo.GetComponent<Button>();
            TextMeshProUGUI nameText = itemInfo.transform.Find("AbilityName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = itemInfo.transform.Find("AbilityCost").GetComponent<TextMeshProUGUI>();

            if (button == null || nameText == null || costText == null)
            {
                Debug.LogError("ItemInfoPrefab missing Button or TextMeshProUGUI components");
                destroyDelegate(itemInfo); // Используем переданный делегат
                continue;
            }

            nameText.text = items[i].itemName;
            int itemQuantity = playerUnitData.GetItemQuantity(items[i]);
            costText.text = itemQuantity.ToString();

            button.GetComponent<Image>().color = defaultColor;

            ItemData currentItem = items[i];
            button.onClick.AddListener(() => onItemButtonClickedDelegate?.Invoke(currentItem, itemInfo));

            int instanceID = itemInfo.GetInstanceID();
            if (!unitItems[unitId].TryAdd(instanceID, itemInfo))
            {
                Debug.LogError($"Error Duplicate item instance ID for unit {unitId}!");
            }
        }
    }

    private void UpdateAbilityInfoElements(List<AbilityData> abilities, int unitId)
    {
        if (!unitAbilities.ContainsKey(unitId)) return;

        foreach (var go in unitAbilities[unitId].Values)
        {
            go.SetActive(false);
        }
        foreach (var ability in abilities)
        {
            GameObject matchingAbilityInfo = unitAbilities[unitId].Values.FirstOrDefault(go =>
                go.GetComponent<Button>().onClick.GetPersistentEventCount() > 0 &&
                go.GetComponent<Button>().onClick.GetPersistentTarget(0).Equals(onAbilityButtonClickedDelegate.Target) &&
                go.GetComponent<Button>().onClick.GetPersistentMethodName(0) == onAbilityButtonClickedDelegate.Method.Name &&
                go.GetComponentInChildren<TextMeshProUGUI>(true).text == ability.abilityName
            );

            if (matchingAbilityInfo != null)
            {
                matchingAbilityInfo.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Ability {ability.abilityName} not found in unit {unitId} abilities");
            }
        }
    }

    private void UpdateItemInfoElements(List<ItemData> items, int unitId, UnitData unitData)
    {
        if (!unitItems.ContainsKey(unitId)) return;

        foreach (var go in unitItems[unitId].Values)
        {
            go.SetActive(false);
        }
        foreach (var item in items)
        {
            GameObject matchingItemInfo = unitItems[unitId].Values.FirstOrDefault(go =>
                go.GetComponent<Button>().onClick.GetPersistentEventCount() > 0 &&
                go.GetComponent<Button>().onClick.GetPersistentTarget(0).Equals(onItemButtonClickedDelegate.Target) &&
                go.GetComponent<Button>().onClick.GetPersistentMethodName(0) == onItemButtonClickedDelegate.Method.Name &&
                go.GetComponentInChildren<TextMeshProUGUI>(true).text == item.itemName
            );

            if (matchingItemInfo != null)
            {
                matchingItemInfo.SetActive(true);
                int itemQuantity = unitData.GetItemQuantity(item);
                matchingItemInfo.transform.Find("AbilityCost").GetComponent<TextMeshProUGUI>().text = itemQuantity.ToString();
            }
            else
            {
                Debug.LogWarning($"Item {item.itemName} not found in unit {unitId} items");
            }
        }
    }

    private void ClearAbilityInfoElements(int unitId)
    {
        if (unitAbilities.ContainsKey(unitId))
        {
            foreach (GameObject ability in unitAbilities[unitId].Values)
            {
                destroyDelegate(ability); // Используем переданный делегат
            }
            unitAbilities.Remove(unitId);
        }
    }

    private void ClearItemInfoElements(int unitId)
    {
        if (unitItems.ContainsKey(unitId))
        {
            foreach (GameObject item in unitItems[unitId].Values)
            {
                destroyDelegate(item); // Используем переданный делегат
            }
            unitItems.Remove(unitId);
        }
    }
    public void SelectAbility(AbilityData ability, GameObject clickedButton)
    {
        ResetSelection();
        currentlySelectedAbility = clickedButton;
        currentlySelectedAbility.GetComponent<Image>().color = selectedColor;
    }

    public void SelectItem(ItemData item, GameObject clickedButton)
    {
        ResetSelection();
        currentlySelectedItem = clickedButton;
        currentlySelectedItem.GetComponent<Image>().color = selectedColor;
    }
    public bool HasAbilities()
    {
        return unitAbilities.Count > 0;
    }

    public bool HasItems()
    {
        return unitItems.Count > 0;
    }
    private void OnAbilityButtonClicked(AbilityData ability, GameObject clickedButton)
    {
        onAbilityButtonClickedDelegate?.Invoke(ability, clickedButton);
    }

    private void OnItemButtonClicked(ItemData item, GameObject clickedButton)
    {
        onItemButtonClickedDelegate?.Invoke(item, clickedButton);
    }
}