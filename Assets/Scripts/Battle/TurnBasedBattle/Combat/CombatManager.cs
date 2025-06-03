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

    [SerializeField]
    private int playerTeamTurns; // Добавлено
    [SerializeField]
    private int enemyTeamTurns;   // Добавлено

    public int PlayerTeamTurns { get => playerTeamTurns; set => playerTeamTurns = value; }
    public int EnemyTeamTurns { get => enemyTeamTurns; set => enemyTeamTurns = value; }

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
        if (endChecker.CheckCombatEnd(playerTeam, enemyTeam, actionSelectorController, previousSceneName))
        {
            return true;
        }
        return false;
    }

    public void StartTurn()
    {
        if (!isCombatActive) return;

        if (CheckCombatEnd()) return;

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
            teamManager.ResetDamageReductionForPlayerTeam(playerTeam);
            SimulateEnemyAction(currentUnit);
            EndTurn();
        }
    }

    private void SimulateEnemyAction(UnitData unit)
    {
        List<UnitData> aliveUnits = GetAliveUnits();
        unitAction.SimulateAction(unit, aliveUnits);
    }

    private void StartPlayerTurn(UnitData currentUnit)
    {
        if (actionSelectorController == null)
        {
            Debug.LogError("ActionSelectorController is null!");
            return;
        }

        actionSelectorController.ClearAllAbilityInfo();
        actionSelectorController.SetCurrentUnitId(currentUnit.unitID);

        Unit unitComponent = currentUnit.GetComponent<Unit>();
        if (unitComponent == null)
        {
            Debug.LogError("Unit component not found on currentUnit!");
            return;
        }
        unitComponent.SetCanBeSelected(false);

        if (battleInterfaceController == null)
        {
            Debug.LogError("BattleInterfaceController is null!");
            return;
        }
        battleInterfaceController.ShowBattleInterface(currentUnit.unitID, currentUnit.gameObject);
        Debug.Log($"Start Turn - {currentUnit.unitName}'s turn");
    }

    public void EndTurn()
    {
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

    public List<UnitData> GetAliveUnits()
    {
        return allUnits.Where(unit => unit != null && unit.gameObject.activeInHierarchy && unit.currentHealth > 0).ToList();
    }
   public List<UnitData> GetCurrentTeam()
    {
        return turnManager.GetCurrentTeam();
    }
}