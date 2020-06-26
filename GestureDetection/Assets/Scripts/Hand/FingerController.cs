using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

partial class DetectionController
{

    /*****************************************************
     * CLASS:   FINGER CONTROLLER
     *
     * INFO:    Permet de contrôler un doigt et de gérer
     *          la détection de celui-ci.
     * 
     *****************************************************/
    public class FingerController
    {
        // Instance d'un doigt virtuel (API LeapMotion)
        private Finger leapFinger;
        private Finger.FingerType fingerType;

       /*****************************************************
        * GET FINGERTIP POSITION
        *
        * INFO:   Retourne la position du bout du doigt.
        *         (API LeapMotion)
        * 
        *****************************************************/
        public Vector3 GetFingertipPosition()
        {
            return leapFinger.TipPosition.ToVector3();
        }

        /*****************************************************
        * GET FINGER DIRECTION
        *
        * INFO:   Retourne le sens que pointe un doigt.
        *         (API LeapMotion)
        * 
        *****************************************************/
        public Vector3 GetFingerDirection()
        {
            return leapFinger.Direction.ToVector3();
        }

        /*****************************************************
        * SET FINGER
        *
        * INFO:   Initialise le doigt et son type au contrôleur.
        * 
        *****************************************************/
        public void SetFinger(Finger _finger)
        {
            leapFinger = _finger;
            fingerType = _finger.Type;         
        }

        /*****************************************************
        * GET FINGER TYPE
        *
        * INFO:   Retourne le type du doigt actuel.
        * 
        *****************************************************/
        public FingersE GetFingerType()
        {
            return (FingersE)((int) fingerType);
        }

        /*****************************************************
        * IS FINGER OPEN
        *
        * INFO:   Valide que le doigt de la main est sortie.
        *         (API LeapMotion)
        * 
        *****************************************************/
        public bool IsFingerOpen()
        {
            return leapFinger.IsExtended;
        }

        /*****************************************************
        * GET LEAP FINGER
        *
        * INFO:   Retourne le doigt Leap
        * 
        *****************************************************/
        public Finger GetLeapFinger()
        {
            return leapFinger;
        }

        /*****************************************************
        * GET FINGER RAY
        *
        * INFO:   Retourne une ligne a partir du bout du doigt
        *         dans le sens que pointe le doigt.
        * 
        *****************************************************/
        public Ray GetFingerRay()
        {
            return new Ray(GetFingertipPosition(), GetFingerDirection());
        }
    }
}
