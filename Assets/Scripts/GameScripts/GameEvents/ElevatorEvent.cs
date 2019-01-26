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
    private int SelectedButton;
    private bool ConfirmedForTurn;

    public override void EndTurn() {
    }

    public override void StartTurn() {
        ConfirmedForTurn = false;
        SelectButtons[0].sprite = Food;
        SelectButtons[1].sprite = Power;
        SelectButtons[2].sprite = Human;
    }

    public void SelectShopItem(int ClickedButton) {
        if(!ConfirmedForTurn) {
            foreach (Image button in SelectButtons) {
                button.gameObject.GetComponent<Button>().enabled = true;
                button.color = new Color(1.0f, 1.0f, 1.0f);
            }

            SelectedButton = ClickedButton;
            SelectButtons[ClickedButton].color = new Color(0.0f, 0.0f, 0.0f);
        }
    }

    public void Confirm() {
        ConfirmedForTurn = true;
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
        StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
