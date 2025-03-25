using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public string playerTag = "Player";
    public string companionTag = "Companion";
    public string enemyTag = "Enemy";
    public ActionSelectorController actionSelectorController;
    public BattleInterfaceController battleInterfaceController;
    [SerializeField] private string previousSceneName;  // Название предыдущей сцены

    public List<UnitData> allUnits = new List<UnitData>();
    public List<UnitData> playerTeam = new List<UnitData>();
    public List<UnitData> enemyTeam = new List<UnitData>();

    private CombatTurnManager turnManager;
    private CombatTeamManager teamManager;
    private CombatUnitAction unitAction;
    private CombatEndChecker endChecker;

    private bool isCombatActive = false;

    public int playerTeamTurns; // Добавлено
    public int enemyTeamTurns;   // Добавлено

    void Awake()
    {
        turnManager = new CombatTurnManager(this);
        teamManager = new CombatTeamManager(this);
        unitAction = new CombatUnitAction(this);
        endChecker = new CombatEndChecker(this);
    }
    void Start()
    {
        StartCoroutine(InitializeCombat());
        if (battleInterfaceController == null)
        {
            battleInterfaceController = FindObjectOfType<BattleInterfaceController>();
        }
    }

    public IEnumerator InitializeCombat()
    {
        yield return null;
        teamManager.FindAndPopulateTeams(playerTag, companionTag, enemyTag);
        DebugAllUnits();
        StartCombat();
    }

    private void StartCombat()
    {
        isCombatActive = true;
        turnManager.ResetTurns(playerTeam.Count, enemyTeam.Count);
        Debug.Log("Combat Started!");
        StartTurn();
    }
    public bool CheckCombatEnd()
    {
        if (endChecker.CheckCombatEnd(playerTeam, enemyTeam, actionSelectorController, previousSceneName)) // Передаем название сцены
        {
            return true;
        }
        return false;
    }
    public void StartTurn()
    {
        if (!isCombatActive) return;

        if (endChecker.CheckCombatEnd(playerTeam, enemyTeam, actionSelectorController, previousSceneName)) return;

        UnitData currentUnit = turnManager.GetCurrentUnit();
        if (currentUnit == null)
        {
            return;
        }

        Debug.Log($"Turn {turnManager.TotalTurns} - {turnManager.GetCurrentTeamName()} - {currentUnit.unitName}'s turn (ID: {currentUnit.unitID})");
        DebugAliveUnits();

        if (teamManager.IsPlayerOrCompanion(currentUnit))
        {
            StartPlayerTurn(currentUnit);
        }
        else
        {
            ResetDamageReductionForPlayerTeam();
            List<UnitData> aliveUnits = GetAliveUnits();
            SimulateAction(currentUnit, aliveUnits);
            EndTurn();
        }
    }
    private void SimulateAction(UnitData unit, List<UnitData> aliveUnits)
    {
        unitAction.SimulateAction(unit, aliveUnits);
    }
    private void ResetDamageReductionForPlayerTeam()
    {
        teamManager.ResetDamageReductionForPlayerTeam(playerTeam);
    }
    private void StartPlayerTurn(UnitData currentUnit)
    {
        if (actionSelectorController != null)
        {
            actionSelectorController.ClearAllAbilityInfo();
            actionSelectorController.SetCurrentUnitId(currentUnit.unitID);

            Unit unitComponent = currentUnit.GetComponent<Unit>();
            if (unitComponent != null)
            {
                unitComponent.SetCanBeSelected(false);
            }
            else
            {
                Debug.LogError("Unit component not found on currentUnit!");
            }

            if (battleInterfaceController != null)
            {
                battleInterfaceController.ShowBattleInterface(currentUnit.unitID, currentUnit.gameObject);
            }
            else
            {
                Debug.LogError("BattleInterfaceController is null!");
            }
            Debug.Log($"Start Turn - {currentUnit.unitName}'s turn");
        }
        else
        {
            Debug.LogError("ActionSelectorController is null!");
        }
    }
    public void EndTurn(UnitData target = null)
    {
        UnitData currentUnit = turnManager.GetCurrentUnit();
        turnManager.NextTurn(actionSelectorController, battleInterfaceController);
        StartTurn();
    }
    private void DebugAllUnits()
    {
        Debug.Log("--- All Units ---");
        foreach (UnitData unit in allUnits)
        {
            if (unit != null)
            {
                Debug.Log($"Unit: {unit.unitName} (ID: {unit.unitID})");
            }
        }
        Debug.Log("	");
    }
    public void DebugAliveUnits()
    {
        Debug.Log("--- Alive Units ---");
        List<UnitData> aliveUnits = GetAliveUnits();
        foreach (UnitData unit in aliveUnits)
        {
            if (unit != null)
            {
                Debug.Log($"Unit: {unit.unitName} (ID: {unit.unitID})");
            }
        }
        Debug.Log("	");
    }
    public void EndCombat(string winnerMessage)
    {
        isCombatActive = false;
        Debug.Log(winnerMessage);
        Debug.Log("Combat Ended!");
        if (actionSelectorController != null)
        {
            actionSelectorController.HideActionSelector();
        }
    }
    public List<UnitData> GetAliveUnits()
    {
        return allUnits.Where(unit => unit != null && unit.gameObject.activeInHierarchy && unit.currentHealth > 0).ToList();
    }
    public List<UnitData> GetAllUnits()
    {
        return allUnits;
    }
    public void RemoveUnit(UnitData unit)
    {
        allUnits.Remove(unit);
        playerTeam.Remove(unit);
        enemyTeam.Remove(unit);
    }
    public List<UnitData> GetCurrentTeam()
    {
        return turnManager.GetCurrentTeam();
    }
}