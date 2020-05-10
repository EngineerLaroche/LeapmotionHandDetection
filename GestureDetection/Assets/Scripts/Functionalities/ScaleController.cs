using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   SCALE CONTROLLER
 *
 * INFO:    Permet de redimensionner un objet 3D. 
 *          Les axes supportées sont X et Y. 
 *          L'action est effectuée lorsque l'utilisateur 
 *          ferme les deux mains et qu'il les déplacent vers
 *          l'extérieur ou l'intérieur du centre de la caméra.
 * 
 *****************************************************/
public class ScaleController : HandDataStructure
{
    //Instance de la classe
    private static ScaleController instance = null;

    //Valeur qui amplifie le scaling
    public float amplifyScale = 5f;

    [Space(10)]

    //Grosseur minimal et maximal de l'objet 3D
    public float minimumScale = 0.04f;
    public float maximumScale = 1.0f;

    //Variables bénéral
    private float lastDistance = 0f;
    private bool isScaling = false;


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
        //else { Destroy(this); }
    }

    /*****************************************************
    * GEST DISTANCE
    *
    * INFO:    Retourne la distance entre l'index (doigt) 
    *          de la main gauche et l'index (doigt) de la 
    *          main droite.
    *
    *****************************************************/
    private float GetDistance()
    {
        //Le doigt (index) de la main gauche et droite
        Vector3 leftFingerPosition = DetectionController.GetInstance().GetHand(HandsE.gauche).GetFinger(FingersE.index).GetFingertipPosition();
        Vector3 rightFingerPosition = DetectionController.GetInstance().GetHand(HandsE.droite).GetFinger(FingersE.index).GetFingertipPosition();
        return Vector3.Distance(leftFingerPosition, rightFingerPosition);
    }

    /*****************************************************
    * START SCALING
    *
    * INFO:    Evenement qui permet le scaling de d'un objet.
    *          Le redimensionnement commence par la récupération
    *          de la distance autorise le redimensionnement de 
    *          l'objet.
    * 
    *****************************************************/
    public void StartScaling()
    {
        lastDistance = GetDistance();
        isScaling = true;
        SoundController.GetInstance().PlaySound(AudioE.grab);
    }

    /*****************************************************
    * STOP SCALING
    *
    * INFO:    Evenement qui interdit le redimensionnement 
    *          de l'objet.
    * 
    *****************************************************/
    public void StopScaling()
    {
        isScaling = false;
    }

    /*****************************************************
    * RESET OBJECT SCALE
    *
    * INFO:    Remet l'objet a sa position initiale.
    *
    *****************************************************/
    public void ResetObjectScale()
    {
        //La liste des objets sélectionnés avec le raycast
        List<SelectedObject> objectsToScaleList = SelectionController.GetInstance().GetSelectedObjects();

        if (objectsToScaleList.Count > 0)
        {
            foreach (SelectedObject objectToScale in objectsToScaleList)
            {
                objectToScale.ResetScale();
            }
        }
    }

    /*****************************************************
    * SET SCALE AMPLIFICATION
    *
    * INFO:    Initialise la vouvelle valeur d'amplification
    *          de translation.
    * 
    *****************************************************/
    public void SetScaleAmplification(float _amplifyScale)
    {
        amplifyScale = _amplifyScale;
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Recupere la distance du scaling et met à jour 
    *          l'objet à chaque frame. Un amplificateur de 
    *          mouvement est utilisé pour faciliter le
    *          redimensionnement.
    *          
    *          Ex: Si l'amplificateur est de 2.0f et que les 
    *          mains se déplacent de 1" vers l'extérieur, 
    *          l'objet va grossir de 2" dans tous les sens.
    *
    *****************************************************/
    void Update()
    {
        //Si la manipulation contrôlé d'objet est activé et que les deux mains sont détectés.
        if (isScaling && DetectionController.GetInstance().IsBothHandsDetected() &&
            PalmUIController.GetInstance().IsManipulationControlled())
        {
            //La liste des objets sélectionnés avec le raycast
            List<SelectedObject> objectsToScaleList = SelectionController.GetInstance().GetSelectedObjects();

            if (objectsToScaleList.Count > 0)
            {
                float newDistance = GetDistance();
                float distance = newDistance - lastDistance;

                foreach (SelectedObject objectToScale in objectsToScaleList)
                {
                    // Met à jour la dimension de l'objet (X,Y,Z) avec l'amplificateur pris en compte.
                    objectToScale.TransformObject.localScale += new Vector3(distance, distance, distance) * amplifyScale;

                    // Évite que le redimensionnement de l'objet soit trop petit ou trop grand
                    if (objectToScale.GetScale().x < minimumScale)
                    {
                        objectToScale.TransformObject.localScale = new Vector3(minimumScale, minimumScale, minimumScale);
                    }
                    if (objectToScale.GetScale().x > maximumScale)
                    {
                        objectToScale.TransformObject.localScale = new Vector3(maximumScale, maximumScale, maximumScale);
                    } 
                }
                lastDistance = newDistance;
            }
        }
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static ScaleController GetInstance()
    {
        return instance;
    }
}
