using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random=System.Random;

public class Enemy3 : MonoBehaviour {
    
    private float speed = 0.5f;
    private double healthPoints = 200;
    private int dice = 0;
    private Vector2 direction = Vector2.right;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isPredator = false;                 // Used to start or stop enemy moving towards player
    private bool isStunned = false;                   // Used to pause Villain motion on hit
    private bool isAttackInCooldown = false;         // Used to delay attack
    private bool isVunerable = true;                // Used to make succeptible to player attacks
    private Animator animator;
	private Rigidbody2D rb;
    private GameObject playerGO;
    private Random rnd;
    
    
	void Start () {
        rnd = new Random();
        playerGO = GameObject.Find("Player(Clone)");  //needs to be changed eventually
        InvokeRepeating("CheckDistanceToPlayer", 0.0f, 0.6f);
	}
    
    void Awake() {
        isMoving = false;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
	
	void Update () {
		// RefreshUI();
	}
    
    void FixedUpdate() {
        
        // rnd = new Random();
        
        if(isStunned || isAttacking) {
            direction = Vector2.zero;
            DisableRagdoll();      //disable ragdoll
        }
        
        rb.velocity = direction * speed;
        
        if(!isMoving && !isStunned && !isAttacking) {
            InvokeRepeating("SwapDirection", 0.0f, 2f);
        }
        
        if(!isStunned && !isAttacking) {
            EnableRagdoll();     //enable ragdoll
        }
        
        if(healthPoints <= 0) {
            playerGO.GetComponent<Player>().EnemyDefeat();
            gameObject.SetActive(false);
        }
        
    }
    
    void NewDirection() {
        // Random rnd = new Random();
        dice = rnd.Next(1, 4); //left1 up2 right3 down4 pause>4
    }
    
    void SwapDirection() {
        isMoving = true;
        
        NewDirection();
        
        switch(dice) {
            
            case 1: {
                direction = Vector2.left;
                animator.Play ("Enemy3WalkLeft");
                break;
            }
            
            case 2: {
                direction = Vector2.up;
                animator.Play ("Enemy3WalkUpward");
                break;
            }
            
            case 3: {
                direction = Vector2.right;
                animator.Play ("Enemy3WalkRight");
                break;
            }
            
            case 4: {
                direction = Vector2.down;
                animator.Play ("Enemy3WalkDownward");
                break;
            }
            
            default : {
                direction = Vector2.zero;
                animator.Play ("Enemy3Idle");
                break;
            }
        }
    }
    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Player" && isAttacking && !isAttackInCooldown) {
            InflictDamage();
        }
    }
    
    void OnTriggerEnter2D(Collider2D col) {
        
        // enemy under attack
        if(col.tag == "Weapon" && col.GetComponent<Renderer>().enabled == true && isVunerable == true) {
            DisableRagdoll();
            CancelInvoke("SwapDirection");
            StartCoroutine(StunEnemy());
        }
        
        // if(col.tag == "Player" && isAttacking) {
            // InflictDamage();
        // }
    }
    
    void OnTriggerStay2D(Collider2D col) {
        
        // enemy under attack
        if(col.tag == "Weapon" && col.GetComponent<Renderer>().enabled == true && isVunerable == true) {
            DisableRagdoll();
            CancelInvoke("SwapDirection");
            StartCoroutine(StunEnemy());
        }
    }
    
    void CheckDistanceToPlayer() {
        float x1 = gameObject.transform.position.x;
        float y1 = gameObject.transform.position.y;
        float x2 = playerGO.transform.position.x;
        float y2 = playerGO.transform.position.y;
        float dist = DistanceBetweenTwoPoints(x1,x2,y1,y2);
        
        if(dist <= 4f && !IsAttackModeInCooldown() && !isStunned) {       //Attack player
            StartCoroutine(AttackPlayer());
        }
        
        else if(dist <= 5) {                                 // ..start moving towards player...
        // if(dist <= 5) {                                 // ..start moving towards player...
            CancelInvoke("SwapDirection");
            EnablePredator();
            direction = new Vector2((x2-x1) * 0.3f, (y2-y1) * 0.3f); //calculate vector, a maths equation
        }
        
        else if(dist > 8 && isPredator) {
            DisablePredator();
            InvokeRepeating("SwapDirection", 0.0f, 2f);
        }
    }
    
    IEnumerator AttackPlayer() {

        CancelInvoke("SwapDirection");
        direction = Vector2.zero;
        
        DisableRagdoll();
        DisableVunerable();
        DisableMoving();
        EnableAttacking();
        
        animator.Play ("Enemy3Attack");
        
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;  //Get Animator controller
        for(int i = 0; i<ac.animationClips.Length; i++) {                   //For all animations
            if(ac.animationClips[i].name == "Enemy3Attack") {               //If it has the same name as attack animation clip
                time = ac.animationClips[i].length;
            }
        }

        yield return new WaitForSeconds(time * 2f);                         //wait for animation to finish then switch off attack mode stuff
        SetAttackModeToCooldown();
        DisableAttacking();
        EnableRagdoll();
        EnableVunerable();
        EnableMoving();
        EnablePredator();
    }
    
    IEnumerator StunEnemy() { //stun enemy for n time
        
        
        
        direction = Vector2.zero;
        // isMoving = false;
        DisableMoving();
        // isStunned = true;
        EnableStunned();
        animator.Play ("Enemy3Stunned");
        
        
        yield return new WaitForSeconds(2f);
        healthPoints -= 10;
        isStunned = false;
    }
    
    void InflictDamage() {
        if(isAttacking && !playerGO.GetComponent<Player>().isInKnockback()) {
            playerGO.GetComponent<Player>().TakeDamage(gameObject);      //Causes player knockback
        }
    }
    
    bool IsAttackModeInCooldown() {
        return isAttackInCooldown;
    }
    
    void SetAttackModeToCooldown() {
        StartCoroutine(AttackModeModeToCooldown());
    }
    
    IEnumerator AttackModeModeToCooldown() {
        
        isAttackInCooldown = true;
        isPredator = false;
        // StartCoroutine(StunEnemy());
        // ..retreat from player..
        
        yield return new WaitForSeconds (8f);
        isAttackInCooldown = false;
        // isPredator = true;
        
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
    
    float DistanceBetweenTwoPoints() {
        float x1 = gameObject.transform.position.x;
        float y1 = gameObject.transform.position.y;
        float x2 = playerGO.transform.position.x;
        float y2 = playerGO.transform.position.y;
        return (float)Math.Sqrt((Math.Pow(x1-x2,2) + Math.Pow(y1-y2,2)));
    }
    
    void RefreshUI() {
        // infoText.text = "Enemy Health: " + healthPoints;
    }
}
