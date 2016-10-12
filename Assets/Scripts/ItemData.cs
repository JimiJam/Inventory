using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Item item;
    public int amount; // for stackable items
    public int slot;

    private Inventory inv;
    private Tooltip tooltip;
    private Vector2 offset;
    private ContextMenu contextmenu;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();

        contextmenu = inv.GetComponent<ContextMenu>();
        tooltip = inv.GetComponent<Tooltip>();
    }

    // Drag And Drop functions
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Make sure that slot contain Item
        if (item != null)
        {
            this.transform.SetParent(this.transform.parent.parent);
            this.transform.position = eventData.position;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            this.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(inv.slots[slot].transform);
        this.transform.position = inv.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                contextmenu.Deactivate();
                contextmenu.GetContext(slot);
            }
            if (Input.GetMouseButtonDown(0))
            {
                contextmenu.Deactivate();
            }
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }
}
