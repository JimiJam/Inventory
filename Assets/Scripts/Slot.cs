using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class Slot : MonoBehaviour, IDropHandler {
    public int id;
    private Inventory inv;

	// Use this for initialization
	void Start () {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
	}

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

        if (inv.items[id].itemID == -1)
        {
            // Move
            inv.items[droppedItem.slot] = new Item();
            inv.items[id] = droppedItem.item;
            droppedItem.slot = id;
        }
        else if(droppedItem.slot != id)
        {
            // Replace
            Transform item = this.transform.GetChild(0);

            item.GetComponent<ItemData>().slot = droppedItem.slot;
            item.transform.SetParent(inv.slots[droppedItem.slot].transform);
            item.transform.position = inv.slots[droppedItem.slot].transform.position;

            inv.items[droppedItem.slot] = item.GetComponent<ItemData>().item;

            droppedItem.slot = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;

            inv.items[id] = droppedItem.item;
        }
    }
}
