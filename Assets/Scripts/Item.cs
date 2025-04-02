
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _positionItem;
    private int _touchId = -1;

    public ItemSize Size;
    public Cell prevCell;
    public Cell startCell;
    public Inventory inventory;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private Touch GetTouchById(int id)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).fingerId == id)
            {
                return Input.GetTouch(i);
            }
        }
        return default(Touch);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 0.7f;
        _canvasGroup.blocksRaycasts = false;
        inventory.draggedItem = this;
        if (prevCell != null)
            inventory.CellsOccupation(prevCell, Size, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_touchId == -1 && Input.touchCount > 0)
        {
            _touchId = Input.GetTouch(0).fingerId;
        }

        if (_touchId != -1)
        {
            Touch touch = GetTouchById(_touchId);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchPosition = touch.position;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform.parent as RectTransform,
                    touchPosition,
                    null,
                    out Vector2 localPoint
                );

                localPoint.y -= Screen.height / 2;
                localPoint.x += Screen.width / 2;

                _rectTransform.anchoredPosition = localPoint;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        inventory.draggedItem = null;

        _touchId = -1;
    }


    public Vector2Int GetSize()
    {
        Vector2Int size;
        switch (Size)
        {
            case ItemSize.Small:
                return size = Vector2Int.one;
            case ItemSize.MediumHorizontal:
                return size = new Vector2Int(2, 1);
            case ItemSize.MediumVertical:
                return size = new Vector2Int(1, 2);
        }
        return size = Vector2Int.zero;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragItem = eventData.pointerDrag.GetComponent<Item>();
        SetPosition(dragItem, dragItem.prevCell);
    }

    public void SetPosition(Item item, Cell cell)
    {
        if (cell == null)
        {
            return;
        }
        item.transform.SetParent(cell.transform);
        item.transform.localPosition = Vector3.zero;
        var itemSize = item.GetSize();
        var newPos = item.transform.localPosition;
        if (itemSize.x > 1)
        {
            newPos.x += itemSize.x * 24;
        }
        if (itemSize.y > 1)
        {
            newPos.y -= itemSize.y * 24;
        }
        item.transform.localPosition = newPos;
        item.transform.SetParent(inventory.transform);
        inventory.CellsOccupation(cell, item.Size, false);
    }
}
