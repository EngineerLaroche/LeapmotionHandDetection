using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   CLAP GESTURE
 *
 * INFO:    Représente le geste de taper les mains ensemble.
 *          La paume de la main gauche doit toucher celle
 *          de la main droite. Doit etre fait de façon rapide.
 * 
 *****************************************************/
public class ClapGesture : GestureMediator
{
    //Point de contact des mains
    public float handsDistance = 0.08f; 
    public float cooldownTime = 0.2f;

    //Vitesse de contact des deux mains
    [Range(0.1f, 6.0f)]
    public float clapSpeed = 1.5f;
    private float cooldownLeft = 0.0f;

    /*****************************************************
    * UPDATE
    *
    * INFO:    Maintient a jour le cooldown pour chaque
    *          geste Clap effectué. Aide a valider si c'est
    *          vraiment un geste Clap.
    *           
    *****************************************************/
    void Update()
    {
        if (cooldownLeft > 0.0f)
        {
            cooldownLeft -= Time.deltaTime;
            if (cooldownLeft < 0.0f) { cooldownLeft = 0.0f; }
        }
    }

    /*****************************************************
    * DETECTED FIST GESTURE
    *
    * INFO:    Valide la détection du geste qui consiste 
    *          à tapper des mains. La fonction 
    *          communique directement avec le médiateur
    *          du contrôleur de gestes.
    *           
    *****************************************************/
    public override bool IsDetectedGesture()
    {
        //Si les deux mains sont visibles
        if (DetectionController.GetInstance().IsBothHandsVisible() && cooldownLeft <= 0.0f)
        {
            //La main gauche et droite
            DetectionController.HandController leftHand = DetectionController.GetInstance().GetHand(HandsE.gauche);
            DetectionController.HandController rightHand = DetectionController.GetInstance().GetHand(HandsE.droite);
            
            if (leftHand.GetHandVelocity().magnitude >= clapSpeed &&
                rightHand.GetHandVelocity().magnitude >= clapSpeed)
            {
                //Si les deux mains sont assez proche l'une de l'autre
                if (DetectionController.GetInstance().GetDistanceBetweenHands() <= handsDistance)
                {
                    cooldownLeft = cooldownTime;
                    return true;
                }
            }
        }
        return false;
    }

    /*****************************************************
    * GESTURE TYPE
    *
    * INFO:    Retourne (écrase) le type de geste détecté. 
    *          La fonction communique directement avec le 
    *          médiateur du contrôleur de gestes.
    *          
    *****************************************************/
    public override string DetectedGestureName()
    {
        return "Clap Hands";
    }
}
