using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   TRANSLATION CONTROLLER
 *
 * INFO:    Permet d'effectuer une translation sur un 
 *          objet 3D. Les axes supportées sont X, Y et Z. 
 *          L'action est effectuée lorsque l'utilisateur 
 *          ferme la main droite et se met à bouger la main. 
 * 
 *****************************************************/
public class TranslationController : HandDataStructure
{
    //Instance de la classe
    private static TranslationController instance = null;

    //La main qui effectue la translation
    public HandsE hand = HandsE.droite;

    //Valeur qui amplifie le deplacement
    public float amplifyMovement = 3f;

    private Vector3 lastHandPosition;
    private bool isTranslating = false;


    /*****************************************************
    * AWAKE
    *
    * INFO:    Instance de la classe.
    *
    *****************************************************/
    private void Awake()
    {
        //Initialise l'instance de cette classe
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }

    /*****************************************************
    * GET POSITION
    *
    * INFO:    Retourne la position (x,y,z) de la main 
    *          qui effectue la geste Fist.
    * 
    *****************************************************/
    private Vector3 GetStartHandPosition()
    {
        return DetectionController
            .GetInstance()
            .GetHand(hand)
            .GetHandPosition();
    }

    /*****************************************************
    * START TRANSLATION
    *
    * INFO:    Evenement qui demarre la translation de l'objet.
    *          Recupere la position de la main initiale pour
    *          pouvoir calculer la distance. On desactive
    *          la gravité de l'objet pour le manipuler facilement.
    * 
    *****************************************************/
    public void StartTranslation()
    {
        lastHandPosition = GetStartHandPosition();
        isTranslating = true;

        //Arret temporaire de la gravité de l'objet s'il en a.
        GravityController.GetInstance().ToAllowGravity(false, true);
    }

    /*****************************************************
    * STOP TRANSLATION
    *
    * INFO:    Evenement qui arrete la translation de l'objet.
    *          Permet aussi de remettre l'état de la gravité 
    *          de l'objet comme il etait.
    * 
    *****************************************************/
    public void StopTranslation()
    {
        isTranslating = false;

        //Retabli la gravité de l'objet s'il en avait
        GravityController.GetInstance().ToAllowGravity(true, false);
    }

    /*****************************************************
    * RESET OBJECT POSITION
    *
    * INFO:    Remet l'objet a sa position initiale.
    *
    *****************************************************/
    public void ResetObjectPosition()
    {
        //La liste des objets sélectionnés avec le raycast
        List<SelectedObject> objectsToTranslateList = SelectionController.GetInstance().GetSelectedObjects();
        if (objectsToTranslateList.Count > 0)
        {
            foreach (SelectedObject objectToTranslate in objectsToTranslateList)
            {
                objectToTranslate.ResetPosition();
            }
        }
    }

    /*****************************************************
    * SET TRANSLATION AMPLIFICATION
    *
    * INFO:    Initialise la vouvelle valeur d'amplification
    *          de translation.
    * 
    *****************************************************/
    public void SetTranslationAmplification(float _amplifyMovement)
    {
        amplifyMovement = _amplifyMovement;
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static TranslationController GetInstance()
    {
        return instance;
    }

    /*****************************************************
    * METHOD:  UPDATE
    *
    * INFO:    Recupere la distance de la translation et met 
    *          à jour l'objet à chaque frame. Un amplificateur 
    *          de mouvement est utilisé pour faciliter les 
    *          déplacements.
    *          
    *          Ex: Si l'amplificateur est de 2.0f et que la 
    *          main se déplace de 1" vers la gauche, l'objet 
    *          va se déplacer de 2" vers la gauche.
    * 
    *****************************************************/
    void Update()
    {
        //Si la manipulation contrôlé d'objet est activé
        if (PalmUIController.GetInstance().IsManipulationControlled())
        {
            // Si la main initialisé avec le geste Fist est détecté, on effectue une translation
            if (isTranslating && DetectionController.GetInstance().IsHandDetected(hand))
            {
                //La liste des objets sélectionnés avec le raycast
                List<SelectedObject> objectsToTranslateList = SelectionController.GetInstance().GetSelectedObjects();
                if (objectsToTranslateList.Count > 0)
                {
                    // Calcul la distance du déplacement
                    Vector3 newHandPosition = GetStartHandPosition();
                    Vector3 handDistance = newHandPosition - lastHandPosition;

                    foreach (SelectedObject objectToTranslate in objectsToTranslateList)
                    {
                        // Met à jour la position de l'objet (X,Y,Z) avec l'amplificateur pris en compte.
                        objectToTranslate.TransformObject.position += handDistance * amplifyMovement;
                    }
                    lastHandPosition = newHandPosition;
                }
            }
        }
    }
}
