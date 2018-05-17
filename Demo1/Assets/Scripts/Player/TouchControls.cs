using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random=System.Random;
using UnityEngine.EventSystems;

public class TouchControls : MonoBehaviour {
    
    private GameObject playerGO;

	void Start () {
		playerGO = GameObject.Find("Player(Clone)");
	}
    
    public void UpwardArrow() {
        Debug.Log("Up Arrow");
        playerGO.GetComponent<Player>().MoveUp();
    }
    
    public void DownwardArrow() {
        Debug.Log("down Arrow");
        playerGO.GetComponent<Player>().MoveDown();
    }
    
    public void LeftArrow() {
        Debug.Log("left Arrow");
        playerGO.GetComponent<Player>().MoveLeft();
    }
    
    public void RightArrow() {
        Debug.Log("righy Arrow");
        playerGO.GetComponent<Player>().MoveRight();
    }
    
    public void UnpressedUpwardArrow() {
        playerGO.GetComponent<Player>().IdleUpward();
    }
    
    public void UnpressedDownwardArrow() {
        playerGO.GetComponent<Player>().IdleDownward();
    }
    
    public void UnpressedLeftArrow() {
        playerGO.GetComponent<Player>().IdleLeft();
    }
    
    public void UnpressedRightArrow() {
        playerGO.GetComponent<Player>().IdleRight();
    }    
    
    public void AttackButton() {
        playerGO.GetComponent<Player>().LaunchAttack();
    }
    
    public void FishButton() {
        playerGO.GetComponent<Player>().Fish();
    }
    
    public void FishButtonOff() {
        playerGO.GetComponent<Player>().FishOff();
    }
}
