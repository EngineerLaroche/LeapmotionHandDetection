using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   DISPLAY ANIMATION
 *
 * INFO:    Permet d'activer l'animation d'un Objet qui
 *          consiste à s'ouvrir ou se fermer.
 * 
 *****************************************************/
public class DisplayAnimation : MonoBehaviour
{
    // GameObject to display or hide
    public GameObject objectToAnimate;

    // Child animation component of GameObject
    private Animator animator;

    /*****************************************************
    * AUTOMATE OBJECT DISPLAY
    *
    * INFO:     Recupere l'etat de l'animation (bool).
    *           Si l'objet est caché, on l'affiche.
    *           Si l'objet est affiché, on le cache.
    *           
    *****************************************************/
    public void AutomateObjectDisplay()
    {
        if (IsAnimationValid())
        {
            bool isOpen = animator.GetBool("open");
            animator.SetBool("open", !isOpen);
        }
    }

    /*****************************************************
    * DISPLAY OBJECT
    *
    * INFO:    Recupere l'etat de l'animation (bool).
    *          Force l'affichage de l'objet.
    *          
    *          *** Non utilisé ***
    *           
    *****************************************************/
    public void DisplayObject()
    {
        if (IsAnimationValid()) { animator.SetBool("open", true); }
    }

    /*****************************************************
    * HIDE OBJECT
    *
    * INFO:    Recupere l'etat de l'animation (bool).
    *          Cache de force l'objet.
    *          
    *          *** Non utilisé ***
    *           
    *****************************************************/
    public void HideObject()
    {
        if (IsAnimationValid()) { animator.SetBool("open", false); }
    }

    /*****************************************************
    * IS ANIMATION VALID
    *
    * INFO:    Vérifie si l'objet et l'animation 
    *          (child component) ne sont pas null.
    *          Retourne vrai si respecté.
    *           
    *****************************************************/
    private bool IsAnimationValid()
    {
        if (objectToAnimate != null)
        {
            animator = objectToAnimate.GetComponent<Animator>();
            if (animator != null) { return true; }
            return false;
        }
        return false;
    }
}
