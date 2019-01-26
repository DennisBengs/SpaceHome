using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorEvent : GameEvent
{
    public Sprite Food;
    public Sprite Power;
    public Sprite Human;
    public Sprite Module;
    private List<Image> SelectButtons;
    private Button ConfirmButton;
    private int SelectedButton;
    private bool ConfirmedSelection;

    public override void EndTurn() {
    }

    public override void StartTurn() {
        ConfirmedSelection = false;
        SelectButtons[0].sprite = Food;
        SelectButtons[1].sprite = Power;
        SelectButtons[2].sprite = Human;
    }

    public void SelectShopItem(int ClickedButton) {
        if(!ConfirmedSelection) {

            foreach (Image button in SelectButtons) {
                button.gameObject.GetComponent<Button>().enabled = true;
                button.color = new Color(1.0f, 1.0f, 1.0f);
            }

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

    // Start is called before the first frame update
    void Start()
    {
        
        SelectButtons = new List<Image>();
        SelectButtons.Add(GameObject.Find("SelectButton1").GetComponent<Image>());
        SelectButtons.Add(GameObject.Find("SelectButton2").GetComponent<Image>());
        SelectButtons.Add(GameObject.Find("SelectButton3").GetComponent<Image>());
        ConfirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
        ConfirmButton.enabled = false;
        ConfirmButton.gameObject.GetComponent<Image>().color = new Color(0.4f, 0.5f, 0.4f);
        GameObject.Find("Text").GetComponent<Text>().color = new Color(0.7f, 0.7f, 0.7f);
        StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
