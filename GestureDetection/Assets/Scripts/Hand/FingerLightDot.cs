using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FingerLightDot : HandDataStructure
{
    //Doigt de la main qui permet d'orienter le point de lumiere
    public HandsE hand = HandsE.droite;
    public FingersE finger = FingersE.index;

    //public Vector3 rotAngle = new Vector3(-12, 90f, 0f);
    private new Light light = null;

    //Le controleur de la main et du doigt
    private DetectionController.HandController handController;
    private DetectionController.FingerController fingerController;

    // Start is called before the first frame update
    void Awake()
    {
        light = gameObject.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            handController = DetectionController.GetInstance().GetHand(hand);

            //Si le doigt du raycast est le seul d'ouvert et le Palm UI n'est pas affiché
            if (handController.IsIndexOpened()) //&& !PalmUIController.GetInstance().IsPalmUIOpen()
            {
                //La position du bout du foigt et la direction auquelle il pointe
                fingerController = handController.GetFinger(finger);
                light.transform.position = fingerController.GetFingertipPosition();
                light.transform.rotation = Quaternion.LookRotation(fingerController.GetFingerDirection());
            }
        }
    }
}
