using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   ONE FINGER GESTURE
 *
 * INFO:    Geste qui consiste a avoir seulement un doigt
 *          d'ouvert pour une main donnée.
 * 
 *****************************************************/
public class OneFingerGesture : GestureMediator
{
    // La main qui effectuer le geste
    public HandsE hand = HandsE.droite;
    // Le doigt de la main qui represente le geste
    public FingersE finger = FingersE.index;

    /*****************************************************
    * DETECTED ONLY INDEX GESTURE
    *
    * INFO:    Retourne vrai si seulement ce doigt est ouvert.
    *           
    *****************************************************/
    public override bool IsDetectedGesture()
    {      
        //Si la bonne main est détectée
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            bool onlyThisFingerOpened = false;
            DetectionController.HandController handController = DetectionController.GetInstance().GetHand(hand);

            //Pour tous les doigts de la main
            foreach (DetectionController.FingerController fingerController in handController.GetFingers())
            {
                //Verifie si l'index est ouvert et que les autres doigts sont fermés
                if (fingerController == handController.GetFinger(finger)) { onlyThisFingerOpened = fingerController.IsFingerOpen(); }
                else { if (fingerController.IsFingerOpen()) { onlyThisFingerOpened = false; } }
            }
            return onlyThisFingerOpened;
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
        return "Finger (" + hand.ToString() + " - " + finger.ToString() + ")";
    }
}
