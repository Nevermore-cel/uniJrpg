using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ActionSelectorController : MonoBehaviour
{
    public GameObject actionSelectorInterface;
    public BattleInterfaceController battleInterfaceController;
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;
    public Button abilityButton;
    public Button itemButton;

    private bool interfaceIsActive = false;
    public enum ActionType { Ability, Item, None }
    public ActionType currentActionType = ActionType.None;
    public int currentUnitID;
    private bool isSelectingTarget = false;
    private AbilityData selectedAbility;
    private ItemData selectedItem;
    private UnitData targetUnit;
    private CombatManager combatManager;
    private List<UnitData> allUnits;

    [Header("Action Panel Settings")]
    public Transform abilityPanel;
    public GameObject abilityInfoPrefab;
    public float verticalSpacing = 30f;
    public AbilityItemCreator _abilityItemCreator;
    public ActionSelector actionSelector; // ссылка на ActionSelector
    private GameObject _lastSelectedAbilityButton; //  Последняя подсвеченная кнопка
    private GameObject _lastSelectedItemButton; //  Последняя подсвеченная кнопка

    void Start()
    {
        _abilityItemCreator = new AbilityItemCreator(
            abilityPanel,
            abilityInfoPrefab,
            verticalSpacing,
            defaultColor,
            selectedColor,
            (prefab, parent) => Instantiate(prefab, parent),
            (obj) => Destroy(obj)
        );

        combatManager = FindObjectOfType<CombatManager>();
        if (combatManager != null)
        {
            allUnits = combatManager.GetAllUnits();
        }
        else
        {
            Debug.LogError("CombatManager is null!");
        }

        actionSelectorInterface.SetActive(false);
        interfaceIsActive = false;
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
        if (actionSelector == null)
        {
            actionSelector = FindObjectOfType<ActionSelector>();
            if (actionSelector == null)
            {
                Debug.LogError("ActionSelector not found!");
            }
        }
    }

    public void ShowAbilities()
    {
        ResetSelection();
        currentActionType = ActionType.Ability;
        _abilityItemCreator.HideItems();
        ShowActionSelector();
        HighlightSelectedAbility();
    }

    public void ShowItems()
    {
        ResetSelection();
        currentActionType = ActionType.Item;
        _abilityItemCreator.HideAbilities();
        ShowActionSelector();
        HighlightSelectedItem();
    }

    private void ResetSelection()
    {
        isSelectingTarget = false;
        selectedAbility = null;
        selectedItem = null;
        targetUnit = null;
        _abilityItemCreator.ResetSelection();
    }

    public void SetCurrentUnitId(int unitId)
    {
        currentUnitID = unitId;
    }

    public void SetUnitAbilities(int unitId, List<AbilityData> abilities)
    {
        _abilityItemCreator.SetUnitAbilities(abilities, unitId, OnAbilityButtonClicked);
    }
    public void SetUnitItems(int unitId, List<ItemData> items)
    {
        UnitData unitData = GetUnitData(unitId);

        // Получаем UnitData игрока (если это компаньон)
        UnitData playerUnitData = null;
        if (unitData != null && unitData.playerUnitData != null)
        {
            playerUnitData = unitData.playerUnitData;
        }

        if (playerUnitData != null)
        {
            _abilityItemCreator.SetUnitItems(items, unitId, playerUnitData, OnItemButtonClicked);
        }
        else
        {
            // Если это игрок
            _abilityItemCreator.SetUnitItems(items, unitId, unitData, OnItemButtonClicked);
        }

    }
    public void ShowActionSelector()
    {
        if (!interfaceIsActive && (_abilityItemCreator.HasAbilities() || _abilityItemCreator.HasItems()))
        {
            actionSelectorInterface.SetActive(true);
            interfaceIsActive = true;
            if (currentActionType == ActionType.Ability)
            {
                _abilityItemCreator.ShowAbilitiesInternal();
            }
            else if (currentActionType == ActionType.Item)
            {
                _abilityItemCreator.ShowItemsInternal();
            }
        }
    }
    public void ClearAllAbilityInfo()
    {
        _abilityItemCreator.ClearAllAbilityInfoElements();
    }

    public void HideActionSelector()
    {
        if (interfaceIsActive)
        {
            _abilityItemCreator.HideAbilities();
            _abilityItemCreator.HideItems();
            actionSelectorInterface.SetActive(false);
            interfaceIsActive = false;
            currentActionType = ActionType.None;
            ResetSelection();
        }
    }
        private void OnAbilityButtonClicked(AbilityData ability, GameObject clickedButton)
    {
        _abilityItemCreator.SelectAbility(ability, clickedButton);
        _lastSelectedAbilityButton = clickedButton;
        selectedAbility = ability;
        selectedItem = null;
         if (actionSelector != null)
        {
             actionSelector.TriggerAbilityButton();
             isSelectingTarget = true;
        }
        else
        {
            Debug.LogError("ActionSelector is null!");
        }
        Debug.Log($"Ability '{ability.abilityName}' selected, selecting target");
    }
    public void HighlightSelectedAbility()
    {
        if (actionSelector == null)
        {
            Debug.LogError("ActionSelector is null!");
            return;
        }

        // Сбрасываем цвет предыдущей кнопки
        if (_lastSelectedAbilityButton != null)
        {
            _lastSelectedAbilityButton.GetComponent<Image>().color = defaultColor;
        }
         if (actionSelector.availableAbilities != null && actionSelector.availableAbilities.Count > 0)
        {
             AbilityData ability = actionSelector.availableAbilities[actionSelector.currentAbilityIndex];
             foreach (var kvp in _abilityItemCreator.unitAbilities)
            {
                foreach (var go in kvp.Value.Values)
                {
                    TextMeshProUGUI nameText = go.transform.Find("AbilityName").GetComponent<TextMeshProUGUI>();
                      if (nameText.text == ability.abilityName)
                    {
                        _lastSelectedAbilityButton = go;
                        _lastSelectedAbilityButton.GetComponent<Image>().color = selectedColor;
                         return;
                    }
                }
            }
        }
    }
        private void OnItemButtonClicked(ItemData item, GameObject clickedButton)
    {
        _abilityItemCreator.SelectItem(item, clickedButton);
        _lastSelectedItemButton = clickedButton;
        selectedItem = item;
        selectedAbility = null;
        if (actionSelector != null)
        {
            actionSelector.TriggerAbilityButton();
            isSelectingTarget = true;
        }
        else
        {
            Debug.LogError("ActionSelector is null!");
        }
        Debug.Log($"Item '{item.itemName}' selected, selecting target");
    }
    public void HighlightSelectedItem()
    {
        if (actionSelector == null)
        {
            Debug.LogError("ActionSelector is null!");
            return;
        }

        // Сбрасываем цвет предыдущей кнопки
        if (_lastSelectedItemButton != null)
        {
            _lastSelectedItemButton.GetComponent<Image>().color = defaultColor;
        }

        if (actionSelector.availableItems != null && actionSelector.availableItems.Count > 0)
        {
            ItemData item = actionSelector.availableItems[actionSelector.currentItemIndex];
            foreach (var kvp in _abilityItemCreator.unitItems)
            {
                foreach (var go in kvp.Value.Values)
                {
                    TextMeshProUGUI nameText = go.transform.Find("AbilityName").GetComponent<TextMeshProUGUI>();
                    if (nameText.text == item.itemName)
                    {
                        _lastSelectedItemButton = go;
                        _lastSelectedItemButton.GetComponent<Image>().color = selectedColor;
                        return;
                    }
                }
            }
        }
    }
        
    void Update()
    {
        if (isSelectingTarget && Input.GetMouseButtonDown(0))
        {
            HandleTargetSelection();
            isSelectingTarget = false;
        }
        HighlightSelectedAbility();
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
                        HideActionSelector();
                        combatManager.EndTurn();
                    }
                    else
                    {
                        Debug.LogWarning($"Unit with ID {currentUnitID} has not enough Action Points to use {selectedAbility.abilityName}");
                    }
                }
                else if (selectedItem != null)
                {
                    if (currentUnit != null)
                    {
                        currentUnit.UseItem(selectedItem, targetUnit);
                        
                        bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for item use
                        targetUnit.ApplyItemEffect(selectedItem, targetUnit, isCrit); // Pass isCrit
                         Debug.Log($"Item '{selectedItem.itemName}' Target Unit '{targetUnit.unitName}', ID: {targetUnit.unitID}, Type: {selectedItem.typeAction}");
                        HideActionSelector();
                        combatManager.EndTurn();
                    }
                    else
                    {
                        Debug.LogWarning($"Unit with ID {currentUnitID} has not enough points to use {selectedItem.itemName}");
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
        if (combatManager.CheckCombatEnd()) // Check if combat ended
        {
            return; // Exit if combat ended
        }
        combatManager.EndTurn(); // Guarantee EndTurn() is called
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