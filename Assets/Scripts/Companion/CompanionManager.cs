using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class CompanionManager : MonoBehaviour
{
    public string companionTag = "Companion";
    public string playerTag = "Player";

    public List<UnitData> allUnits = new List<UnitData>();
    public Dictionary<int, UnitData> unitDataById = new Dictionary<int, UnitData>();

    private GridGenerator gridGenerator;
    private bool isSceneLoading = false; // Flag to prevent duplicate calls
    private UnitData playerUnit;

    void Start()
    {
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateUnitsList();

        // Attach position change handler
        FindPlayerUnit();
    }
    private void FindPlayerUnit()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            playerUnit = playerObject.GetComponent<UnitData>();
            if (playerUnit != null)
            {
                playerUnit.OnPositionChanged += HandlePlayerPositionChange;
            }
        }
    }
    public void UpdateUnitsList()
    {
        allUnits.Clear();
        unitDataById.Clear();

        GameObject[] playerAndCompanionObjects = GameObject.FindGameObjectsWithTag(companionTag);
        playerAndCompanionObjects = playerAndCompanionObjects.Concat(GameObject.FindGameObjectsWithTag(playerTag)).ToArray();

        foreach (GameObject unitObject in playerAndCompanionObjects)
        {
            UnitData unitData = unitObject.GetComponent<UnitData>();
            if (unitData != null)
            {
                allUnits.Add(unitData);
                unitDataById[unitData.unitID] = unitData;
                unitData.Initialize();
                Debug.Log($"Unit found: ID={unitData.unitID}, isActive={unitData.isActive}");
            }
        }
    }

    public void ChangeActivity(UnitData unit)
    {
        unit.isActive = !unit.isActive;
        Debug.Log($"Changed unit activity: ID={unit.unitID}, isActive={unit.isActive}");
    }

    public List<UnitData> UpdateActiveUnits()
    {
        List<UnitData> activeUnits = new List<UnitData>();
        foreach (UnitData unit in allUnits)
        {
            if (unit.isActive)
            {
                activeUnits.Add(unit);
            }
        }
        return activeUnits;
    }

    public void PositionUnitsOnGrid()
    {
        if (gridGenerator == null)
        {
            Debug.Log("No GridGenerator found");
            return;
        }
        gridGenerator.occupiedPositions.Clear();

        List<UnitData> activeUnits = UpdateActiveUnits();
        foreach (UnitData unit in activeUnits)
        {
            if (unit != null && unit.gameObject != null)
            {
                Vector3 cellPosition = gridGenerator.GetCellPosition(unit.startGridPosition);
                unit.gameObject.transform.position = cellPosition;
                gridGenerator.occupiedPositions.Add(cellPosition);
            }
        }
    }

    private bool IsCellOccupied(Vector3 cellPosition)
    {
        foreach (UnitData unit in allUnits)
        {
            if (unit != null && unit.gameObject != null && unit.gameObject.transform.position == cellPosition)
            {
                return true;
            }
        }
        return false;
    }
    public void RefreshActiveUnits()
    {
        // Update the list of active units
        UpdateActiveUnits();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isSceneLoading)
        {
            return; //Prevent duplicate calls
        }
        isSceneLoading = true;
        GameObject[] unitObjects = GameObject.FindGameObjectsWithTag(companionTag);
        unitObjects = unitObjects.Concat(GameObject.FindGameObjectsWithTag(playerTag)).ToArray();

         foreach (GameObject unitObject in unitObjects)
        {
            UnitData unitData = unitObject.GetComponent<UnitData>();
            if (unitData != null)
            {
                if (unitDataById.ContainsKey(unitData.unitID))
                {
                      unitData.isActive = unitDataById[unitData.unitID].isActive;
                      unitData.selectedAbilitiesIndexes = new List<int>(unitDataById[unitData.unitID].selectedAbilitiesIndexes);
                        unitData.currentHealth = unitDataById[unitData.unitID].currentHealth;
                      unitData.maxActionPoints = unitDataById[unitData.unitID].maxActionPoints;
                      unitData.currentActionPoints = unitDataById[unitData.unitID].currentActionPoints;
                       unitData.maxHealth = unitDataById[unitData.unitID].maxHealth;
                     unitData.attackDamage = unitDataById[unitData.unitID].attackDamage;
                      unitData.startGridPosition = unitDataById[unitData.unitID].startGridPosition;


                     // Load item quantities
                     unitData.itemQuantities.Clear();
                    foreach (var itemQuantity in unitDataById[unitData.unitID].itemQuantities)
                        {
                            unitData.itemQuantities.Add(new ItemQuantity { itemIndex = itemQuantity.itemIndex, quantity = itemQuantity.quantity });
                    }
                }
            }
        }

        allUnits.Clear();
        unitDataById.Clear();
        gridGenerator = FindObjectOfType<GridGenerator>();

        if (gridGenerator != null)
        {
            UpdateUnitsList();
            PositionUnitsOnGrid();
        }
        FindPlayerUnit(); // Attach position change handler on new scene
        isSceneLoading = false;
    }
    private void HandlePlayerPositionChange(Vector3 newPosition)
    {
        Debug.Log("Player moved to: " + newPosition);

        if (gridGenerator != null)
        {
            Vector3 expectedPosition = gridGenerator.GetCellPosition(playerUnit.startGridPosition);
            if (newPosition != expectedPosition)
            {
                playerUnit.transform.position = expectedPosition;
                Debug.Log("Player position corrected to: " + expectedPosition);
            }
        }
    }
}