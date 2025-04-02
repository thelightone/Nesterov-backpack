using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class Cell : MonoBehaviour, IDropHandler
{
    public int x, y;
    public bool isFree;
    public Inventory inventory;
    public TMP_Text cellIndex;
    public TMP_Text cellText;
    public Image image;

    private int _touchId = -1; 

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (_touchId != -1 && Input.touchCount > 0)
        {
            Touch touch = GetTouchById(_touchId);
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _touchId = -1;
            }
        }
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

    public void OnDrop(PointerEventData eventData)
    {
        var dragItem = eventData.pointerDrag.GetComponent<Item>();
        if (inventory.CheckCellFree(this, dragItem.Size))
        {
            dragItem.SetPosition(dragItem, this);
            dragItem.prevCell = this;
        }
        else
        {
            dragItem.SetPosition(dragItem, dragItem.prevCell);
        }
    }
}

