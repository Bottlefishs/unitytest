using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour {
    // Use this for initialization
    void Awake () {
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
