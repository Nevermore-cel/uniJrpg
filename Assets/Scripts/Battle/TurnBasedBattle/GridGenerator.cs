using UnityEngine;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public float cellSize;
    public float gridSpacing;
    public float cellSpacing;

    public GameObject cellPrefab;
    public string enemySpawnPointTag = "enemySpawnPoint";
    public string playerTag = "Player";
    public string companionTag = "Companion";


    public List<Vector3> occupiedPositions = new List<Vector3>();

    private SpawnManager spawnManager;
    private CompanionManager companionManager;

    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        companionManager = FindObjectOfType<CompanionManager>();
         GenerateGrid(transform.position, Vector3.zero);
        GenerateGrid(transform.position + new Vector3(gridSpacing, 0, 0), Vector3.zero);

        if (spawnManager != null)
        {
             
           PositionPlayerAndCompanions();
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(enemySpawnPointTag);
           SortSpawnPoints(spawnPoints);
          foreach (GameObject spawnPoint in spawnPoints)
            {
                Vector3 cellPosition = FindFreeCellPosition(transform.position + new Vector3(gridSpacing, 0, 0), Vector3.zero);
               spawnPoint.transform.position = cellPosition;
                 occupiedPositions.Add(cellPosition);
          }
             spawnManager.SpawnEnemies();
         }
       
        Debug.Log("Companion states applied.");
    }
    private void GenerateGrid(Vector3 origin, Vector3 offset)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 cellPosition = origin + new Vector3((x * cellSize + cellSize / 2) + x * cellSpacing, 0, (y * cellSize + cellSize / 2) + y * cellSpacing) + offset;
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                cell.transform.localScale = new Vector3(cellSize, 1, cellSize);
                cell.name = $"Cell_{x}_{y}";
           }
        }
   }
    public Vector3 FindFreeCellPosition(Vector3 origin, Vector3 offset)
   {
        while (true)
        {
            int randomX = Random.Range(0, gridWidth);
           int randomY = Random.Range(0, gridHeight);
            Vector3 cellPosition = origin + new Vector3((randomX * cellSize + cellSize / 2) + randomX * cellSpacing, 0, (randomY * cellSize + cellSize / 2) + randomY * cellSpacing) + offset;
            if (!occupiedPositions.Contains(cellPosition))
            {
               return cellPosition;
           }
        }
  }

   private void SortSpawnPoints(GameObject[] spawnPoints)
    {
       System.Array.Sort(spawnPoints, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
    }
    private void PositionPlayerAndCompanions()
    {
    
        if (companionManager == null)
        {
            Debug.LogError("CompanionManager not found!");
            return;
        }

        List<UnitData> allUnits = companionManager.allUnits;

        foreach(UnitData unit in allUnits){
            if(unit != null && unit.gameObject != null){
                Vector3 cellPosition = GetCellPosition(unit.startGridPosition);
                  unit.gameObject.transform.position = cellPosition;
                  occupiedPositions.Add(cellPosition);
            }

        }
    }
    public Vector3 GetCellPosition(Vector2Int gridPosition)
    {
         return transform.position + new Vector3(
            (gridPosition.x * cellSize + cellSize / 2) + gridPosition.x * cellSpacing,
             0,
            (gridPosition.y * cellSize + cellSize / 2) + gridPosition.y * cellSpacing
        );
    }
}