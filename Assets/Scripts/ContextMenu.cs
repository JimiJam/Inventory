using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContextMenu : MonoBehaviour {

    private int itemSlot;
    private GameObject contextmenu;
    private Inventory inv;
    private GameObject UseButton;
    private GameObject SellButton;
    private GameObject DropButton;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        UseButton = GameObject.Find("Use");
        UseButton.SetActive(false);
        SellButton = GameObject.Find("Sell");
        SellButton.SetActive(false);
        DropButton = GameObject.Find("Drop");
        DropButton.SetActive(false);

        contextmenu = GameObject.Find("ContextMenu");
        contextmenu.SetActive(false);
    }

    // Available actions for item
    public void GetContext(int id)
    {
        this.itemSlot = id;
        contextmenu.transform.GetChild(0).GetComponent<Text>().text = "";
        contextmenu.transform.position = Input.mousePosition;
        contextmenu.SetActive(true);
        int count = 0;

        if (inv.CheckUseItem(id) && !inv.items[itemSlot].itemQuest)
        {
            UseButton.transform.position = new Vector2(contextmenu.transform.position.x + 95, contextmenu.transform.position.y - 50);
            UseButton.SetActive(true);
        }
        else count++;

        if (inv.CheckSellItem(id))
        {
            SellButton.transform.position = new Vector2(contextmenu.transform.position.x + 95, contextmenu.transform.position.y - 70);
            SellButton.transform.GetChild(0).GetComponent<Text>().text = "Sell at a price: " + inv.items[id].itemPrice.ToString();
            SellButton.SetActive(true);
        }
        else count++;

        if (inv.CheckDropItem(id))
        {
            DropButton.transform.position = new Vector2(contextmenu.transform.position.x + 95, contextmenu.transform.position.y - 90);
            DropButton.SetActive(true);
        }
        else count++;

        if (count == 3)
        {
            contextmenu.transform.GetChild(0).GetComponent<Text>().text = "No available actions";
        }
    }

    public void Deactivate()
    {
        contextmenu.SetActive(false);
        UseButton.SetActive(false);
        SellButton.SetActive(false);
        DropButton.SetActive(false);
    }
    public void OnClickUse() {
        inv.UseItem(itemSlot);

        Deactivate();
    }

    public void OnClickSell()
    {
        inv.SellItem(itemSlot);

        Deactivate();
    }

    public void OnClickDrop()
    {
        inv.DropItem(itemSlot);

        Deactivate();
    }

}
