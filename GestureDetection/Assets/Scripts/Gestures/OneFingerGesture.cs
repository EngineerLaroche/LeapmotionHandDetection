using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*****************************************************
 * CLASS:   ONE FINGER GESTURE
 *
 * INFO:    Geste qui consiste a avoir seulement un doigt
 *          d'ouvert pour une main donnée.
 * 
 *****************************************************/
public class OneFingerGesture : GestureMediator
{
    //Instance de la classe
    private static OneFingerGesture instance = null;

    // La main et le doigt qui effectue le geste
    public HandsE hand = HandsE.droite;
    public FingersE finger = FingersE.index;
    private bool onlyThisFingerOpened = false;


    //Initialise l'instance de la classe (Singleton)
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    /*****************************************************
    * DETECTED ONLY INDEX GESTURE
    *
    * INFO:    Retourne vrai si seulement ce doigt est ouvert.
    *           
    *****************************************************/
    public override bool IsDetectedGesture()
    {
        //Si la main du geste n'est pas détectée.
        if (!DetectionController.GetInstance().IsHandDetected(hand)) { return false; }

        DetectionController.HandController handController = DetectionController.GetInstance().GetHand(hand);
        onlyThisFingerOpened = false;

        //Pour tous les doigts de la main, partant de l'auriculaire
        for (int i = 0; i < (int)FingersE.auriculaire; i++)
        {
            FingersE _finger = FingersE.pouce + i;

            if (handController.GetFinger(_finger).IsFingerOpen())
            {
                //Verifie si seulement le doigt est ouvert et que les autres sont fermés
                if (_finger == finger) { onlyThisFingerOpened = true; }
                else { return false; }
            }
        }
        DisplayDectedGesture(onlyThisFingerOpened);
        return onlyThisFingerOpened;
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
            SystemUIController.GetInstance().AddGesture(finger.ToString() + " " + hand.ToString());
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
        return finger.ToString() + " " + hand.ToString();
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static OneFingerGesture GetInstance()
    {
        return instance;
    }

    public bool IsPointing()
    {
        return onlyThisFingerOpened;
    }
}