using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using Leap;
using Leap.Unity;
using System;

/*****************************************************
 * CLASS:   DETECTION CONTROLLER
 *
 * INFO:    Permet de controler la détection et 
 *          l'utilisation de la main gauche ou de la 
 *          main droite. 
 * 
 *****************************************************/
partial class DetectionController : HandDataStructure
{
    //Instance de la classe
    private static DetectionController instance = null;

    //LeapMotion
    LeapServiceProvider leapService = null;

    //Contrôleur de la main gauche et droite
    HandController leftHandController = null;
    HandController rightHandController = null;

    //Visibilité de la main gauche et droite
    private bool isLeftHandVisible = false;
    private bool isRightHandVisible = false;
    private bool isObjectsMaterialCleared = false;

    //Nombre de mains détectées dans la scène
    private int numberOfHands = 0;

    //Dictionnaire de collisions des doigts de la main gauche et droite
    Dictionary<FingersE, GameObject> rightFingerCollisions = new Dictionary<FingersE, GameObject>();
    Dictionary<FingersE, GameObject> leftFingerCollisions = new Dictionary<FingersE, GameObject>();

    // ***En cours*** Événements d'entrées et de sorties de la main gauche et droite.
    //public UnityEvent leftHandEnter = null;
    //public UnityEvent leftHandExit = null;
    //public UnityEvent rightHandEnter = null;
    //public UnityEvent rightHandExit = null;


    /*****************************************************
    * AWAKE
    *
    * INFO:    Initialise l'instance de la classe ainsi que
    *          le contrôleur de chaque main et recupère le
    *          fournisseur de service LeapMotion.
    *
    *****************************************************/
    void Awake()
    {
        //Initialise l'instance de cette classe
        if (instance == null) { instance = this; }
        else { Destroy(this); }

        leapService = FindObjectOfType<LeapXRServiceProvider>();
        leftHandController = new HandController();
        rightHandController = new HandController();
    }

    /*****************************************************
    * GET HAND
    *
    * INFO:    Valide et retourne la bonne main (gauche ou droite).
    *
    *****************************************************/
    public HandController GetHand(HandsE _hand)
    {
        return _hand == HandsE.gauche ? leftHandController : rightHandController;
    }

    /*****************************************************
    * GET DISTANCE BETWEEN HANDS
    *
    * INFO:    Retourne la distance entre la main gauche
    *          et la main droite.
    *
    *****************************************************/
    public float GetDistanceBetweenHands()
    {
        Vector3 leftHandPosition = leftHandController.GetHandPosition();
        Vector3 rightHandPosition = rightHandController.GetHandPosition();
        return Vector3.Distance(leftHandPosition, rightHandPosition);
    }

    /*****************************************************
    * IS HAND DETECTED
    *
    * INFO:    Valide l'utilisation et la visibilité d'une 
    *          main en particulier.
    *
    *****************************************************/
    public bool IsHandDetected(HandsE _hand)
    {
        bool isVisible = _hand == HandsE.gauche ? isLeftHandVisible : isRightHandVisible;
        return GetHand(_hand).IsHandSet() && isVisible;
    }

    /*****************************************************
    * IS BOTH HANDS DETECTED
    *
    * INFO:    Valide la détection de la main gauche et de 
    *          la main droite et retourne une réponse.
    *
    *****************************************************/
    public bool IsBothHandsDetected()
    {
        return IsHandDetected(HandsE.gauche) && IsHandDetected(HandsE.droite);
    }

    /*****************************************************
    * IS BOTH HANDS VISIBLE
    *
    * INFO:    Valide la visibilité et la détection de la 
    *          main gauche et de la main droite et retourne
    *          une réponse.
    *
    *****************************************************/
    public bool IsBothHandsVisible()
    {
        return numberOfHands >= 2 && IsBothHandsDetected();
    }

    /*****************************************************
    * UPDATE HAND STATE
    *
    * INFO:    Met a jour l'état d'une main en particulier
    *          ainsi que son contrôleur.
    *
    *****************************************************/
    private void UpdateHandState(Hand _leapHand)
    {
        if (_leapHand.IsLeft) //Main gauche
        {
            leftHandController.SetHand(_leapHand);
            UpdateFingerState(leftHandController);
            isLeftHandVisible = true;
            if (numberOfHands == 1) { isRightHandVisible = false; }
        }
        else //Main droite
        {
            rightHandController.SetHand(_leapHand);
            UpdateFingerState(rightHandController);
            isRightHandVisible = true;
            if (numberOfHands == 1) { isLeftHandVisible = false; }
        }
    }

    /*****************************************************
    * UPDATE FINGER STATE
    *
    * INFO:    Met a jour l'état des 5 doigts de la main
    *          et de son controleur.
    *
    *****************************************************/
    private void UpdateFingerState(HandController _handController)
    {
        for (int i = 0; i < 5; i++)
        {
            FingersE fingersE = FingersE.pouce + i;
            if (leftFingerCollisions.ContainsKey(fingersE))
            {
                leftFingerCollisions[fingersE].transform.position = 
                    _handController.GetFinger(fingersE).GetFingertipPosition();
                leftFingerCollisions[fingersE].SetActive(true);
            }

            if (numberOfHands == 1)
            {
                if (rightFingerCollisions.ContainsKey(fingersE))
                {
                    rightFingerCollisions[fingersE].SetActive(false);
                }
            }
        }
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Met à jour les mains de la scène actuelle
    *          et gère les sorties de main de la zone de 
    *          détection du LeapMotion. Initialise aussi
    *          le raycast et la détection d'objet 3D.
    *
    *****************************************************/
    void Update()
    {
        //Cadre de la scène et le nombre de mains détecté
        Frame frame = leapService.CurrentFrame;
        numberOfHands = frame.Hands.Count;

        //Sauvegarde l'état des deux mains au cas si elles sortent de la zone de détection
        bool leftHandWasVisible = isLeftHandVisible;
        bool rightHandWasVisible = isRightHandVisible;

        //Si le LeapMotion détecte au moins une main dans la scène
        if (numberOfHands > 0 && leapService.IsConnected())
        {
            isObjectsMaterialCleared = false;

            // Initialise les mains détectées par le LeapMotion
            foreach (Hand hand in frame.Hands) { UpdateHandState(hand); }

            //Demarre le raycast et la selection d'objet 3D
            SelectionController.GetInstance().InitiateRaycast();
        }
        else
        {
            isLeftHandVisible = isRightHandVisible = false;

            //Si la main du raycast n'est plus détecté, on reset une fois le highlight des objets
            if (!isObjectsMaterialCleared)
            {
                SelectionController.GetInstance().ClearTargetedObjects();
                isObjectsMaterialCleared = true;
            }
        }

        //Si la main gauche sort de la zone de détection du LeapMotion
        if (leftHandWasVisible != isLeftHandVisible)
        {
            //*** TODO: to test and check if it could really be usefull ...
            //if (leftHandIsVisible){ leftHandEnter.Invoke(); }
            //else { leftHandExit.Invoke(); }
        }

        //Si la main droite sort de la zone de détection du LeapMotion
        if (rightHandWasVisible != isRightHandVisible)
        {
            //*** TODO: to test and check if it could really be usefull ...
            //if (rightHandIsVisible){ rightHandEnter.Invoke(); }
            //else { rightHandExit.Invoke(); }
        }
    }

    /*****************************************************
    * GET NUMBERS OF HAND
    *
    * INFO:    Retourne le nombre de main détecté par le
    *          LeapMotion.
    *
    *****************************************************/
    public int GetNumbersOfHand()
    {
        return numberOfHands;
    }

    /*****************************************************
    * GET LEAP SERVICE
    *
    * INFO:    Retourne le fournisseur de service LeapMotion.
    *
    *****************************************************/
    public LeapServiceProvider GetLeapService()
    {
        return leapService;
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static DetectionController GetInstance()
    {
        return instance;
    }
}
