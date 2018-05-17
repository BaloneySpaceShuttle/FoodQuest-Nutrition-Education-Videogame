using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public int playerFoodPoints = 1000;
    public int playerProteinPoints = 100;
    public int playerCarbsPoints = 300;
    public int playerFatsPoints = 100;
    public static GameManager instance = null;

	void Awake() {
        if (instance == null) {
            instance = this;
        }
        
        else if (instance != this) {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }
}
