using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

	public static HUD instance = null;
    
    void Awake() {
        if (instance == null) {
            instance = this;
        }
        
        else if (instance != this) {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    void Start() {
        // GameObject.Find ("HUD(Clone)/TouchControls/FishButton").GetComponent<UnityEngine.UI.Button>().enabled = false;
    }
}
