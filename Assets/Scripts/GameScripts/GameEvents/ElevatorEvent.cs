using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorEvent : GameEvent
{
    public List<Sprite> shopItems;
    private List<Image> SelectButtons;
    private Button ConfirmButton;
    private int SelectedButton;
    private bool ConfirmedSelection;

    public override void EndTurn() {
        Image selectedItem = SelectButtons[SelectedButton];
        Debug.Log(selectedItem.sprite.name);
        switch(SelectButtons[SelectedButton].sprite.name) {
            case "Power3":
                GameController.Instance.EnergyCount += 1;
                break;
            case "Grain":
                GameController.Instance.FoodCount += 1;
                break;
        }
        IsDestroyed = true;
    }

    public override void StartTurn() {
        ConfirmedSelection = false;
        List<int> selectedIndexes = new List<int>();

        while (selectedIndexes.Count < SelectButtons.Count) {
            int chosenIndex = Random.Range(0, shopItems.Count);
            if(!selectedIndexes.Contains(chosenIndex)) {
                selectedIndexes.Add(chosenIndex);
            }
        }

        for (int i = 0; i < selectedIndexes.Count; i++) {
            SelectButtons[i].sprite = shopItems[selectedIndexes[i]];
        }

        foreach (Image button in SelectButtons) {
            button.gameObject.GetComponent<Button>().enabled = true;
            button.color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    public void SelectShopItem(int ClickedButton) {
        if(!ConfirmedSelection) {
            SelectedButton = ClickedButton;
            SelectButtons[ClickedButton].color = new Color(0.0f, 0.0f, 0.0f);

            ConfirmButton.enabled = true;
            ConfirmButton.gameObject.GetComponent<Image>().color = new Color(0.0f, 0.8f, 0.0f);
            GameObject.Find("Text").GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    public void Confirm() {
        ConfirmButton.enabled = false;
        ConfirmButton.gameObject.GetComponent<Image>().color = new Color(0.4f, 0.5f, 0.4f);
        GameObject.Find("Text").GetComponent<Text>().color = new Color(0.7f, 0.7f, 0.7f);
        ConfirmedSelection = true;
        foreach(Image button in SelectButtons) {
            button.gameObject.GetComponent<Button>().enabled = false;
            button.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        }

        SelectButtons[SelectedButton].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    void Awake() {
        SelectButtons = new List<Image>();
        SelectButtons.Add(GameObject.Find("SelectButton1").GetComponent<Image>());
        SelectButtons.Add(GameObject.Find("SelectButton2").GetComponent<Image>());
        SelectButtons.Add(GameObject.Find("SelectButton3").GetComponent<Image>());
        ConfirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
        ConfirmButton.enabled = false;
        ConfirmButton.gameObject.GetComponent<Image>().color = new Color(0.4f, 0.5f, 0.4f);
        GameObject.Find("Text").GetComponent<Text>().color = new Color(0.7f, 0.7f, 0.7f);
        StartTurn();

        SelectButtons[0].gameObject.GetComponent<Button>().onClick.AddListener(() => SelectShopItem(0));
        SelectButtons[1].gameObject.GetComponent<Button>().onClick.AddListener(() => SelectShopItem(1));
        SelectButtons[2].gameObject.GetComponent<Button>().onClick.AddListener(() => SelectShopItem(2));
        ConfirmButton.onClick.AddListener(Confirm);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
