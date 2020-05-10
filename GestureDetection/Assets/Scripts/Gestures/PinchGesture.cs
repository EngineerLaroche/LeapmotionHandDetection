using UnityEngine;
using System.Collections;

/*****************************************************
 * CLASS:   PINCH GESTURE
 *
 * INFO:    Représente le geste de pincer les deux doigts.
 *          La détection du poing de la main dépend de 
 *          la tolérance entrée en paramètre.
 * 
 *****************************************************/
public class PinchGesture : GestureMediator
{
    // La main qui effectuer le geste
    public HandsE hand;

    // Tolerance de détection du geste en %
    [Range(0.0f, 1.0f)]
    public float tolerance = 0.8f;


    /*****************************************************
    * DETECTED PINCH GESTURE
    *
    * INFO:    Valide la détection du geste qui consiste à
    *          effectuer un pincement de doigt. La fonction 
    *          communique directement avec le médiateur
    *          du contrôleur de gestes.
    *          
    *****************************************************/
    public override bool IsDetectedGesture()
    {
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            return DetectionController.GetInstance().GetHand(hand).IsHandPinching(tolerance);
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
        return "Pinch (" + hand.ToString() + ")";
    }
}
