using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public string playerTag = "Player";
    public string companionTag = "Companion";
    public string enemyTag = "Enemy";
    public ActionSelectorController actionSelectorController;
    private List<UnitData> allUnits = new List<UnitData>();
    public List<UnitData> playerTeam = new List<UnitData>();
    public List<UnitData> enemyTeam = new List<UnitData>();
    private int currentUnitIndex = 0;
    private int currentTeamIndex = 0;
    private int totalTurns = 0;
    private bool isCombatActive = false;
    private UnitData currentUnit;
    private int playerTeamTurns;
    private int enemyTeamTurns;
    private BattleInterfaceController battleInterfaceController;
    [SerializeField] private ExitInteract exitInteract; // Добавляем ссылку на ExitInteract
    private UnitData _playerSelectedTarget;

    void Start()
    {
        StartCoroutine(InitializeCombat());
        battleInterfaceController = FindObjectOfType<BattleInterfaceController>();
        if (exitInteract == null)
        {
            Debug.LogError("ExitInteract не назначен в CombatManager!");
        }
    }

    public IEnumerator InitializeCombat()
    {
        // Ждем один кадр
        yield return null;
        // 1. Find and populate teams
        FindAndPopulateTeams();
        // Вывод информации о юнитах
        DebugAllUnits();
        // 2. Start combat
        isCombatActive = true;
        totalTurns = 1;
        playerTeamTurns = playerTeam.Count;
        enemyTeamTurns = enemyTeam.Count;
        Debug.Log("Combat Started!");
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

    public void FindAndPopulateTeams()
    {
        allUnits.Clear();
        playerTeam.Clear();
        enemyTeam.Clear();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag(playerTag);
        GameObject[] companionObjects = GameObject.FindGameObjectsWithTag(companionTag);
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(enemyTag);

        AddUnitsToList(playerObjects, playerTeam);
        AddUnitsToList(companionObjects, playerTeam);
        AddUnitsToList(enemyObjects, enemyTeam);
    }

    private void AddUnitsToList(GameObject[] objects, List<UnitData> team)
    {
        foreach (GameObject obj in objects)
        {
            UnitData unit = obj.GetComponent<UnitData>();
            if (unit != null)
            {
                allUnits.Add(unit);
                team.Add(unit);
            }
        }
    }

    private void StartTurn()
    {
        if (!isCombatActive) return;
        if (playerTeamTurns <= 0 && enemyTeamTurns <= 0)
        {
            EndCombat("Both teams have no units left!");
            return;
        }
        List<UnitData> currentTeam = GetCurrentTeam();
        string teamName = GetCurrentTeamName();

        if (currentTeam.Count == 0)
        {
            Debug.Log($"---{teamName} team has no units---");
            if (teamName == "Player team")
            {
                playerTeamTurns = 0;
                EndCombat("Enemy team wins!");
            }
            else
            {
                enemyTeamTurns = 0;
                EndCombat("Player team wins!");
            }
            return;
        }

        currentUnit = currentTeam[currentUnitIndex];
        // Проверяем, что текущий юнит всё ещё существует и имеет положительное здоровье
        if (currentUnit == null || !currentUnit.gameObject.activeInHierarchy || currentUnit.currentHealth <= 0)
        {
            EndTurn();
            return;
        }

        Debug.Log($"Turn {totalTurns} - {teamName} - {currentUnit.unitName}'s turn (ID: {currentUnit.unitID})");
        // Выводим список живых юнитов в начале каждого хода
        DebugAliveUnits(); // Method call is correct now
        if (currentUnit.gameObject.CompareTag(playerTag) || currentUnit.gameObject.CompareTag(companionTag))
        {
            StartPlayerTurn();
        }
        else
        {
            // Сбрасываем урон для команды игрока перед ходом врагов
            ResetDamageReductionForPlayerTeam();
            // Создаем список живых юнитов
            List<UnitData> aliveUnits = GetAliveUnits();
            SimulateAction(currentUnit, aliveUnits);
            EndTurn();
        }
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
    private void ResetDamageReductionForPlayerTeam()
    {
        foreach (UnitData unit in playerTeam)
        {
            if (unit != null && unit.gameObject.activeInHierarchy)
            {
                unit.ApplyDamageReduction(0f);
            }
        }
        Debug.Log("Damage reduction reset for the player team.");
    }
    private void StartPlayerTurn()
    {
        if (actionSelectorController != null)
        {
            // Очищаем способности перед установкой новых
            actionSelectorController.ClearAllAbilityInfoElements();
            actionSelectorController.SetCurrentUnitId(currentUnit.unitID);
            // Находим Unit компонент, связанный с currentUnit
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
    private void SimulateAction(UnitData unit, List<UnitData> aliveUnits)
    {
        if (unit.abilitiesData == null || unit.GetAbilities().Count == 0)
        {
            Debug.LogWarning($"{unit.unitName} (ID: {unit.unitID}) has no ability to use.");
            return;
        }
        AbilityData ability = unit.GetAbilities()[0]; // (Возможно, вам нужно выбрать заклинание случайным образом)
        Debug.Log($"{unit.unitName} (ID: {unit.unitID}) uses {ability.abilityName}.");
        unit.DeductActionPoints(ability.cost);

        // Важно: выбираем целевого юнита случайным образом из списка живых юнитов
        UnitData targetUnit = FindTarget(unit, aliveUnits);

        if (targetUnit != null && targetUnit.gameObject.activeInHierarchy && targetUnit.currentHealth > 0)
        {
            // Рассчитываем, является ли атака критической
            bool isCrit = unit.CalculateCrit();

            targetUnit.ApplyAbilityEffect(ability, isCrit);
        }
        else
        {
            Debug.LogWarning($"{unit.unitName} (ID: {unit.unitID}) found no valid target.");
        }
    }

    private UnitData FindTarget(UnitData attacker, List<UnitData> aliveUnits)
    {
        List<UnitData> targetTeam;

        if (attacker.gameObject.CompareTag(playerTag) || attacker.gameObject.CompareTag(companionTag))
        {
            targetTeam = enemyTeam;
        }
        else if (attacker.gameObject.CompareTag(enemyTag))
        {
            targetTeam = playerTeam;
        }
        else
        {
            return null; // Или выбросить исключение, если метка не определена.
        }

        if (aliveUnits != null && aliveUnits.Count > 0)
        {
            // Фильтруем список живых юнитов, чтобы убедиться, что они принадлежат к нужной команде для выбора цели
            List<UnitData> validTargets = aliveUnits.Where(unit =>
               (attacker.gameObject.CompareTag(playerTag) || attacker.gameObject.CompareTag(companionTag)) ? targetTeam.Contains(unit) : targetTeam.Contains(unit)
               && unit.currentHealth > 0 // Убедимся, что цель имеет положительное здоровье
            ).ToList();
            if (validTargets.Count > 0)
            {
                return validTargets[UnityEngine.Random.Range(0, validTargets.Count)];
            }
        }
        return null;
    }
    public void EndTurn(UnitData target = null)
    {
        List<UnitData> currentTeam = GetCurrentTeam();

        // Удаляем мертвых юнитов из текущей команды
        currentTeam.RemoveAll(unit => unit == null || !unit.gameObject.activeInHierarchy || unit.currentHealth <= 0);

        if (currentTeam.Count == 0)
        {
            Debug.Log($"---{GetCurrentTeamName()} team has no units---");
            if (GetCurrentTeamName() == "Player team")
            {
                playerTeamTurns = 0;
                EndCombat("Enemy team wins!");
            }
            else
            {
                enemyTeamTurns = 0;
                EndCombat("Player team wins!");
            }
            return;
        }
        // Здесь, перед завершением хода, вызываем метод для отображения информации о крит. ударе.
        DisplayCritInfo(currentUnit);

        currentUnitIndex++;
        if (currentUnitIndex >= currentTeam.Count)
        {
            Debug.Log($"---{GetCurrentTeamName()}'s turn has ended---");
            currentUnitIndex = 0;
            currentTeamIndex = (currentTeamIndex + 1) % 2; // Переключение на следующую команду
            if (currentTeamIndex == 0)
            {
                playerTeamTurns = playerTeam.Count;
            }
            else
            {
                enemyTeamTurns = enemyTeam.Count;
            }
            StartTurn(); // вызов StartTurn
            return;
        }
        //  Проверка на существование юнита
        if (currentUnit != null)
        {
            Unit unitComponent = currentUnit.GetComponent<Unit>();
            if (unitComponent != null)
            {
                unitComponent.SetCanBeSelected(true);
            }
            if (battleInterfaceController != null)
            {
                battleInterfaceController.HideBattleInterface();
            }
            foreach (UnitData unit in allUnits)
            {
                Unit unitComponentAll = unit.GetComponent<Unit>();
                if (unitComponentAll != null)
                {
                    unitComponentAll.SetCanBeSelected(true);
                }
            }
        }
        currentUnit = null; //  Reset current unit
        totalTurns++;
        StartTurn();
    }

    private void DisplayCritInfo(UnitData unit)
    {
        if (unit != null)
        {
            // Рассчитываем критический удар и выводим информацию.
            bool isCrit = unit.CalculateCrit();
            string critMessage = unit.GetCritInfo();
            Debug.Log($"[End of Turn] {critMessage} Current CritChance: {unit.currentCritChance * 100}%");
        }
    }


    public void RemoveUnit(UnitData unit)
    {
        allUnits.Remove(unit);
        playerTeam.Remove(unit);
        enemyTeam.Remove(unit);
    }

    private void EndCombat(string winnerMessage)
    {
        isCombatActive = false;
        Debug.Log(winnerMessage);
        Debug.Log("Combat Ended!");
        if (exitInteract != null)
        {
            exitInteract._active = !exitInteract._active; // Переключаем значение _active
        }
        else
        {
            Debug.LogWarning("ExitInteract is null in CombatManager!");
        }
        if (actionSelectorController != null)
        {
            actionSelectorController.HideActionSelector();
        }
    }

    private string GetCurrentTeamName()
    {
        if (currentTeamIndex == 0)
        {
            return "Player team";
        }
        else
        {
            return "Enemy team";
        }
    }

    public List<UnitData> GetCurrentTeam()
    {
        if (currentTeamIndex == 0)
        {
            return playerTeam;
        }
        else
        {
            return enemyTeam;
        }
    }

    private List<UnitData> GetAliveUnits()
    {
        return allUnits.Where(unit => unit != null && unit.gameObject.activeInHierarchy && unit.currentHealth > 0).ToList();
    }
    // Добавляем метод для получения списка всех юнитов
    public List<UnitData> GetAllUnits()
    {
        return allUnits;
    }
}