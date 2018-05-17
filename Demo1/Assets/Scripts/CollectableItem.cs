using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour {
    
    public int protein;
    public int carbs;
    public int fats;
    public int fibre;
    public string itemName;
    public bool isConsumed;
    
    
    void Awake() {
        
    }
    
    void OnTriggerEnter2D(Collider2D col) {
        
	}
	void Start () {
	}
	void Update () {
		
	}
    
    // int getProtein() {
        // return protein;
    // }
    
    // int getCarbs() {
        // return carbs;
    // }
    
    // int getFats() {
        // return fats;
    // }
    
    // int getFibre() {
        // return fibre;
    // }
    
    // string getName() {
        // return itemName;
    // }
    
    
}
