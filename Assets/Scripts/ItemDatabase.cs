using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

// Contains all item in game
public class ItemDatabase : MonoBehaviour {

    private List<Item> database = new List<Item>();
    private JsonData itemData;

    // Use this for initialization
    void Start () {
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        ConstructItemDatabase();
    }
	
    public Item FetchItemByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
            if (database[i].itemID == id)
                return database[i];
        return null;
    }

    void ConstructItemDatabase()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            database.Add(new Item(
                (int)itemData[i]["id"], 
                itemData[i]["title"].ToString(), 
                itemData[i]["description"].ToString(),
                (int)itemData[i]["price"],
                itemData[i]["type"].ToString(),
                (bool)itemData[i]["stackable"],
                (bool)itemData[i]["quest"],
                itemData[i]["slug"].ToString()
                ));
        }
    }

}

public class Item
{
    public int itemID { get; set; }
    public string itemTitle { get; set; }
    public string itemDescription { get; set; }
    public int itemPrice { get; set; }
    public bool itemStackable { get; set; }
    public bool itemQuest { get; set; }
    public string itemSlug { get; set; }
    public Sprite itemSprite { get; set; }

    public ItemType itemType { get; set; }
    public enum ItemType
    {
        Empty,
        Trash,
        Quest,
        Consumable,
        Equipment
    };
    
    public Item(int id, string title, string descr, int price, string type, bool stackable, bool quest, string slug)
    {
        this.itemID = id;
        this.itemTitle = title;
        this.itemDescription = descr;
        this.itemPrice = price;
        this.itemStackable = stackable;
        this.itemQuest = quest;
        this.itemSlug = slug;
        this.itemSprite = Resources.Load<Sprite>("Sprites/Items/" + slug);
        switch (type)
        {
            case "Equipment":
                this.itemType = ItemType.Equipment;
                break;
            case "Consumable":
                this.itemType = ItemType.Consumable;
                break;
            case "Trash":
                this.itemType = ItemType.Trash;
                break;
            default:
                break;
        }
    }

    public Item()
    {
        this.itemID = -1;
        this.itemPrice = -1;
        this.itemType = ItemType.Empty;
    }

}

public class ItemComparerByPrice : IComparer<Item>
{
    public int Compare(Item item1, Item item2)
    {
        return item1.itemPrice == item2.itemPrice ? 0 : (item1.itemPrice > item2.itemPrice ? -1 : 1);
    }
}

public class ItemComparerByType : IComparer<Item>
{
    public int Compare(Item item1, Item item2)
    {
        // For Empty slots
        if (item1.itemType == Item.ItemType.Empty || item2.itemType == Item.ItemType.Empty)
            return item1.itemType == item2.itemType ? 0 : (item1.itemType == Item.ItemType.Empty) ? 1 : -1; ;

        if (item1.itemQuest) {
            if (item2.itemQuest)
                return  item1.itemType == item2.itemType ? 0 : (item1.itemType > item2.itemType ? -1 : 1);
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
}
