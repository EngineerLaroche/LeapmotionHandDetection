using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;


/*****************************************************
 * CLASS:   GESTURE CONTROLLER
 *
 * INFO:    Permet de gérer les événements des différents 
 *          gestes de la main à détecter.
 * 
 *****************************************************/
public class GestureController : MonoBehaviour
{
    //Liste de gestes à détecter
    private GestureMediator[] gestures = null;

    //Événements (fonctionnalités) qui contrôle l'action des gestes
    public UnityEvent startGesture;
    public UnityEvent stopGesture;
    public UnityEvent keepGesture;

    //Valide la détection d'un geste
    private bool detectedGesture = false;


    /*****************************************************
    * AWAKE
    *
    * INFO:    
    *
    *****************************************************/
    void Awake()
    {
        //Liste des gestes détectés
        gestures = gameObject.GetComponents<GestureMediator>();
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Déclenche des événements selon la détection
    *          de gestes pour démarrer, arrêter ou maintenir
    *          une fonctionalité quelconque.
    *
    *****************************************************/
    void Update()
    {
        //Si le Palm UI n'est pas ouvert
        if (!PalmUIController.GetInstance().IsPalmUIOpen())
        {
            //Quantité de gestes détectés
            int gesturesQty = 0;

            //Pour tous les gestes détectés
            foreach (GestureMediator gesture in gestures)
            {
                if (gesture.IsGestureDetected()) { gesturesQty++; }
            }

            //Si le systeme détecte au moin un geste configuré
            if (gesturesQty >= gestures.Length)
            {
                //Si aucun geste actif
                if (!detectedGesture)
                {
                    //Démarre la fonctionnalité du geste
                    startGesture.Invoke();
                    detectedGesture = true;
                }
                //Maintient la fonctionnalité du geste
                keepGesture.Invoke();
            }
            else
            {
                //Si le geste est encore perçu comme actif
                if (detectedGesture)
                {
                    //Arret de la fonctionnalité du geste
                    stopGesture.Invoke();
                    detectedGesture = false;
                }
            }
        }
    }
}
