using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   PULL OBJECT CONTROLLER
 *
 * INFO:    Permet à l'utilisateur d'effectuer un 
 *          mouvement rapide de la main vers l'extérieur
 *          pour simuler l'action de tirer un objet.
 *          Si l'objet est sélectionné, il viendra se
 *          placer devant la camera principale.
 * 
 *****************************************************/
public class PullObjectController : HandDataStructure
{
    //Temps de deplacement total
    public float translationTime = 0.3f;

    //Distance finale de la position avec la camera
    public float offsetToCamera = 0.4f;

    private bool isPullObject = false;
    private bool wasPullingObject = false;   
    private bool isReadyToPull = false;

    private bool isPushBackObject = false;
    private bool isReadyToPush = false;
    

    /*****************************************************
    * PULL OBJECT
    *
    * INFO:    Evenement declenché lorsque une velocité
    *          vers l'arriere est détecté. (Tirer - Swipe)
    *           
    *****************************************************/
    public void PullObject()
    {
        isPullObject = true;
        //SoundController.GetInstance().PlaySound(AudioE.grab);
    }

    /*****************************************************
    * START PULL
    *
    * INFO:    Evenement declenché lorsque seulement le
    *          pouce et l'index de la main choisie sont
    *          ouverts. (Fusil) 
    *           
    *****************************************************/
    public void StartPull()
    {
        isReadyToPull = true;
    }

    /*****************************************************
    * STOP PULL
    *
    * INFO:    Evenement declenché lorsque le système ne
    *          détecte plus l'ouverture seule du pouce et 
    *          de l'index de la main choisie. (Fusil) 
    *           
    *****************************************************/
    public void StopPull()
    {
        isReadyToPull = false;
    }

    /*****************************************************
    * PUSH BACK
    *
    * INFO:    Evenement declenché lorsque une velocité
    *          vers l'avant est détecté et que le geste
    *          de tirer l'objet a déjà été effectué. 
    *           
    *****************************************************/
    public void PushBackObject()
    {
        isPushBackObject = true;
        //SoundController.GetInstance().PlaySound(AudioE.grab);
    }

    /*****************************************************
    * START PUSH
    *
    * INFO:    Evenement declenché lorsque seulement le
    *          pouce et l'index de la main choisie sont
    *          ouverts. (Fusil) 
    *           
    *****************************************************/
    public void StartPush()
    {
        isReadyToPush = true;
    }

    /*****************************************************
    * STOP PUSH
    *
    * INFO:    Evenement declenché lorsque le système ne
    *          détecte plus l'ouverture seule du pouce et 
    *          de l'index de la main choisie. (Fusil) 
    *           
    *****************************************************/
    public void StopPush()
    {
        isReadyToPush = false;
    }

    /*****************************************************
    * MOVE TO POSITION
    *
    * INFO:    Deplace un objet de sa position vers celle
    *          de la camera en un temps donné. 
    *           
    *****************************************************/
    public IEnumerator MoveToPosition(SelectedObject selectedObject, Vector3 position, float timeToMove)
    {
        Vector3 currentPos = selectedObject.TransformObject.position;
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            selectedObject.TransformObject.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Deplace un objet de sa position vers celle
    *          de la camera en un temps donné. 
    *           
    *****************************************************/
    void Update()
    {
        //La main utilisé pour sélectionner un objet 3D
        DetectionController.HandController handController = DetectionController.GetInstance().GetHand(SelectionController.GetInstance().GetHand());

        if (handController.IsHandSet())
        {
            //La liste des objets sélectionnés avec le raycast
            List<SelectedObject> objectsToPullList = SelectionController.GetInstance().GetSelectedObjects();

            if (objectsToPullList.Count > 0)
            {
                foreach (SelectedObject objectToPull in objectsToPullList)
                {
                    //Pour rapprocher (tirer) l'objet
                    if (isReadyToPull && isPullObject)
                    {
                        isPullObject = false;
                        wasPullingObject = true;

                        //La position de la camera avec un offset pour garder une certaine distance avec l'objet
                        Transform camera = Camera.main.transform;
                        Vector3 cameraPosition = camera.position + camera.forward * offsetToCamera;

                        //Demarre le deplacement de l'objet vers la camera aen un temps donné
                        StartCoroutine(MoveToPosition(objectToPull, cameraPosition, translationTime));
                    }

                    //Pour remettre (pousser) l'objet a sa position initial
                    if (isReadyToPush && isPushBackObject && wasPullingObject) 
                    {
                        isPushBackObject = false;
                        wasPullingObject = false;

                        //Demarre le deplacement de l'objet vers sa position initiale apres avoir tiré l'objet
                        StartCoroutine(MoveToPosition(objectToPull, objectToPull.GetInitialPosition(), translationTime));
                    }
                }
            }
        }
    }
}
