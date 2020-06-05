﻿using UnityEngine;
using System.Collections;
using Leap.Unity.Interaction.Examples;

/*****************************************************
 * CLASS:   FIST GESTURE
 *
 * INFO:    Représente le geste de fermer la main.
 *          La détection du poing de la main dépend de 
 *          la tolérance entrée en paramètre.
 * 
 *****************************************************/
public class FistGesture : GestureMediator
{
    // La main qui effectuer le geste
    public HandsE hand = HandsE.droite;

    // Tolerance de détection du geste en %
    [Range(0.0f, 1.0f)]
    public float tolerance = 0.8f;

    /*****************************************************
    * DETECTED FIST GESTURE
    *
    * INFO:    Valide la détection du geste qui consiste 
    *          à fermer la main (poing). La fonction 
    *          communique directement avec le médiateur
    *          du contrôleur de gestes.
    *           
    *****************************************************/
    public override bool IsDetectedGesture()
    {
        bool isFisting = false;
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            DetectionController.HandController handController = DetectionController.GetInstance().GetHand(hand);
            isFisting = handController.IsFist(tolerance) && handController.IsAllFingersClosed();    
        }
        DisplayDectedGesture(isFisting);
        return isFisting;
    }

    /*****************************************************
    * DISPLAY DETECTED GESTURE
    *
    * INFO:    Indique le geste détecté pour être affiché
    *          sur le UI System.
    *          
    *****************************************************/
    private void DisplayDectedGesture(bool isDetected)
    {
        if (isDetected)
        {
            SystemUIController.GetInstance().AddGesture("Poing " + hand.ToString());
        }
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
        return "Poing " + hand.ToString();
    }
}