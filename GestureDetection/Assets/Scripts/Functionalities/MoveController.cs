﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public enum MoveE { HandPosition, PalmAngle, BothHands, Keyboard, None }

/*****************************************************
 * CLASS:   MOVE CONTROLLER
 *
 * INFO:    Permet au joueur de se déplacer en
 *          bougeant sa main sur l'axe X ou Y ou Z.
 *          Le déplacement de la main lui permet de 
 *          reculer/avancer ou de se deplacer vers la 
 *          gauche/droite ou de monter/descendre dans
 *          l'espace.
 * 
 *****************************************************/
public class MoveController : HandDataStructure
{
    [Header("Move settings")]
    public bool allowMove = true;
    public MoveE moveType = MoveE.HandPosition;
    [Range(0f, 100f)]
    public float speed = 50.0f;

    //Hand
    [Header("Move player with hand")]
    [Space(10)]
    [ConditionalHide("allowMove")]
    public HandsE hand = HandsE.gauche;
    [ConditionalHide("allowMove")]
    public bool stayInRadius = false;
    [Space(10)]
    [ConditionalHide("allowMove")]
    public bool moveInDepth = true;
    [ConditionalHide("allowMove")]
    public bool moveHorizontal = true;
    [ConditionalHide("allowMove")]
    public bool moveVertical = false;

    //Keyboard
    [Header("Move player with keyboard")]
    [Space(10)]
    [ConditionalHide("allowMove")]
    public float maxSpeed = 5.0f;
    [ConditionalHide("allowMove")]
    public float dampingSpeed = 0.2f;
    [Space(10)]
    [ConditionalHide("allowMove")]
    public KeyCode fwdKey = KeyCode.W;
    [ConditionalHide("allowMove")]
    public KeyCode leftKey = KeyCode.A;
    [ConditionalHide("allowMove")]
    public KeyCode backKey = KeyCode.S;
    [ConditionalHide("allowMove")]
    public KeyCode rightKey = KeyCode.D;

    private float speedX, speedZ = 0;
    private Vector3 startHandPosition;

    private bool isHandSet = false;
    private bool isMoving = false;
    private bool isMovingPaused = true;
    private CharacterController controller = null;

    /*****************************************************
    * START
    *
    * INFO:    Recupere le Character Controller pour 
    *          permettre les deplacements.
    *
    *****************************************************/
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    /*****************************************************
    * GET HAND POSITION
    *
    * INFO:    Retourne la position relative de la main.
    *
    *****************************************************/
    Vector3 GetHandPosition()
    {
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            return DetectionController
                    .GetInstance()
                    .GetHand(hand)
                    .GetRelativeHandPosition();
        }
        return Vector3.zero;
    }

    /*****************************************************
    * PREPARE TO MOVE
    *
    * INFO:    Prepare la fonctionnalité en initialisant
    *          une première position de la main afin de 
    *          garder une référence. 
    *
    *****************************************************/
    public void PrepareToMove()
    {
        startHandPosition = GetHandPosition();
        isHandSet = isHandSet ? false : true;
    }

    /*****************************************************
    * TOGGLE MOVING
    *
    * INFO:    Début ou arret du déplacement du joueur.
    *          Effectuer une fois le geste demarre la
    *          fonctionnalité et le refaire cesse la
    *          fonctionnalité.
    *
    *****************************************************/
    public void ToggleMoving()
    {
        isMoving = isMoving ? false : true;
    }

    /*****************************************************
    * PAUSE MOVING
    *
    * INFO:    Figer temporairement le deplacement.
    *
    *****************************************************/
    public void PauseMoving()
    {
        isMovingPaused = isMovingPaused ? false : true;
    }

    /*****************************************************
    * MOVE WITH HAND
    *
    * INFO:    Permet au joueur de se déplacer en
    *          bougeant sa main sur l'axe X ou Y ou Z.
    *          
    *          Si l'option stayInRadius est activé, le
    *          déplacement sera contrôlé pour rester 
    *          dans un rayon qui est celui de la zone 
    *          de détection du Leap Motion. Le point 
    *          entral est la potision initiale de la main 
    *          enregistrée au départ.
    *
    *****************************************************/
    private void MoveWithHandPosition()
    {
        if (isMoving && isHandSet && !isMovingPaused && controller != null && 
            DetectionController.GetInstance().IsHandDetected(hand))
        {
            //La position actuelle de la main
            Vector3 actualPosition = GetHandPosition();
            Vector3 distance = actualPosition - startHandPosition;
            float movingSpeed;

            //Pour le deplacement libre dans l'espace
            if (!stayInRadius) { movingSpeed = speed * Time.deltaTime; } 
            else { movingSpeed = speed; }

            //Déplacement horizontal (X) activé
            if (moveHorizontal){ controller.Move(transform.right * distance.x * movingSpeed); }
            //Déplacement vertical (Y) activé
            if (moveVertical) { controller.Move(transform.up * distance.y * movingSpeed); }
            //Déplacement en profondeur (Z) activé
            if (moveInDepth) { controller.Move(transform.forward * distance.z * movingSpeed); }

            //Permet de maintient le deplacement dans un rayon
            if (stayInRadius) { startHandPosition = actualPosition; }
        }
    }

    /*****************************************************
    * 
    *
    * INFO:    
    *
    *****************************************************/
    private void MoveWithPalmAngles()
    {
        if (isMoving && isHandSet && !isMovingPaused &&
            DetectionController.GetInstance().IsHandDetected(hand))
        {
            // TODO
        }
    }

    /*****************************************************
    * 
    *
    * INFO:    
    *
    *****************************************************/
    private void MoveWithBothHands()
    {
        if (isMoving && isHandSet && !isMovingPaused &&
            DetectionController.GetInstance().IsHandDetected(HandsE.gauche) &&
            DetectionController.GetInstance().IsHandDetected(HandsE.droite))
        {

        }
    }

    /*****************************************************
    * MOVE WITH KEYBOARD
    *
    * INFO:    Permet au joueur de se déplacer avec les
    *          touches du clavier.
    *
    *****************************************************/
    private void MoveWithKeyboard()
    {
        //Left or right
        if (Input.GetKey(rightKey)) { speedX += speed * Time.deltaTime; }
        else if (Input.GetKey(leftKey)) { speedX -= speed * Time.deltaTime; }

        //Forward or backward
        if (Input.GetKey(backKey)) { speedZ -= speed * Time.deltaTime; }
        else if (Input.GetKey(fwdKey)) { speedZ += speed * Time.deltaTime; }

        speedX = Mathf.Lerp(speedX, 0, dampingSpeed * Time.deltaTime);
        speedZ = Mathf.Lerp(speedZ, 0, dampingSpeed * Time.deltaTime);

        //Moving is controlled with a minimum and maximum speed
        speedX = Mathf.Clamp(speedX, -maxSpeed * Time.deltaTime, maxSpeed * Time.deltaTime);
        speedZ = Mathf.Clamp(speedZ, -maxSpeed * Time.deltaTime, maxSpeed * Time.deltaTime);

        //Deplacement 
        transform.position = transform.TransformPoint(new Vector3(speedZ, 0, speedX));
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Met a jour le deplacement du joueur en
    *          utilisant la main ou en utilisant le clavier.
    *          
    *          L'utilisateur doit choisir un mode de déplacement.
    *          Par defaut c'est avec la main.
    *
    *****************************************************/
    void FixedUpdate()
    {
        if (allowMove)
        {
            if (moveType == MoveE.HandPosition) { MoveWithHandPosition(); }
            if (moveType == MoveE.PalmAngle) { MoveWithPalmAngles(); }
            if (moveType == MoveE.BothHands) { MoveWithBothHands(); }
            if (moveType == MoveE.Keyboard) { MoveWithKeyboard(); }
        }
    }
}