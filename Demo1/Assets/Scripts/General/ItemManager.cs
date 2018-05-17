using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour {
    
    
    
    // private List<GameObject> sceneInventory; 
    
    void Awake() {
        
    }

	void Start () {
		// sceneInventory = new List<GameObject>();
	}
	
	void Update () {
		
	}
    
    // called first
    void OnEnable()  {
        // Debug.Log("OnEnable called");
        // SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Debug.Log("OnSceneLoaded: " + scene.name);
        
        // GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        // foreach (GameObject item in items) {
            // sceneInventory.add(item);
        // }
    }
}
