using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class ActionSelectorController : MonoBehaviour
{
    public GameObject actionSelectorInterface;
    public GameObject abilityInfoPrefab;
    public Transform abilityPanel;
    public float verticalSpacing = 30f;
    public BattleInterfaceController battleInterfaceController;
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;

    private bool interfaceIsActive = false;
    private enum ActionType { Ability, Item, None } // Adding an enum for state checking
    private ActionType currentActionType = ActionType.None;
    private Dictionary<int, Dictionary<int, GameObject>> unitAbilities = new Dictionary<int, Dictionary<int, GameObject>>();
    private Dictionary<int, Dictionary<int, GameObject>> unitItems = new Dictionary<int, Dictionary<int, GameObject>>();
    private GameObject currentlySelectedAbility = null;
    private GameObject currentlySelectedItem = null;
    private bool isSelectingTarget = false;
    private AbilityData selectedAbility;
    private ItemData selectedItem;
    public int currentUnitID;
    private CombatManager combatManager;
    private List<UnitData> allUnits;
    public Button abilityButton;
    public Button itemButton;
    private UnitData playerUnit;
    private UnitData targetUnit; // Added to store the selected target
    private UnitData _playerSelectedTarget;

    private void Start()
    {
        actionSelectorInterface.SetActive(false);
        interfaceIsActive = false;
        combatManager = FindObjectOfType<CombatManager>();
        if (combatManager != null)
        {
            allUnits = combatManager.GetAllUnits();
        }
        else
        {
            Debug.LogError("CombatManager is null!");
        }

        if (abilityButton == null)
        {
            Debug.LogError("Ability Button not found");
        }
        if (itemButton == null)
        {
            Debug.LogError("Item Button not found");
        }
        abilityButton.onClick.AddListener(ShowAbilities);
        itemButton.onClick.AddListener(ShowItems);
        FindPlayerUnit();
    }
    private void FindPlayerUnit()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerUnit = playerObject.GetComponent<UnitData>();
            if (playerUnit == null)
            {
                Debug.LogError("Player UnitData not found in scene!");
            }
        }
        else
        {
            Debug.LogError("Player object not found in scene!");
        }
    }
    public void ShowAbilities()
    {
        ResetSelection();
        currentActionType = ActionType.Ability;
        HideItems();
        ShowActionSelector();
    }
    public void ShowItems()
    {
        ResetSelection();
        currentActionType = ActionType.Item;
        HideAbilities();
        ShowActionSelector();
    }
    private void ResetSelection()
    {
        isSelectingTarget = false;
        currentlySelectedAbility = null;
        currentlySelectedItem = null;
        selectedAbility = null;
        selectedItem = null;
        targetUnit = null; // Reset targetUnit
    }
    public void SetCurrentUnitId(int unitId)
    {
        currentUnitID = unitId;
    }

    public void SetUnitAbilities(int unitId, List<AbilityData> abilities)
    {
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
    public void SetUnitItems(int unitId, List<ItemData> items)
    {
        if (items == null || items.Count == 0)
        {
            Debug.LogWarning($"No items found for unit {unitId}");
            ClearItemInfoElements(unitId);
            return;
        }
        UnitData unitData = GetUnitData(unitId); // Get UnitData for accessing item quantity
        if (!unitItems.ContainsKey(unitId))
        {
            CreateItemInfoElements(items, unitId, unitData);
        }
        else
        {
            UpdateItemInfoElements(items, unitId, unitData);
        }
    }
    public void ShowActionSelector()
    {
        if (!interfaceIsActive && (unitAbilities.Count > 0 || unitItems.Count > 0))
        {
            actionSelectorInterface.SetActive(true);
            interfaceIsActive = true;
            if (currentActionType == ActionType.Ability)
            {
                ShowAbilitiesInternal();
            }
            else if (currentActionType == ActionType.Item)
            {
                ShowItemsInternal();
            }
        }
    }
    private void ShowAbilitiesInternal()
    {
        foreach (var kvp in unitAbilities)
        {
            foreach (var go in kvp.Value.Values)
            {
                go.SetActive(true);
            }
        }
    }
    private void ShowItemsInternal()
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
    public void HideActionSelector()
    {
        if (interfaceIsActive)
        {
            HideAbilities();
            HideItems();
            actionSelectorInterface.SetActive(false);
            interfaceIsActive = false;
            currentActionType = ActionType.None;
            ResetSelection();

        }
    }
    public void ClearAllAbilityInfoElements()
    {
        foreach (var kvp in unitAbilities)
        {
            foreach (var go in kvp.Value.Values)
            {
                Destroy(go);
            }
        }
        unitAbilities.Clear();
        foreach (var kvp in unitItems)
        {
            foreach (var go in kvp.Value.Values)
            {
                Destroy(go);
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
            GameObject abilityInfo = Instantiate(abilityInfoPrefab, abilityPanel);
            abilityInfo.SetActive(false);
            RectTransform rectTransform = abilityInfo.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError($"AbilityInfoPrefab missing RectTransform");
                Destroy(abilityInfo);
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
                Destroy(abilityInfo);
                continue;
            }
            nameText.text = abilities[i].abilityName;
            costText.text = abilities[i].cost.ToString();
            button.GetComponent<Image>().color = defaultColor;

            AbilityData currentAbility = abilities[i];
            button.onClick.AddListener(() => OnAbilityButtonClicked(currentAbility, abilityInfo));

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

        for (int i = 0; i < items.Count; i++)
        {

            GameObject itemInfo = Instantiate(abilityInfoPrefab, abilityPanel);
            itemInfo.SetActive(false);
            RectTransform rectTransform = itemInfo.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError($"ItemInfoPrefab missing RectTransform");
                Destroy(itemInfo);
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
                Destroy(itemInfo);
                continue;
            }
            nameText.text = items[i].itemName;
            int itemQuantity;
            if (unitData != null && unitData.gameObject.CompareTag("Player"))
            {
                itemQuantity = unitData.GetItemQuantity(items[i]);
            }
            else if (playerUnit != null)
            {
                itemQuantity = playerUnit.GetItemQuantity(items[i]);
            }
            else
            {
                itemQuantity = unitData.GetItemQuantity(items[i]);
            }
            costText.text = itemQuantity.ToString();

            button.GetComponent<Image>().color = defaultColor;

            ItemData currentItem = items[i];
            button.onClick.AddListener(() => OnItemButtonClicked(currentItem, itemInfo));

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
                go.GetComponent<Button>().onClick.GetPersistentTarget(0).Equals(this) &&
                go.GetComponent<Button>().onClick.GetPersistentMethodName(0) == nameof(OnAbilityButtonClicked) &&
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
                 go.GetComponent<Button>().onClick.GetPersistentTarget(0).Equals(this) &&
                go.GetComponent<Button>().onClick.GetPersistentMethodName(0) == nameof(OnItemButtonClicked) &&
               go.GetComponentInChildren<TextMeshProUGUI>(true).text == item.itemName
          );
            if (matchingItemInfo != null)
            {
                matchingItemInfo.SetActive(true);
                int itemQuantity;
                if (unitData != null && unitData.gameObject.CompareTag("Player"))
                {
                    itemQuantity = unitData.GetItemQuantity(item);
                }
                else if (playerUnit != null)
                {
                    itemQuantity = playerUnit.GetItemQuantity(item);
                }
                else
                {
                    itemQuantity = unitData.GetItemQuantity(item);
                }
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
                Destroy(ability);
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
                Destroy(item);
            }
            unitItems.Remove(unitId);
        }
    }

    private void OnAbilityButtonClicked(AbilityData ability, GameObject clickedButton)
    {
        if (currentlySelectedAbility != null)
        {
            currentlySelectedAbility.GetComponent<Image>().color = defaultColor;
        }
        if (currentlySelectedItem != null)
        {
            currentlySelectedItem.GetComponent<Image>().color = defaultColor;
        }
        currentlySelectedAbility = clickedButton;
        currentlySelectedAbility.GetComponent<Image>().color = selectedColor;
        isSelectingTarget = true;
        selectedAbility = ability;
        selectedItem = null;
        Debug.Log($"Ability '{ability.abilityName}' selected, selecting target");
    }
    private void OnItemButtonClicked(ItemData item, GameObject clickedButton)
    {
        if (currentlySelectedAbility != null)
        {
            currentlySelectedAbility.GetComponent<Image>().color = defaultColor;
        }
        if (currentlySelectedItem != null)
        {
            currentlySelectedItem.GetComponent<Image>().color = defaultColor;
        }
        currentlySelectedItem = clickedButton;
        currentlySelectedItem.GetComponent<Image>().color = selectedColor;
        isSelectingTarget = true;
        selectedItem = item;
        selectedAbility = null;
        Debug.Log($"Item '{item.itemName}' selected, selecting target");
    }
    private void Update()
    {
        if (isSelectingTarget && Input.GetMouseButtonDown(0))
        {
            HandleTargetSelection();
            isSelectingTarget = false;
        }
    }

    private void HandleTargetSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject target = hit.collider.gameObject;
            targetUnit = target.GetComponent<UnitData>(); // Get target unit data

            if (targetUnit != null)
            {
                UnitData currentUnit = GetUnitData(currentUnitID); // Get the correct currentUnit

                if (selectedAbility != null)
                {
                    if (currentUnit.CanUseAbility(selectedAbility))
                    {
                        currentUnit.DeductActionPoints(selectedAbility.cost);
                        bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for ability use
                        targetUnit.ApplyAbilityEffect(selectedAbility, isCrit); // Pass isCrit
                        Debug.Log($"Ability '{selectedAbility.abilityName}' Target Unit '{targetUnit.unitName}', ID {targetUnit.unitID}, Type: {selectedAbility.typeAction}");
                        // End the turn
                        combatManager.EndTurn();
                    }
                    else
                    {
                        Debug.LogWarning($"Unit with ID {currentUnitID} has not enough Action Points to use {selectedAbility.abilityName}");
                    }
                }
                else if (selectedItem != null)
                {

                    if (currentUnit != null && currentUnit.gameObject.CompareTag("Companion") && playerUnit != null)
                    {
                        if (playerUnit.CanUseItem(selectedItem))
                        {
                            playerUnit.UseItem(selectedItem, currentUnit);
                            bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for item use
                            targetUnit.ApplyItemEffect(selectedItem, targetUnit, isCrit); // Pass isCrit
                            Debug.Log($"Item '{selectedItem.itemName}' Target Unit '{targetUnit.unitName}', ID {targetUnit.unitID}, Type: {selectedItem.typeAction}, used from player's inventory");
                            combatManager.EndTurn();
                        }
                        else
                        {
                            Debug.LogWarning($"Unit with ID {currentUnitID} has not enough  {selectedItem.itemName}");
                        }
                    }
                    else if (currentUnit != null && currentUnit.CanUseItem(selectedItem))
                    {
                        currentUnit.UseItem(selectedItem);
                        bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for item use
                        targetUnit.ApplyItemEffect(selectedItem, targetUnit, isCrit); // Pass isCrit
                        Debug.Log($"Item '{selectedItem.itemName}' Target Unit '{targetUnit.unitName}', ID {targetUnit.unitID}, Type: {selectedItem.typeAction}");
                        combatManager.EndTurn();
                    }
                    else
                    {
                        Debug.LogWarning($"Unit with ID {currentUnitID} has not enough  {selectedItem.itemName}");
                    }

                }
            }
            else
            {
                Debug.LogWarning("Target clicked has no UnitData Component");
            }
        }
        else
        {
            Debug.Log("Clicked on nothing, target selection cancelled");
        }

        isSelectingTarget = false;
        selectedAbility = null;
        selectedItem = null;
        targetUnit = null;

    }

    private UnitData GetUnitData(int unitId)
    {
        if (allUnits != null)
        {
            foreach (UnitData unit in allUnits)
            {
                if (unit != null && unit.unitID == unitId)
                {
                    return unit;
                }
            }
        }
        Debug.LogWarning($"No unit found with ID {unitId}");
        return null;
    }
}