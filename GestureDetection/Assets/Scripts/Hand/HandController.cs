using UnityEngine;
using Leap.Unity;
using Leap;
using System;


partial class DetectionController
{

    /*****************************************************
     * CLASS:   HAND CONTROLLER
     *
     * INFO:    Permet de contrôler une main et de gérer
     *          la détection de celle-ci.
     * 
     *****************************************************/
    public class HandController : DetectionController
    {
        // Instance d'une main (API LeapMotion)
        private Hand leapHand;

        // Les 5 doigts de la main
        private FingerController[] fingers = new FingerController[5];


        /*****************************************************
        * GET HAND POSITION
        *
        * INFO:    Retourne la position (X,Y,Z) de la main.
        * 
        *****************************************************/
        public Vector3 GetHandPosition()
        {
            return leapHand.PalmPosition.ToVector3();
        }

        /*****************************************************
        * GET HAND QUATERNION
        *
        * INFO:    Retourne le quaternion (X,Y,Z) de la main.
        * 
        *****************************************************/
        public Quaternion GetHandQuaternion()
        {
            return leapHand.Rotation.ToQuaternion();
        }

        /*****************************************************
        * GET RELATIVE HAND POSITION
        *
        * INFO:    Retourne la position relative de la main
        *          en fonction du controleur LeapMotion.
        * 
        *****************************************************/
        public Vector3 GetRelativeHandPosition()
        {
            Vector3 relativePosition = GetHandPosition() - GetInstance().GetLeapService().transform.position;
            return relativePosition;
        }

        /*****************************************************
        * GET RELATIVE QUATERNION
        *
        * INFO:    Retourne le quaternion relatif de la main
        *          en fonction du controleur LeapMotion.
        *          
        *          *** INCERTAIN ***
        * 
        *****************************************************/
        public Quaternion GetRelativeHandQuaternion()
        {
            Quaternion relativeQuaternion = GetHandQuaternion() * GetInstance().GetLeapService().transform.rotation;
            return relativeQuaternion;
        }

        /*****************************************************
        * IS FIST
        *
        * INFO:    Valide le geste de fermeture de la main en 
        *          fonction de la tolérance (%) entrée en 
        *          paramètre et retourne la réponse.
        * 
        *****************************************************/
        public bool IsFist(float _tolerence)
        {
            return leapHand.GetFistStrength() > _tolerence;
        }

        /*****************************************************
        * IS HAND SET
        *
        * INFO:    Vérifie si la main existe, qu'il n'y ait pas
        *          de détection fautive de doigts (inconnu) et 
        *          retourne une réponse.
        * 
        *****************************************************/
        public bool IsHandSet()
        {
            foreach (FingerController finger in fingers)
            {
                if (finger != null)
                {
                    //Verifie si le doigt détecté existe bel et bien
                    if (finger.GetFingerType() == FingersE.inconnu) { return false; }
                }
            }
            return leapHand != null;
        }

        /*****************************************************
        * SET HAND
        *
        * INFO:    Initialise et associe chaque doigt de la main 
        *          reçue en paramètre à son contrôleur.
        * 
        *****************************************************/
        public void SetHand(Hand _hand)
        {
            leapHand = _hand;
            foreach (Finger finger in leapHand.Fingers)
            {
                fingers[(int)finger.Type] = new FingerController();
                fingers[(int)finger.Type].SetFinger(finger);
            }
        }

        /*****************************************************
        * GET FINGER
        *
        * INFO:    Valide et retourne un des doigts de la main.   
        * 
        *****************************************************/
        public FingerController GetFinger(FingersE _fingerType)
        {
            return fingers[(int)_fingerType];
        }

        /*****************************************************
        * IS OTHER FINGER CLOSED
        *
        * INFO:    Retourne vrai si le doigt passé en parametre
        *          est ouvert et que les autres sont fermés.
        * 
        *****************************************************/
        public bool IsOnlyThisFingerOpened(FingersE _finger)
        {
            int fingerID = (int)_finger;
            bool onlyThisFingerOpened = false;
            foreach (Finger finger in leapHand.Fingers)
            {
                if ((int)finger.Type == fingerID) { onlyThisFingerOpened = finger.IsExtended; }
                else { if (finger.IsExtended) { onlyThisFingerOpened = false; } }
            }
            return onlyThisFingerOpened;
        }

        /*****************************************************
        * GET HAND AXIS
        *
        * INFO:    Retourne l'axe de deplacement d'un des 
        *          composants de la main.
        * 
        *****************************************************/
        public Vector3 GetHandAxis(HandAxisE _axis)
        {
            switch (_axis)
            {
                case HandAxisE.paume: return leapHand.PalmarAxis();
                case HandAxisE.pouce: return leapHand.RadialAxis();
                case HandAxisE.doigt: return leapHand.DistalAxis();
                default: break;
            }
            return Vector3.zero;
        }

        /*****************************************************
        * IS HAND PINCHING WITH TOLERENCE
        *
        * INFO:    Valide si la main effectue un pincement selon
        *          une tolérance (%) entrée en paramètre et retourne 
        *          une reponse. 
        * 
        *****************************************************/
        public bool IsHandPinching(float _tolerence)
        {
            return leapHand.PinchStrength > _tolerence;
        }

        /*****************************************************
        * IS HAND PINCH DISTANCE
        *
        * INFO:    Retourne vrai si le pouce et l'index effectuent
        *          un pincement acceptable.
        * 
        *****************************************************/
        public bool IsPinchDistance(float _tolerence)
        {
            return leapHand.PinchDistance < _tolerence;
        }

        /*****************************************************
        * GET HAND PINCH DISTANCE
        *
        * INFO:    Retourne la distance entre le pouce et l'index.
        * 
        *****************************************************/
        public float GetPinchDistance()
        {
            return leapHand.PinchDistance;
        }

        /*****************************************************
        * GET HAND VELOCITY
        *
        * INFO:    Retourne la vélocité de la main (swipe).
        *          En d'autres mots, retourne la direction de
        *          la paume de la main.  
        * 
        *****************************************************/
        public Vector3 GetHandVelocity()
        {
            return leapHand.PalmVelocity.ToVector3();
        }

        /*****************************************************
        * GET PINCH STRENGTH
        *
        * INFO:    Retourne la force de pincement de la main. 
        *          Le pincement consiste au contact de l'index
        *          et le majeur de la main.
        * 
        *****************************************************/
        public float GetPinchStrength()
        {
            return leapHand.PinchStrength;
        }

        /*****************************************************
        * GET FIST STRENGTH
        *
        * INFO:    Retourne la force du poing de la main. 
        * 
        *****************************************************/
        public float GetFistStrenght()
        {
            return leapHand.GetFistStrength();
        }

        /*****************************************************
        * GET GRAB STRENGTH
        *
        * INFO:    Retourne la force de la prise de main. 
        *          Si la main equivaut a 0, c'est qu'aucuns 
        *          doigts de la main n'est ouverts.
        * 
        *****************************************************/
        public float GetGrabStrenght()
        {
            return leapHand.GrabStrength;
        }

        /*****************************************************
        * GET FINGERS COUNT
        *
        * INFO:    Retourne le nombre de doigts détecté par 
        *          LeapMotion pour cette main.
        * 
        *****************************************************/
        public int GetFingersCount()
        {
            return leapHand.Fingers.Count;
        }

        /*****************************************************
        * GET LEAP HAND
        *
        * INFO:    Retourne la main Leap Motion du controleur.
        * 
        *****************************************************/
        public Hand GetLeapHand()
        {
            return leapHand;
        }

        /*****************************************************
        * GET FINGERS
        *
        * INFO:    Retourne la liste des doigts de la main.
        * 
        *****************************************************/
        public FingerController[] GetFingers()
        {
            return fingers;
        }
    }
}
