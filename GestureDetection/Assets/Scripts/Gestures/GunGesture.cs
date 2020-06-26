using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   GUN GESTURE
 *
 * INFO:    Geste qui consiste a avoir seulement l'index
 *          et le pouce d'ouvert pour une main donnée.
 * 
 *****************************************************/
public class GunGesture : GestureMediator
{
    // La main qui effectuer le geste
    public HandsE hand = HandsE.droite;


    /*****************************************************
    * DETECTED ONLY INDEX AND THUMB GESTURE
    *
    * INFO:    Retourne vrai si seulement le pouce et 
    *          l'index sont ouverts.
    *           
    *****************************************************/
    public override bool IsDetectedGesture()
    {
        bool isGunGesture = false;

        //Si la bonne main est détectée
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            bool isIndexOpen = false;
            bool isThumbOpen = false;
            DetectionController.HandController handController = DetectionController.GetInstance().GetHand(hand);

            //Pour tous les doigts de la main
            foreach (DetectionController.FingerController finger in handController.GetFingers())
            {
                //Verifie si l'index et le pouce sont ouverts et que les autres doigts sont fermés
                if (finger == handController.GetFinger(FingersE.index) || finger == handController.GetFinger(FingersE.pouce))
                {
                    if (finger == handController.GetFinger(FingersE.index)) { isIndexOpen = finger.IsFingerOpen(); }
                    if (finger == handController.GetFinger(FingersE.pouce)) { isThumbOpen = finger.IsFingerOpen(); }
                }
                //else if (finger == handController.GetFinger(Fingers5.pouce)) { isOnlyIndexThumbOpen = finger.IsFingerOpen();  }
                else { if (finger.IsFingerOpen()) { isIndexOpen = isThumbOpen = false; } }
            }
            isGunGesture = isIndexOpen && isThumbOpen;
        }
        DisplayDectedGesture(isGunGesture);
        return isGunGesture;
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
            SystemUIController.GetInstance().AddGesture("Pointeur (fusil) " + hand.ToString());
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
        return "Gun (" + hand.ToString() + ")";
    }
}
