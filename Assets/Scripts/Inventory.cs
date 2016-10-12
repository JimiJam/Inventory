using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

    GameObject InventoryPanel;
    GameObject slotPanel;
    ItemDatabase database;
    int slotAmount;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    // This is just for test
    public RectTransform AddItemButton;
    public InputField AddItemField;

    public RectTransform DelItemButtonBySlot;
    public InputField DelItemBySlotField;

    public RectTransform DelItemButtonBySlotAndCount;
    public InputField SlotField;
    public InputField CountField;

    public RectTransform DelItemButtonByTypeAndCount;
    public InputField DTypeField;
    public InputField DCountField;

    public RectTransform SortInvByPrice;
    public RectTransform SortInvByType;

    void Start()
    {
        // This is just for test
        AddItemButton.gameObject.SetActive(true);
        DelItemButtonBySlot.gameObject.SetActive(true);
        DelItemButtonBySlotAndCount.gameObject.SetActive(true);
        DelItemButtonByTypeAndCount.gameObject.SetActive(true);
        SortInvByPrice.gameObject.SetActive(true);
        SortInvByType.gameObject.SetActive(true);

        database = GetComponent<ItemDatabase>();

        slotAmount = 16;
        InventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = InventoryPanel.transform.FindChild("Slot Panel").gameObject; 

        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(slotPanel.transform);
        }

        AddItem(1);
        AddItem(2);
        AddItem(3);
        AddItem(4);
        AddItem(4);
        AddItem(4);
        AddItem(4);
        AddItem(4);
        AddItem(5);
        AddItem(6);
        AddItem(2);
        AddItem(1);
        AddItem(1);
    }

    public void AddButton()
    {
        int id = Convert.ToInt32(AddItemField.text);
        AddItem(id);
    }

    public void DelSlotButton()
    {
        int id = Convert.ToInt32(DelItemBySlotField.text);
        DelItemBySlot(id);
    }

    public void DelSlotCountButton()
    {
        int id = Convert.ToInt32(SlotField.text);
        int count = Convert.ToInt32(CountField.text);
        DelItemBySlotAndCount(id, count);
    }

    public void DelTypeCountButton()
    {
        string type = DTypeField.text.ToString();
        int count = Convert.ToInt32(DCountField.text);
        switch (type)
        {
            case "Equipment":
                DelItemByTypeAndCount(Item.ItemType.Equipment, count);
                break;
            case "Consumable":
                DelItemByTypeAndCount(Item.ItemType.Consumable, count);
                break;
            case "Trash":
                DelItemByTypeAndCount(Item.ItemType.Trash, count);
                break;
            case "Quest":
                DelItemByTypeAndCount(Item.ItemType.Quest, count);
                break;
            default:
                return;
        }
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);

        // If id wrong
        if (itemToAdd == null) return;

        // If Quest and NonStack Item already in inventory
        if(itemToAdd.itemQuest && !itemToAdd.itemStackable)
        {
            foreach(Item itemToCheck in items)
            {
                if (itemToCheck.itemID == id) return;
            }
        }

        if (itemToAdd.itemStackable && CheckIfItemIsInInventory(itemToAdd))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemID == id)
                {
                    ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();

                    data.amount++;
                    data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                // Slot is empty
                if (items[i].itemID == -1)
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    itemObj.GetComponent<ItemData>().item = itemToAdd;
                    itemObj.GetComponent<ItemData>().amount = 1;
                    itemObj.GetComponent<ItemData>().slot = i;
                    itemObj.transform.SetParent(slots[i].transform);
                    itemObj.transform.position = Vector2.zero;
                    itemObj.transform.localPosition = Vector2.zero;
                    itemObj.GetComponent<Image>().sprite = itemToAdd.itemSprite;
                    itemObj.name = itemToAdd.itemTitle;
                    break;
                }
            }
        }
    }

    bool CheckIfItemIsInInventory(Item item)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].itemID == item.itemID)
                return true;
        }
        return false;
    }

    // Delete by type and count
    void DelItemByTypeAndCount(Item.ItemType type, int count)
    {
        if (count > 0)
        {
            int deletedItems = 0;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemType == type || (items[i].itemQuest && type == Item.ItemType.Quest))
                {
                    if (items[i].itemStackable)
                    {
                        ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                        int minCount = Math.Min((count - deletedItems), data.amount);

                        for (int j = 0; j < minCount; j++)
                        {
                            data.amount--;
                            deletedItems++;
                        }

                        if (data.amount > 0)
                            data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                        else
                            DelItemBySlot(i);
                    }
                    else
                    {
                        DelItemBySlot(i);
                        deletedItems++;
                    }
                }
                if (deletedItems == count) break;
            }
        }
        else
        {
            return;
        }  
    }

    // Delete by slot and count
    void DelItemBySlotAndCount(int slot, int count)
    {
        // if slot and count in range 
        if (slot < 0 || slot >= slots.Count || count == 0)
        { 
            return;
        }

        if (items[slot].itemID != -1 )
        {
            ItemData data = slots[slot].transform.GetChild(0).GetComponent<ItemData>();

            for (int i = 0; i < count; i++) {
                if (data.amount == 1 || items[slot].itemType == Item.ItemType.Equipment)
                {
                    DelItemBySlot(slot);
                }
                else
                {
                    data.amount--;
                    data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                }
            }
        }
    }

    // Delete by slot
    void DelItemBySlot(int slot)
    {
        if (slot >= slots.Count || slot < 0)
        {
            return;
        }

        GameObject itemToDel;
        GameObject slotToClear;

        if (items[slot].itemID != -1)
        {
            slotToClear = slotPanel.transform.GetChild(slot).gameObject;
            itemToDel = slotToClear.transform.GetChild(0).gameObject;

            Destroy(itemToDel);
            items[slot] = new Item();
        }
        else
        {
            return;
        }
    }
    
    // Use items
    public void UseItem(int id)
    {
        if (CheckUseItem(id)){
            if (items[id].itemType == Item.ItemType.Equipment) PutOnItem(id);
            if (items[id].itemType == Item.ItemType.Consumable) DrinkItem(id);
        }
    }

    // Put the equipment
    void PutOnItem(int id)
    {
        /* Some magic */
        Debug.Log(items[id].itemTitle + " is equiped.");

        DelItemBySlot(id);
    }

    // Drink the potion
    void DrinkItem(int id)
    {

        /* Some magic */
        Debug.Log(items[id].itemTitle + " was drunk.");

        // For stackable items
        ItemData data = slots[id].transform.GetChild(0).GetComponent<ItemData>();

        if (data.amount == 1 || !items[id].itemStackable)
        {
            DelItemBySlot(id);
        }
        else
        {
            data.amount--;
            data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
        }
    }

    public void SellItem(int id)
    {
        /* Some magic */
        Debug.Log(items[id].itemTitle + " is sold.");

        DelItemBySlot(id);
    }

    public void DropItem(int id)
    {
        /* Some magic */
        Debug.Log(items[id].itemTitle + " is dropped.");

        DelItemBySlot(id);
    }

    public bool CheckUseItem(int id)
    {
        return (items[id].itemType != Item.ItemType.Trash) ? true : false;
    }

    public bool CheckSellItem(int id)
    {
        return (!items[id].itemQuest) ? true : false;
    }

    public bool CheckDropItem(int id)
    {
        return (!items[id].itemQuest) ? true : false;
    }

    // Sorting by type (Equip, Consum, Quest, Trash, Empty)
    public void SortByType()
    {

        slots.Sort(CompareObjType);
        items.Sort(new ItemComparerByType());

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<RectTransform>().SetAsLastSibling();
            slots[i].GetComponent<Slot>().id = i;
            try
            {
                slots[i].transform.GetChild(0).GetComponent<ItemData>().slot = i;
            }
            catch
            {
                Debug.Log("p");
            }
        }
    }

    // Sorting by price (Highest To Lower)
    public void SortByPrice()
    {

        slots.Sort(CompareObjPrice);
        items.Sort(new ItemComparerByPrice());

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<RectTransform>().SetAsLastSibling();
            slots[i].GetComponent<Slot>().id = i;
            try
            {
                slots[i].transform.GetChild(0).GetComponent<ItemData>().slot = i;
            }
            catch
            {
                Debug.Log("p");
            }
        }
    }

    private int CompareObjPrice(GameObject obj1, GameObject obj2)
    {
        try
        {
            Item item1 = obj1.transform.GetChild(0).GetComponent<ItemData>().item;
            try
            {
                Item item2 = obj2.transform.GetChild(0).GetComponent<ItemData>().item;

                return item1.itemPrice == item2.itemPrice ? 0 : (item1.itemPrice > item2.itemPrice ? -1 : 1);
            }
            catch (Exception a)
            {
                return -1;
            }
        }
        catch (Exception b)
        {
            try
            {
                Item item2 = obj2.transform.GetChild(0).GetComponent<ItemData>().item;
                return 1;
            }
            catch (Exception c)
            {
                return 0;
            }
        }
    }

    private int CompareObjType(GameObject obj1, GameObject obj2)
    {
        try
        {
            Item item1 = obj1.transform.GetChild(0).GetComponent<ItemData>().item;
            try
            {
                Item item2 = obj2.transform.GetChild(0).GetComponent<ItemData>().item;

                if (item1.itemType == Item.ItemType.Empty || item2.itemType == Item.ItemType.Empty)
                    return item1.itemType == item2.itemType ? 0 : (item1.itemType == Item.ItemType.Empty) ? 1 : -1; ;

                if (item1.itemQuest)
                {
                    if (item2.itemQuest)
                        return item1.itemType == item2.itemType ? 0 : (item1.itemType > item2.itemType ? -1 : 1);
                    else
                    {
                        if (item2.itemType == Item.ItemType.Trash) return -1;
                        return 1;
                    }

                }
                else if (item2.itemQuest)
                {
                    if (item1.itemType == Item.ItemType.Trash) return 1;
                    return -1;
                }
                else
                    return item1.itemType == item2.itemType ? 0 : (item1.itemType > item2.itemType ? -1 : 1);
            }
            catch (Exception a)
            {
                return -1;
            }
        }
        catch (Exception b)
        {
            try
            {
                Item item2 = obj2.transform.GetChild(0).GetComponent<ItemData>().item;
                return 1;
            }
            catch (Exception c)
            {
                return 0;
            }
        }
    }

}

