using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform _transform;

    public int sizeX, sizeY;
    public Cell cellPrefab;
    public List<Cell> cellsList = new List<Cell>();
    public Cell[,] cells;
    public Item draggedItem;

    private void Start()
    {
        cells = new Cell[sizeX, sizeY];
        CreateNewInventory();
    }

    private void CreateNewInventory()
    {
        Cell[,] cellArray = ConvertListToArray(cellsList, sizeX, sizeY);

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                var newCell = cellArray[x, y];
                newCell.name = x + " " + y;
                newCell.x = x;
                newCell.y = y;
                newCell.isFree = true;
                cells[x, y] = newCell;
                newCell.inventory = this;
                newCell.cellIndex.text = x + " " + y;
            }
        }
    }

    private Cell[,] ConvertListToArray(List<Cell> list, int width, int height)
    {
        if (list.Count != width * height)
        {
            throw new ArgumentException("Список должен содержать ровно " + (width * height) + " элементов.");
        }

        Cell[,] array = new Cell[width, height];

        for (int i = 0; i < list.Count; i++)
        {
            int x = i % width;  // Вычисляем индекс по оси X
            int y = i / width;  // Вычисляем индекс по оси Y

            array[x, y] = list[i];
        }

        return array;
    }

    public bool CheckCellFree(Cell cell, ItemSize size)
    {
        Vector2Int newSize = GetSize(size);
        for (int y = cell.y; y < cell.y + newSize.y; y++)
        {
            for (int x = cell.x; x < cell.x + newSize.x; x++)
            {
                if (x + 1 <= sizeX && y + 1 <= sizeY)
                {
                    if (!cells[x, y].isFree)
                    {
                        return false;
                    }
                }
                if (x + 1 > sizeX || y + 1 > sizeY)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Vector2Int GetSize(ItemSize size)
    {
        Vector2Int newSize = Vector2Int.zero;
        switch (size)
        {
            case ItemSize.Small:
                newSize = Vector2Int.one;
                break;
            case ItemSize.MediumHorizontal:
                newSize = new Vector2Int(2, 1);
                break;
            case ItemSize.MediumVertical:
                newSize = new Vector2Int(1, 2);
                break;
        }
        return newSize;
    }

    public void CellsOccupation(Cell cell, ItemSize size, bool ordered)
    {
        Vector2Int newSize = GetSize(size);
        for (int y = cell.y; y < cell.y + newSize.y; y++)
        {
            for (int x = cell.x; x < cell.x + newSize.x; x++)
            {
                cells[x, y].isFree = ordered;
            }
        }
    }
}
