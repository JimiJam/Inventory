using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour {
    private Item item;
    private string data;
    private GameObject tooltip;

    void Start()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltip.SetActive(false);
    }

    void Update()
    {
        if (tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }

	public void Activate(Item item)
    {
        this.item = item;
        ConstructDataString();

        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    // Text for tooltip
    public void ConstructDataString()
    {
        data = "<color=#000000><b>" + item.itemTitle
            + "</b></color>\n\n" + item.itemType.ToString();
        if (!item.itemQuest) data += "\nPrice: " + item.itemPrice.ToString();
        data += "\n\n" + item.itemDescription + "";

        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }
}
