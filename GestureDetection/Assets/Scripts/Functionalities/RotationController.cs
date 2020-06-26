using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using Leap;

/*****************************************************
 * CLASS:   ROTATION CONTROLLER
 *
 * INFO:    Permet d'effectuer une rotation sur un objet 3D. 
 *          Les axes supportées sont X et Y. La rotation 
 *          démarre lorsque le pouce ou l'index ou le pouce 
 *          et l'index sont ouverts et que les autres doigts
 *          sont fermés. Le pouce représente une rotation sur
 *          l'axe des X, l'index représente une rotation sur
 *          l'axe des Y et les deux doigts ouverts représentent
 *          une rotation sur les axes X et Y. Le sens de la 
 *          rotation dépend de la direction que pointe le bout
 *          du bout du doigt.
 * 
 *****************************************************/
public class RotationController : HandDataStructure
{
    //Objet 3D à rotationner
    public float amplifyRotation = 2f;
    public bool isRotationX = true;
    public bool isRotationY = true;

    //Activation de la rotation Swipe
    [Header("Activate Swipe Rotation")]
    public bool isSwipeRotation = true;
    [ConditionalHide("isSwipeRotation")]
    public bool isLockRotation = true;
    [ConditionalHide("isSwipeRotation")]
    public float rotationAngle = 90f;
    [ConditionalHide("isSwipeRotation")]
    public float secondsToRotate = 0.5f;

    //Activation de la rotation Thumb & Index
    [Header("Activate Index/Thumb Rotation")]
    public bool isFingerRotation = false;
    public float amplifyFingerRotation = 3f;

    //Instance de la classe
    private static RotationController instance = null;

    //Le controleur de la main 
    private DetectionController.HandController detectedHand;

    //Parametres de la rotation Thumb & Index
    Vector3 startPosition, stopPosition;
    private bool isThumbOpen = false;
    private bool isIndexOpen = false;

    //Le type de swipe détecté
    private string swipeType = "";

    //private Coroutine swipeCoroutine;
    private bool allowCoroutine = false;


    /*****************************************************
    * AWAKE
    *
    * INFO:    Instance de la classe et rotation initiale.
    *
    *****************************************************/
    private void Awake()
    {
        //Initialise l'instance de cette classe
        if (instance == null) { instance = this; }
        //else { Destroy(this); }
    }

    /*****************************************************
    * START ROTATION
    *
    * INFO:    Evenement qui autorise la rotation d'un objet.
    *          La rotation commence avec la récupération
    *          de la position actuelle du bout du doigt.
    *
    *****************************************************/
    public void StartRotation()
    {
        startPosition = GetFingerPosition();
        //SoundController.GetInstance().PlaySound(AudioE.grab);
    }

    /*****************************************************
    * STOP ROTATION
    *
    * INFO:    Evenement qui interdit la rotation d'un objet.
    *          La rotation de l'objet 3D n'est pas effectuée
    *          lorsque les coordonées de rotation sont tous 
    *          à zéro. 
    *
    *****************************************************/
    public void StopRotation()
    {
        stopPosition = Vector3.zero;
    }

    /*****************************************************
    * UPDATE FINGER ROTATION
    *
    * INFO:    Evenement qui permet de maintenir a jour la
    *          rotation de l'objet. Permet aussi d'appliquer 
    *          et modifier la vitesse de rotation selon 
    *          les valeurs entrées en paramètres. 
    *          
    *****************************************************/
    public void UpdateFingerRotation()
    {
        //Si la manipulation contrôlé d'objet est activé
        if (PalmUIController.GetInstance().IsManipulationControlled())
        {
            //La liste des objets sélectionnés avec le raycast
            List<SelectedObject> objectsToRotateList = SelectionController.GetInstance().GetSelectedObjects();

            if (objectsToRotateList.Count > 0)
            {
                foreach (SelectedObject objectToRotate in objectsToRotateList)
                {
                    //Plus la main est près de l'olbjet 3D, plus la vitesse de rotation sera réduite
                    Vector3 torque = GetFingerPosition() - startPosition;
                    Vector3 newTorque = Vector3.zero;

                    //Le pouce et/ou l'index est ouvert, donc rotation sur l'axe X et/ou Y
                    if (isThumbOpen && isRotationX) { newTorque.y = -torque.x; }
                    if (isIndexOpen && isRotationY) { newTorque.x = torque.y; }

                    //Applique la rotation en fonction de la force
                    Vector3 rotationSpeed = newTorque * amplifyFingerRotation;
                    Vector3 dynamicRotation = objectToRotate.TransformObject.worldToLocalMatrix * rotationSpeed;
                    objectToRotate.TransformObject.Rotate(dynamicRotation);
                }
            }
        }
    }

    /*****************************************************
    * UPDATE SWIPE ROTATION
    *
    * INFO:    Evenement qui permet de maintenir a jour la 
    *          rotation de l'objet lorsque l'utilisateur effectue 
    *          un geste Swipe. 
    *          
    *          *** TO OPTIMIZE ***
    *          
    *****************************************************/
    public void UpdateSwipeRotation()
    {
        //Si la manipulation contrôlé d'objet est activé
        if (PalmUIController.GetInstance().IsManipulationControlled())
        {
            //La liste des objets sélectionnés avec le raycast
            List<SelectedObject> objectsToRotateList = SelectionController.GetInstance().GetSelectedObjects();

            if (objectsToRotateList.Count > 0)
            {
                //Le type de Swipe effectué (gauche, droite, haut, bas)
                swipeType = GestureMediator.GetGestureType();
                if (swipeType.Contains("Swipe"))
                {
                    foreach (SelectedObject objectToRotate in objectsToRotateList)
                    {
                        detectedHand = DetectionController.GetInstance().GetHand(GestureMediator.GetDetectedHand());
                        float velocityX = 0f;
                        float velocityY = 0f;

                        //La velocité du swipe en X ou Y
                        if (isRotationX) { velocityX = -detectedHand.GetHandVelocity().x; }
                        if (isRotationY) { velocityY = detectedHand.GetHandVelocity().y; }

                        allowCoroutine = true;

                        //Rotation horizontale (Swipe gauche ou droite)
                        if (isRotationX && (swipeType.Contains("droite") || swipeType.Contains("gauche")))
                        {
                            //Demarre la rotation horizontale selon le type choisi (swipe lock angle / swipe velocity)
                            Vector3 axis = Mathf.Sign(velocityX) * Vector3.up;
                            StartCoroutine(isLockRotation ?
                                RotateWhenSwipe(objectToRotate.TransformObject, axis, rotationAngle, secondsToRotate) :
                                RotateFreeWhenSwipe(objectToRotate.TransformObject, axis, velocityX));
                        }

                        //Rotation verticale (Swipe haut ou bas)
                        if (isRotationY && (swipeType.Contains("haut") || swipeType.Contains("bas")))
                        {
                            //Demarre la rotation verticale selon le type choisi (swipe lock angle / swipe velocity)
                            Vector3 axis = Mathf.Sign(velocityY) * Vector3.right;
                            StartCoroutine(isLockRotation ?
                                RotateWhenSwipe(objectToRotate.TransformObject, axis, rotationAngle, secondsToRotate) :
                                RotateFreeWhenSwipe(objectToRotate.TransformObject, axis, velocityY));
                        }
                    }
                }
            }
        }
    }

    /*****************************************************
    * ROTATE WHEN SWIPE
    *
    * INFO:     Permer d'effectuer une rotation sur un GameObject
    *           avec un angle donné sur une période donnée autour
    *           de l'axe donné. Lorsque la rotation est déclenchée,
    *           l'objet tourne automatiquement jusqu'a l'angle voulu.
    *
    * SOURCE:   https://stackoverflow.com/questions/56183422/unity-quaternion-euler-rotate-only-around-y-axis
    * 
    *****************************************************/
    private IEnumerator RotateWhenSwipe(Transform obj, Vector3 axis, float angle, float sec)
    {
        float rotationSpeed = angle / sec;
        float rotated = 0;
        do
        {
            float newAngle = Mathf.Min(angle - rotated, rotationSpeed * Time.deltaTime);
            rotated += newAngle;
            obj.Rotate(axis * newAngle, Space.World);
            yield return null;
        } while (rotated < angle && allowCoroutine);
    }

    /*****************************************************
    * ROTATE FREE WHEN SWIPE
    *
    * INFO:     Permer d'effectuer une rotation sur un GameObject.
    *           Plus la vélocité du swipe est élevée, plus l'objet
    *           va effectuer une rotation rapide. Par defaut, la 
    *           rotation ralenti jusqu'à 0 si aucuns autres swipe 
    *           sont effectués. L'objet rotationne selon la velocité
    *           et l'axe donné.
    *
    *****************************************************/
    private IEnumerator RotateFreeWhenSwipe(Transform obj, Vector3 axis, float velocity)
    {
        float currentSpeed = Mathf.Abs(velocity) * amplifyRotation;
        do
        {
            currentSpeed -= Time.deltaTime;
            obj.Rotate(axis * currentSpeed, Space.World);
            yield return null;
        } while (currentSpeed > 0f && allowCoroutine);
    }


    /*****************************************************
    * GET FINGER POSITION
    *
    * INFO:    Supporte la rotation effectuée avec l'index 
    *          et/ou le pouce.
    *          
    *          Retourne la position du bout du doigt. On
    *          met la priorité sur l'index. Sinon ce sera
    *          avec la position du bout du pouce pour indiquer
    *          quel sens la rotation doit prendre. 
    *
    *****************************************************/
    private Vector3 GetFingerPosition()
    {
        //La main utilisée pour effectuer le geste Pouce ou Index.
        HandsE hand = ThumbOrIndexGesture.GetInstance().GetHand();

        //Si la main initialisée pour effecuer la rotation est détectée
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            Vector3 fingertip = Vector3.zero;
            detectedHand = DetectionController.GetInstance().GetHand(hand);

            if (isIndexOpen) { fingertip += detectedHand.GetFinger(FingersE.index).GetFingertipPosition(); }
            else if (isThumbOpen) { fingertip += detectedHand.GetFinger(FingersE.pouce).GetFingertipPosition(); }
            return fingertip;
        }
        return Vector3.zero;
    }

    /*****************************************************
    * UPDATE THUMB/INDEX ROTATION
    *
    * INFO:    Maintient à jour la rotation de l'objet 3D
    *          en fonction du pouce et de l'index de la main.
    *          Ajuste dynamiquement la vitesse de rotation
    *          selon la position du ou des doigts qui 
    *          causent la rotation. C'est ici qu'on valide
    *          quels sont les doigts ouverts.
    *          
    *          *** TO OPTIMIZE ***
    *          
    *****************************************************/
    private void ThumbIndexRotation()
    {
        //Si la main qui permet d'effecuer la rotation est détectée.
        //Si la manipulation contrôlé d'objet est activé.
        if (DetectionController.GetInstance().IsHandDetected(ThumbOrIndexGesture.GetInstance().GetHand()) &&
            PalmUIController.GetInstance().IsManipulationControlled())
        {
            //La liste des objets sélectionnés avec le raycast
            List<SelectedObject> objectsToRotateList = SelectionController.GetInstance().GetSelectedObjects();

            if (objectsToRotateList.Count > 0)
            {
                //Recupère le contrôleur de la la main
                detectedHand = DetectionController.GetInstance().GetHand(ThumbOrIndexGesture.GetInstance().GetHand());

                //Change l'etat des doigts si le pouce et/ou l'index est ouvert
                if (detectedHand.GetFinger(FingersE.pouce).IsFingerOpen() || detectedHand.GetFinger(FingersE.index).IsFingerOpen())
                {
                    isThumbOpen = detectedHand.GetFinger(FingersE.pouce).IsFingerOpen() ? true : false;
                    isIndexOpen = detectedHand.GetFinger(FingersE.index).IsFingerOpen() ? true : false;
                }

                foreach (SelectedObject objectToRotate in objectsToRotateList)
                {
                    //Applique la rotation en fonction de la force
                    Vector3 rotationSpeed = stopPosition * amplifyFingerRotation;
                    Vector3 dynamicRotation = objectToRotate.TransformObject.worldToLocalMatrix * rotationSpeed;
                    objectToRotate.TransformObject.Rotate(dynamicRotation);

                    startPosition = Vector3.Lerp(stopPosition, Vector3.zero, Time.deltaTime);
                }
            }
        }
    }

    /*****************************************************
    * RESET OBJECT ROTATION
    *
    * INFO:    Remet les angles par defaut des objets.
    *
    *****************************************************/
    public void ResetObjectRotation()
    {
        //Arret du Coroutine et la rotation d'objets
        allowCoroutine = false;

        //if(swipeCoroutine != null) StopCoroutine(swipeCoroutine);
        
        //La liste des objets sélectionnés avec le raycast
        List<SelectedObject> objectsToRotateList = SelectionController.GetInstance().GetSelectedObjects();
        if (objectsToRotateList.Count > 0)
        {
            foreach (SelectedObject objectToRotate in objectsToRotateList)
            {
                objectToRotate.ResetRotation();
            }
        }
    }

    /*****************************************************
    * SET LOCK ROTATION
    *
    * INFO:    Permet d'activer ou désactiver la rotation
    *          par bloc d'angle. Si c'est désactivé, le
    *          temps de rotation dépendera de la force du swipe.
    *
    *****************************************************/
    public void SetLockRotation(bool _isLockRotation)
    {
        //Arret du Coroutine et la rotation d'objets
        allowCoroutine = false;
        //StopCoroutine(swipeCoroutine);

        isLockRotation = _isLockRotation;
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static RotationController GetInstance()
    {
        return instance;
    }

    /*****************************************************
    * UPDATE SWIPE ROTATION
    *
    * INFO:    Si l'utilisateur active la fonctionnalité qui
    *          permet d'effectuer une rotation avec le pouce
    *          et/ou l'index de la main gauche, alors on
    *          maintient a jour la rotation de l'objet.
    *          
    *****************************************************/
    void Update()
    {
        if (isFingerRotation) { ThumbIndexRotation(); }
    }


    // *** TODO - TO CHECK ***
    public void SetAngle(float _angle) { rotationAngle = _angle; }
    public float GetAngle() { return rotationAngle; }
    public void SetRotationX(bool _isSwipeRotationX) { isRotationX = _isSwipeRotationX; }
    public void SetRotationY(bool _isSwipeRotationY) { isRotationY = _isSwipeRotationY; }

}
