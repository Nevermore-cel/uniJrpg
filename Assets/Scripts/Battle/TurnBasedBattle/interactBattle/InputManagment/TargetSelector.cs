using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TargetSelector : MonoBehaviour
{
    private CombatManager combatManager;
    private List<UnitData> selectableTargets;
    private int currentTargetIndex = 0;
    private UnitData _selectedTarget;
    public UnitData SelectedTarget => _selectedTarget;
    public Attack attack;
    public bool isSelectingTarget = false;
    public AbilityData selectedAbility;
    public ItemData selectedItem;
    public ActionSelectorController actionSelectorController;

    [Header("Effects")]
    public GameObject damageEffectPrefab;
    public Color fireDamageColor = Color.red;
    public Color iceDamageColor = Color.blue;
    public Color lightningDamageColor = Color.yellow;
    public Color lightDamageColor = Color.white;
    public Color darkDamageColor = Color.black;
    public Color pureDamageColor = Color.magenta;
    public Color piercingDamageColor = Color.gray;
    public Color slashingDamageColor = Color.green;
    public Color bludgeoningDamageColor = Color.black;
    public Color windDamageColor = Color.cyan;
    public Color attackDamageColor = Color.white;
    public Color healColor = Color.green;

    [Header("Target Indicator")]
    public GameObject targetIndicatorPrefab;
    public float indicatorOffsetY = 1.5f;

    private GameObject currentIndicatorInstance;

    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();
        actionSelectorController = FindObjectOfType<ActionSelectorController>();
        if (combatManager == null)
        {
            Debug.LogError("CombatManager is null!");
        }
    }

    public void InitializeTargets(bool isSelectingEnemies)
    {
        selectableTargets = isSelectingEnemies ?
            combatManager.enemyTeam.Where(unit => unit.IsActive()).ToList() :
            combatManager.playerTeam.Where(unit => unit.IsActive()).ToList();

        if (selectableTargets.Count == 0)
        {
            Debug.LogWarning("No selectable targets available.");
            _selectedTarget = null;
            return;
        }

        currentTargetIndex = 0;
        HighlightTarget(selectableTargets[currentTargetIndex]);
        _selectedTarget = selectableTargets[currentTargetIndex];
        attack._selectedTarget = _selectedTarget;

        ShowTargetIndicator(_selectedTarget);
    }

    public void SelectNextTarget()
    {
        if (selectableTargets == null || selectableTargets.Count == 0 || isSelectingTarget == false) return;

        RemoveHighlight(selectableTargets[currentTargetIndex]);
        currentTargetIndex = (currentTargetIndex + 1) % selectableTargets.Count;
        HighlightTarget(selectableTargets[currentTargetIndex]);
        _selectedTarget = selectableTargets[currentTargetIndex];
        attack._selectedTarget = _selectedTarget;

        ShowTargetIndicator(_selectedTarget);
        Debug.Log($"Selected target: {selectableTargets[currentTargetIndex].unitName}, ID: {selectableTargets[currentTargetIndex].unitID}");
    }

    public void SelectPreviousTarget()
    {
        if (selectableTargets == null || selectableTargets.Count == 0 || isSelectingTarget == false) return;

        RemoveHighlight(selectableTargets[currentTargetIndex]);
        currentTargetIndex--;
        if (currentTargetIndex < 0)
        {
            currentTargetIndex = selectableTargets.Count - 1;
        }
        HighlightTarget(selectableTargets[currentTargetIndex]);
        _selectedTarget = selectableTargets[currentTargetIndex];
        attack._selectedTarget = _selectedTarget;

        ShowTargetIndicator(_selectedTarget);
        Debug.Log($"Selected target: {selectableTargets[currentTargetIndex].unitName}, ID: {selectableTargets[currentTargetIndex].unitID}");
    }

    public void ConfirmTarget()
    {

        if (!isSelectingTarget)
        {
            Debug.LogWarning("Не в режиме выбора цели");
            return;
        }
        if (_selectedTarget == null)
        {
            Debug.LogWarning("Нет выбранной цели");
            return;
        }

        UnitData currentUnit = GetCurrentUnitData(actionSelectorController.currentUnitID);
        if (currentUnit == null)
        {
            Debug.LogWarning($"Unit with ID {actionSelectorController.currentUnitID} not found!");
            return;
        }

        if (selectedAbility != null)
        {
            if (currentUnit.CanUseAbility(selectedAbility))
            {
                currentUnit.DeductActionPoints(selectedAbility.cost);
                bool isCrit = currentUnit.CalculateCrit();

                SpawnDamageEffect(_selectedTarget.transform.position, GetDamageColor(selectedAbility.typeAction));

                _selectedTarget.ApplyAbilityEffect(selectedAbility, isCrit);
            }
        }
        else if (selectedItem != null)
        {
            if (currentUnit.CanUseItem(selectedItem))
            {
                currentUnit.UseItem(selectedItem, _selectedTarget);
                bool isCrit = currentUnit.CalculateCrit();

                SpawnDamageEffect(_selectedTarget.transform.position, GetDamageColor(selectedItem.typeAction));

                _selectedTarget.ApplyItemEffect(selectedItem, _selectedTarget, isCrit);
            }
        }
        else
        {
            bool isCrit = currentUnit.CalculateCrit();
            SpawnDamageEffect(_selectedTarget.transform.position, GetDamageColor(currentUnit.attackType));

            _selectedTarget.TakeDamage(currentUnit.attackDamage, currentUnit.attackType, currentUnit.unitName, isCrit);
        }

        Debug.Log($"{currentUnit.unitName} Attack {_selectedTarget.unitName}, ID: {_selectedTarget.unitID}");
        isSelectingTarget = false;
        HideTargetIndicator();
        combatManager.EndTurn();
        actionSelectorController.HideActionSelector();
    }

    private void HighlightTarget(UnitData target)
    {
        ShowTargetIndicator(target);
        Debug.Log($"Highlighting target: {target.unitName}");
    }
    private void RemoveHighlight(UnitData target)
    {
        HideTargetIndicator();
        Debug.Log($"RemoveHighlight: {target.unitName}");
    }
    private UnitData GetCurrentUnitData(int unitId)
    {
        List<UnitData> allUnits = combatManager.GetAllUnits();
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

    private void SpawnDamageEffect(Vector3 position, Color color)
    {
        if (damageEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(damageEffectPrefab, position, Quaternion.identity);
            Renderer renderer = effectInstance.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material newMaterial = new Material(Shader.Find("Standard"));
                newMaterial.color = color;
                renderer.material = newMaterial;
            }
            else
            {
                Debug.LogWarning("Damage effect prefab doesn't have a Renderer!");
            }

            Destroy(effectInstance, 2f);
        }
        else
        {
            Debug.LogWarning("Damage effect prefab is not assigned!");
        }
    }

    private Color GetDamageColor(ActionType type)
    {
        switch (type)
        {
            case ActionType.fire: return fireDamageColor;
            case ActionType.ice: return iceDamageColor;
            case ActionType.lightning: return lightningDamageColor;
            case ActionType.light: return lightDamageColor;
            case ActionType.dark: return darkDamageColor;
            case ActionType.pure: return pureDamageColor;
            case ActionType.piercing: return piercingDamageColor;
            case ActionType.slashing: return slashingDamageColor;
            case ActionType.wind: return windDamageColor;
            case ActionType.attack: return attackDamageColor;
            case ActionType.heal: return healColor;
            default: return Color.white;
        }
    }

    private void ShowTargetIndicator(UnitData target)
    {
        if (targetIndicatorPrefab == null || target == null)
        {
            Debug.LogWarning("Target indicator prefab is not assigned or target is null!");
            return;
        }

        if (currentIndicatorInstance != null)
        {
            Destroy(currentIndicatorInstance);
        }

        Vector3 indicatorPosition = target.transform.position + Vector3.up * indicatorOffsetY;

        currentIndicatorInstance = Instantiate(targetIndicatorPrefab, indicatorPosition, Quaternion.identity);
        currentIndicatorInstance.transform.SetParent(target.transform, true);
    }

    private void HideTargetIndicator()
    {
        if (currentIndicatorInstance != null)
        {
            Destroy(currentIndicatorInstance);
            currentIndicatorInstance = null;
        }
    }
}