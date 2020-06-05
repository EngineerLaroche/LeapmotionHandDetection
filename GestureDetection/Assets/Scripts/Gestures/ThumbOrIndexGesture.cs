using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   THUMB OR/AND INDEX GESTURE
 *
 * INFO:    Représente le geste d'avoir le pouce et l'index
 *          de sortie lorsque les autres sont fermés pour
 *          une main donnée. L'utilisateur pourrait aussi 
 *          garder seulement un des deux doigts d'ouvert 
 *          pour altérer la fonctionnalité qui lui est attachée.
 * 
 *****************************************************/
public class ThumbOrIndexGesture : GestureMediator
{
    // Instance de la classe
    private static ThumbOrIndexGesture instance = null;

    // La main qui permet d'effectuer le geste
    public HandsE hand;


    /*****************************************************
    * AWAKE
    *
    * INFO:    Initialise l'instance de la classe pour
    *          rendre accessible la main et le doigt qui 
    *          ont été initialisés pour effectuer le geste.
    *          
    *****************************************************/
    void Awake()
    {
        //Initialise l'instance de cette classe
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }

    /*****************************************************
    * DETECTED SINGLE FINGER GESTURE
    *
    * INFO:    Valide la détection du geste qui consiste à
    *          garder le pouce ou l'index ou l'index et le pouce
    *          ouvert.La fonction communique directement avec 
    *          le médiateur du contrôleur de gestes.
    *          
    *****************************************************/
    public override bool IsDetectedGesture()
    {    
        bool isFingerOpen = false;
         
        //Si la main est détectée
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            //Recupère le contrôleur de la la main
            DetectionController.HandController detectedHand = DetectionController.GetInstance().GetHand(hand);

            for (int i = 0; i < 4; i++)
            {
                if (detectedHand.GetFinger((FingersE)i).IsFingerOpen())
                {
                    //Si le ou les bons doigts de la main sont ouverts
                    if ((FingersE)i == FingersE.pouce || (FingersE)i == FingersE.index) { isFingerOpen = true; }
                    else { return false; }
                }
            }
        }
        //DisplayDectedGesture(isFingerOpen);
        return isFingerOpen;
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
            SystemUIController.GetInstance().AddGesture("Pouce et Index " + hand.ToString());
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
        return "Finger(s) single/double";
    }

    /*****************************************************
    * GET HAND
    *
    * INFO:    Retourne la main entrée en paramètre pour
    *          effectuer la rotation.
    *
    *****************************************************/
    public HandsE GetHand()
    {
        return hand;
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static ThumbOrIndexGesture GetInstance()
    {
        return instance;
    }
}
