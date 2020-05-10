using UnityEngine;
using System.Collections;

/*****************************************************
 * CLASS:   BOTH FIST GESTURE
 *
 * INFO:    Représente le geste de fermer les deux mains.
 *          L'activation de chaque poing de la main dépend 
            de la tolérance entrée en paramètre.
 * 
 *****************************************************/
public class BothFistGesture : GestureMediator
{
    // Tolerance de détection du geste en %
    [Range(0.0f, 1.0f)]
    public float tolerance = 0.8f;

    /*****************************************************
    * DETECTED FIST GESTURE
    *
    * INFO:    Valide la détection du geste qui consiste 
    *          à fermer les deux mains (poing). La fonction 
    *          communique directement avec le médiateur
    *          du contrôleur de gestes.
    *           
    *****************************************************/
    public override bool IsDetectedGesture()
    {
        if (DetectionController.GetInstance().IsHandDetected(HandsE.gauche) &&
            DetectionController.GetInstance().IsHandDetected(HandsE.droite))
        {
            return DetectionController.GetInstance().GetHand(HandsE.gauche).IsFist(tolerance) && 
                    DetectionController.GetInstance().GetHand(HandsE.droite).IsFist(tolerance);
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
        return "Both Fist";
    }
}
