using UnityEngine;
using System.Collections;

/*****************************************************
 * CLASS:   BOTH HANDS PINCH GESTURE
 *
 * INFO:    Représente le geste de pincer les deux doigts
 *          de chaque main (gauche et droite).
 *          La détection du poing de la main dépend de 
 *          la tolérance entrée en paramètre.
 * 
 *****************************************************/
public class BothPinchGesture : GestureMediator
{
    //Instance de la classe
    private static BothPinchGesture instance = null;

    // Tolerance de détection du geste en %
    [Range(0.0f, 1.0f)]
    public float tolerance = 0.8f;
    public float distance = 20f;
    private bool isBothPinch = false;


    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    /*****************************************************
    * DETECTED BOTH PINCH GESTURE
    *
    * INFO:    Valide la détection du geste qui consiste à
    *          effectuer un pincement de doigts avec les 
    *          deux mains. La fonction communique directement 
    *          avec le médiateur du contrôleur de gestes.
    *          Pincement du pouce et de l'index.
    *          
    *****************************************************/
    public override bool IsDetectedGesture()
    {
        isBothPinch = false;
        if (DetectionController.GetInstance().IsBothHandsDetected() && DetectionController.GetInstance().IsBothHandsVisible())
        {
            //Tolerence acceptable du pincement
            bool leftHandPinching = DetectionController.GetInstance().GetHand(HandsE.gauche).IsHandPinching(tolerance);
            bool rightHandPinching = DetectionController.GetInstance().GetHand(HandsE.droite).IsHandPinching(tolerance);
            //Pincement avec l'index et le pouce
            bool leftIndexThumbPinch = DetectionController.GetInstance().GetHand(HandsE.gauche).IsPinchDistance(distance);
            bool rightIndexThumbPinch = DetectionController.GetInstance().GetHand(HandsE.droite).IsPinchDistance(distance);

            isBothPinch = leftHandPinching && rightHandPinching && leftIndexThumbPinch && rightIndexThumbPinch && !BothFistGesture.GetInstance().IsBothFisting();
        }
        DisplayDectedGesture(isBothPinch);
        return isBothPinch;
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
            SystemUIController.GetInstance().AddGesture("Double pincement");
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
        return "Both Pinch";
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static BothPinchGesture GetInstance()
    {
        return instance;
    }

    public bool IsBothPinching()
    {
        return isBothPinch;
    }
}
