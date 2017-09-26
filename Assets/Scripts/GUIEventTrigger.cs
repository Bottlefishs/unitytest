using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUIEventTrigger : EventTrigger
{
    private BoardManager boardScript;
    private GameObject menu;
    private GameObject gameManager;
    // Use this for initialization
    public override void OnPointerUp(PointerEventData data)
    {
        boardScript = GameObject.Find("GameManager").GetComponent<BoardManager>();
        boardScript.mineNumber = Int32.Parse(GameObject.Find("mineInputField").GetComponent<InputField>().text);
        boardScript.rows = Int32.Parse(GameObject.Find("rowInputField").GetComponent<InputField>().text);
        boardScript.columns = Int32.Parse(GameObject.Find("columnInputField").GetComponent<InputField>().text);
        menu = GameObject.Find("MainMenu");
        menu.SetActive(false);
        gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<GameManager>().InitGame();
    }

    
}
