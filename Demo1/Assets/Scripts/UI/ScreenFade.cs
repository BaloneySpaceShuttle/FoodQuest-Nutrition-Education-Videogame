using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ScreenFade : MonoBehaviour {
    
    private Animator animator;
    
    void Awake() {
        animator = GetComponent<Animator>();
    }
    
    public void FadeOut() {
        animator.Play("FadeOut");
    }
    
    public void FadeInEnded() {
        animator.SetBool("Fade", false);
    }
    
    public void FadeOutEnded() {
        animator.SetBool("Fade", true);
    }
}
