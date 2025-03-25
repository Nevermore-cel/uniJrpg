using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AreaSelector : MonoBehaviour
{
    public Color selectionColor = Color.green;
    public GridGenerator gridGenerator; // Ссылка на GridGenerator
    private List<GameObject> _selectedCells = new List<GameObject>(); // Список выделенных клеток
    private bool isSelectingArea = false;

    public void StartAreaSelection(AbilityData ability, Vector3 origin)
    {
        ClearSelection();
        isSelectingArea = true;
        SelectArea(ability, origin);
    }

    public void SelectArea(AbilityData ability, Vector3 origin)
    {
        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator is null!");
            return;
        }

        int rangeX = ability.rangeX;
        int rangeY = ability.rangeY;

        // Преобразуем мировые координаты в индексы сетки
        Vector2Int originIndices = WorldToGridIndices(origin);

        // Перебираем все клетки в пределах радиуса действия
        for (int x = -rangeX; x <= rangeX; x++)
        {
            for (int y = -rangeY; y <= rangeY; y++)
            {
                // Вычисляем индексы текущей клетки
                int currentX = originIndices.x + x;
                int currentY = originIndices.y + y;

                // Проверяем, что индексы находятся в пределах сетки
                if (currentX >= 0 && currentX < gridGenerator.gridWidth && currentY >= 0 && currentY < gridGenerator.gridHeight)
                {
                    // Получаем позицию клетки в мировых координатах
                    Vector3 cellPosition = gridGenerator.GetCellPosition(new Vector2Int(currentX, currentY));

                    // Получаем GameObject клетки
                    GameObject cell = GetCellAtPosition(cellPosition);

                    if (cell != null)
                    {
                        // Выделяем клетку цветом
                        HighlightCell(cell);
                        _selectedCells.Add(cell);
                    }
                }
            }
        }
    }

    public void ClearSelection()
    {
        if (isSelectingArea == false) return;
        foreach (GameObject cell in _selectedCells)
        {
            ResetCellColor(cell);
        }
        _selectedCells.Clear();
        isSelectingArea = false;
    }

    private void HighlightCell(GameObject cell)
    {
        // Выделяем клетку цветом
        cell.GetComponent<Renderer>().material.color = selectionColor;
    }

    private void ResetCellColor(GameObject cell)
    {
        // Возвращаем клетке исходный цвет
        cell.GetComponent<Renderer>().material.color = Color.white; // или другой цвет по умолчанию
    }
    // Helper method to convert world position to grid indices
    private Vector2Int WorldToGridIndices(Vector3 worldPosition)
    {
        // Calculate grid indices based on world position and grid parameters
        int gridX = Mathf.RoundToInt((worldPosition.x - transform.position.x) / (gridGenerator.cellSize + gridGenerator.cellSpacing));
        int gridY = Mathf.RoundToInt((worldPosition.z - transform.position.z) / (gridGenerator.cellSize + gridGenerator.cellSpacing));

        return new Vector2Int(gridX, gridY);
    }

    // Helper method to get the cell at a specific world position
    private GameObject GetCellAtPosition(Vector3 position)
    {
        // Use a simple approach to find the cell GameObject based on position
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.name.StartsWith("Cell_"))
            {
                return collider.gameObject;
            }
        }
        return null;
    }
}