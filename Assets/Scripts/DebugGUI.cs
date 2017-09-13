using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour {
    private GUIStyle guiStyle = new GUIStyle();
    // Use this for initialization
    void Start() { }

        
    
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnGUI()
    {
        foreach (Touch touch in Input.touches)
        {
            string message = "";
            message += "ID: " + touch.fingerId + "\n";
            message += "Phase: " + touch.phase.ToString() + "\n";
            message += "TapCount: " + touch.tapCount + "\n";
            message += "Pos X: " + touch.position.x + "\n";
            message += "Pos Y: " + touch.position.y + "\n";
            int num = touch.fingerId;
            guiStyle.fontSize = 50;
            GUI.Label(new Rect(0+130*num,0,120,100),message,guiStyle);

        }
    }
}
