using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


/*****************************************************
 * CLASS:   GESTURE MEDIATOR
 *
 * INFO:    Permet de valider la détection des gestes 
 *          entrés en paramètres dans le GestureController.
 *          
 *          Cette classe est utilisée pour garder en mémoire
 *          une liste de 'components' (gestes). 
 *               
 *          Les classes de gestes communiquent avec cette 
 *          classe qui sert de médiateur entre les 
 *          gestes et le contrôleur de gestes.
 * 
 *****************************************************/
[RequireComponent(typeof(GestureController))]
public class GestureMediator : HandDataStructure
{
    //Si un geste a été détecté
    private bool isGestureDetected = false;

    //Le type de geste détecté
    public static string gestureDetectedType = "";

    //La main du geste détecté
    public static HandsE handDetected = HandsE.inconnu;
    //private bool soundPlayed = true;

    
    /*****************************************************
     * IS GESTURE DETECTION
     *
     * INFO:    Permet de valider la détection d'un nouveau 
     *          geste et de retourner la réponse. Permet
     *          aussi de recuperer le type de geste en cours.
     *
     *****************************************************/
    public bool IsGestureDetected()
    {
        bool isDetected = IsDetectedGesture();
        if (isGestureDetected != isDetected)
        {
            //soundPlayed = false;
            isGestureDetected = isDetected;
            gestureDetectedType = isDetected ? DetectedGestureName() : "";
            handDetected = isDetected ? DetectedHand() : HandsE.inconnu;

            PlaySoundOnGesture();
        }

        return isDetected;
    }

    //TODO***
    private void PlaySoundOnGesture()
    {
        if (gestureDetectedType.Contains("Both Fist")) { SoundController.GetInstance().PlaySound(AudioE.select); }
    }

    /*****************************************************
    * IS DETECTED GESTURE
    *
    * INFO:    Permet à une classe d'utiliser virtuellement
    *          cette fonction pour modifier la réponse de
    *          détection de geste. Par defaut, cette fonction 
    *          retourne fasle.
    *
    *****************************************************/
    public virtual bool IsDetectedGesture()
    {
        return false;
    }

    /*****************************************************
    * GESTURE TYPE
    *
    * INFO:    Permet à une classe d'utiliser virtuellement
    *          cette fonction pour modifier la réponse du
    *          type de geste détecté. Par defaut, cette 
    *          fonction retourne un string vide.
    *
    *****************************************************/
    public virtual string DetectedGestureName()
    {
        return "";
    }

    /*****************************************************
    * GET GESTURE TYPE
    *
    * INFO:    Retourne le type de geste qui a été détecté.
    *
    *****************************************************/
    public static string GetGestureType()
    {
        return gestureDetectedType;
    }

    /*****************************************************
    * GET DETECTED HAND
    *
    * INFO:    Retourne la main du geste détecté.
    *
    *****************************************************/
    public static HandsE GetDetectedHand()
    {
        return handDetected;
    }

    /*****************************************************
    * DETECTED HAND
    *
    * INFO:    Permet à une classe d'utiliser virtuellement
    *          cette fonction pour modifier la réponse de
    *          la main détectée. Par défaut, on met la main
    *          détecté comme étant inconnu.
    *
    *****************************************************/
    public virtual HandsE DetectedHand()
    {
        return HandsE.inconnu;
    }
}


