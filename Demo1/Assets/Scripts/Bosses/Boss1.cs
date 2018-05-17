using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random=System.Random;

public class Boss1 : MonoBehaviour {
    
    private int dice = 0;                           // Dice used for direction change 
    private float speed = 2f;                       // Boss speed
    private double healthPoints = 1000;             // Boss health points
    private bool isMoving = false;                  // is enemy in motion
    private bool isAttacking = false;               // is enemy currently attacking
    private bool isPredator = false;                // is enemy moving towards player 
    private bool isStunned = false;                 // Used to pause Villain motion on hit
    private bool isAttackInCooldown = false;        // Used to delay attack
    private bool isVunerable = false;               // Used to make succeptible to player attacks
    
	private Animator animator;                      // Boss Animator
    private Text infoText;                          // HUD InfoText
	private Rigidbody2D rb;                         // Boss Rigidbody2D    
    private GameObject playerGO;                    // Player GameObject
    private Random rnd;                             // Random value used for direction change diceroll
    private Vector2 direction = Vector2.right;      // Current directional vector    
    private Vector2 lastDirection = Vector2.zero;   // Last directional vector   
    
    void Awake() {
        isMoving = false;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
	void Start () {
        rnd = new Random();
        playerGO = GameObject.Find("Player(Clone)");  //needs to be changed eventually
        infoText = GameObject.Find ("HUD(Clone)/InfoText").GetComponent<UnityEngine.UI.Text>();
        
        InvokeRepeating("CheckDistanceToPlayer", 0.0f, 0.5f);
	}
    
	void Update () {
		RefreshUI();
	}
    
    void FixedUpdate() {
        
        if(isStunned || isAttacking) {
            direction = Vector2.zero;
            DisableRagdoll();      //disable ragdoll
        }
        
        if(direction.x != 0 && direction.y != 0) {
            lastDirection = direction;
        }
        rb.velocity = direction * speed;
        
        if(!isMoving && !isStunned && !isAttacking) {
            InvokeRepeating("SwapDirection", 0.0f, 1f);
        }
        
        if(!isStunned && !isAttacking) {
            EnableRagdoll();     //enable ragdoll
        }
        
        if(healthPoints <= 0) {
            // healthPoints = 0;
            infoText.text = "Boss DEFEATED";
            playerGO.GetComponent<Player>().Boss1Defeated();
            gameObject.SetActive(false);
        }
        
        
        
    }
    
    void NewDirection() {
        // Random rnd = new Random();
        dice = rnd.Next(1, 10); //left1 up2 right3 down4 pause>4
    }
    
    void SwapDirection() {
        
        isMoving = true;
        
        // sdCount++;
        
        NewDirection();
        
        switch(dice) {
            
            case 1: {
                direction = Vector2.left;
                break;
            }
            
            case 2: {
                direction = Vector2.up;
                break;
            }
            
            case 3: {
                direction = Vector2.right;
                break;
            }
            
            case 4: {
                direction = Vector2.down;
                break;
            }
            
            case 5 : {
                direction = new Vector2(1f, 1f);
                break;
            }
            
            case 6 : {
                direction = new Vector2(-1f, -1f);
                break;
            }
            
            case 7 : {
                direction = new Vector2(-1f, 1f);
                break;
            }
            
            case 8 : {
                direction = new Vector2(1f, 1f);
                break;
            }
            
            default : {
                direction = Vector2.zero;
                break;
            }
        }
        
        // rb.velocity = direction * speed;
        
        animator.Play ("Boss1Idle");   //has no directional animation
        
    }
    
    void OnCollisionEnter2D(Collision2D col) {

        if(col.gameObject.tag == "Player" && isAttacking && !isAttackInCooldown) {
            InflictDamage();
        }
    }
    
    void OnTriggerEnter2D(Collider2D col) {
        
        // enemy under attack
        if(col.tag == "Weapon" && col.GetComponent<Renderer>().enabled == true && isVunerable) {
            healthPoints -= 10;
        }
    }
    
    void OnTriggerStay2D(Collider2D col) {
        
        // enemy under attack
        if(col.tag == "Weapon" && col.GetComponent<Renderer>().enabled == true && isVunerable) {
            healthPoints -= 10;
        }
    }
    
    // float DistanceToPlayer() {
        // float x1 = gameObject.transform.position.x;
        // float y1 = gameObject.transform.position.y;
        // float x2 = playerGO.transform.position.x;
        // float y2 = playerGO.transform.position.y;
        // float dist = DistanceBetweenTwoPoints(x1,x2,y1,y2);
        // return dist;
    // }
    
    void CheckDistanceToPlayer() {
        
        float x1 = gameObject.transform.position.x;
        float y1 = gameObject.transform.position.y;
        float x2 = playerGO.transform.position.x;
        float y2 = playerGO.transform.position.y;
        float dist = DistanceBetweenTwoPoints(x1,x2,y1,y2);
        
        // if(dist <= 3f && !IsAttackModeInCooldown()) {
            // ..is now close to player...
            // ..start attack..
            // ..check if predator mode is in cooldown..
            
            // isPredator = false;
            // DisableRagdoll();
            // CancelInvoke("SwapDirection");
            // StartCoroutine(AttackPlayer());
            
        // }
        
        if(dist <= 4f && !isAttackInCooldown && !isStunned && !isAttacking) {       //Attack player
            StartCoroutine(AttackPlayer());
        }
        
        else if(dist <= 8f) {
            // ..start moving towards player...
            // ..needs animation fixes..
            CancelInvoke("SwapDirection");
            isPredator = true;
            direction = new Vector2((x2-x1) * 0.3f, (y2-y1) * 0.3f); //vector maths equation x0.3f otherwise too fast
        }
        
        else if(dist > 9f && isPredator) {
            isPredator = false;
            InvokeRepeating("SwapDirection", 0.0f, 1f);
        }
    }
    
    // bool IsAttackModeInCooldown() {
        // return isAttackInCooldown;
    // }
    
    void SetAttackModeToCooldown() {
        StartCoroutine(AttackModeModeToCooldown());
    }
    
    IEnumerator AttackModeModeToCooldown() {
        
        isAttackInCooldown = true;
        // isPredator = false;
        StartCoroutine(StunEnemy());
        // ..retreat from player..
        
        // yield return new WaitUntil(() => isStunned == false);
        yield return new WaitForSeconds (10f);
        isAttackInCooldown = false;
        // isPredator = true;

    }
    
    void InflictDamage() {
        
        if(isAttacking && !isAttackInCooldown && !playerGO.GetComponent<Player>().isInKnockback()) {
            playerGO.GetComponent<Player>().TakeDamage(gameObject);      //Causes player knockback
        }
    }
    
    IEnumerator AttackPlayer() {
        direction = Vector2.zero;
        // isMoving = false;
        DisableMoving();
        // isAttacking = true;
        EnableAttacking();
        // SetAttackModeToCooldown();
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;       //Get Animator controller
        for(int i = 0; i<ac.animationClips.Length; i++) {                        //For all animations
            if(ac.animationClips[i].name == "Boss1Attack") {                     //If it has the same name as your clip
                time = ac.animationClips[i].length;
            }
        }
        animator.Play ("Boss1Attack");
        yield return new WaitForSeconds(time);                              //wait for animation to finish then switch off attack mode stuff
        
        // isAttacking = false;
        DisableAttacking();
        EnableRagdoll();
        SetAttackModeToCooldown();                                              // Set Attack Cooldown to dealy next attack              
    }
    
    IEnumerator StunEnemy() { //stun enemy for n time
        
        direction = Vector2.zero;
        
        isMoving = false;
        isPredator = false;
        isVunerable = true;
        isStunned = true;
        
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;       //Get Animator controller
        for(int i = 0; i<ac.animationClips.Length; i++) {                        //For all animations
            if(ac.animationClips[i].name == "Boss1Stunned") {                     //If it has the same name as your clip
                time = ac.animationClips[i].length;
            }
        }
        
        animator.Play ("Boss1Stunned");
        
        
        // yield return new WaitForSeconds (5f);
        yield return new WaitForSeconds (time * 5f);
        isVunerable = false;
        isStunned = false;
        isMoving = true;
        isPredator = true;
        animator.Play ("Boss1Idle");
    }
    
    void EnableRagdoll() {
        rb.isKinematic = false;
        // rb.detectCollisions = true;
    }
    
    void DisableRagdoll() {
        rb.isKinematic = true;
        // rb.detectCollisions = false;
    }
    
    void EnableVunerable() {
        isVunerable = true;
    }
    
    void DisableVunerable() {
        isVunerable = false;
    }
    
    void EnableMoving() {
        isMoving = true;
    }
    
    void DisableMoving() {
        isMoving = false;
    }
    
    void EnableAttacking() {
        isAttacking = true;
    }
    
    void DisableAttacking() {
        isAttacking = false;
    }
    
    void EnableStunned() {
        isStunned = true;
    }
    
    void DisableStunned() {
        isStunned = false;
    }
    
    void EnablePredator() {
        isPredator = true;
    }
    
    void DisablePredator() {
        isPredator = false;
    }
    
    float DistanceBetweenTwoPoints(float x1, float x2, float y1, float y2) {
        return (float)Math.Sqrt((Math.Pow(x1-x2,2) + Math.Pow(y1-y2,2)));
    }
    
    void RefreshUI() {
        infoText.text = "Boss Health: " + healthPoints;
    }
}
