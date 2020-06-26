using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public float speed = 40.0f;

    //Hand
    [Header("Move player with hand")]
    [Space(5)]
    [ConditionalHide("allowMove")]
    public HandsE hand = HandsE.gauche;
    [Space(10)]
    [ConditionalHide("allowMove")]
    public bool moveInDepth = true;
    [ConditionalHide("allowMove")]
    public bool moveHorizontal = true;
    [ConditionalHide("allowMove")]
    public bool moveVertical = false;
    [Space(10)]
    [ConditionalHide("allowMove")]
    public bool stayInRadius = false;
    
    //Keyboard
    [Header("Move player with keyboard")]
    [Space(5)]
    [ConditionalHide("allowMove")]
    public float maxSpeed = 5.0f;
    [ConditionalHide("allowMove")]
    public float dampingSpeed = 0.2f;
    [Space(5)]
    [ConditionalHide("allowMove")]
    public KeyCode fwdKey = KeyCode.W;
    [ConditionalHide("allowMove")]
    public KeyCode leftKey = KeyCode.A;
    [ConditionalHide("allowMove")]
    public KeyCode backKey = KeyCode.S;
    [ConditionalHide("allowMove")]
    public KeyCode rightKey = KeyCode.D;

    private float speedX, speedZ = 0;
    private Vector3 lastHandPosition;
    private Vector3 lastHandPosition2;

    private bool isHandSet = false;
    private bool isMoving = false;
    private bool isMovingPaused = true;
    private CharacterController controller;
 

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
    * GET RELATIVE HAND POSITION
    *
    * INFO:    Retourne la position relative de la main.
    *
    *****************************************************/
    Vector3 GetRelativeHandPosition()
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
    * GET HAND POSITION
    *
    * INFO:    Retourne la position de la main.
    *
    *****************************************************/
    Vector3 GetHandPosition()
    {
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            return DetectionController
                    .GetInstance()
                    .GetHand(hand)
                    .GetHandPosition();
        }
        return Vector3.zero;
    }

    /*****************************************************
    * GET HAND AXIS
    *
    * INFO:    Retourne l'axe (vecteur) que prend les doigts 
    *          de la main pour rendre le déplacement plus 
    *          dynamique.
    *          
    *****************************************************/
    public Vector3 GetHandAxis()
    {
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            return DetectionController
                .GetInstance()
                .GetHand(hand)
                .GetAxis(HandAxisE.doigt);
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
        lastHandPosition2 = GetHandPosition();
        lastHandPosition = GetRelativeHandPosition();
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
        lastHandPosition2 = GetHandPosition();
        lastHandPosition = GetRelativeHandPosition();
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
        lastHandPosition2 = GetHandPosition();
        lastHandPosition = GetRelativeHandPosition();
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
        if (IsReadyToMove())
        {
            //La position actuelle de la main           
            Vector3 actualPosition = GetRelativeHandPosition();
            Vector3 distance = actualPosition - lastHandPosition;
            float movingSpeed;

            //Pour le deplacement libre dans l'espace
            if (stayInRadius) { movingSpeed = speed; } 
            else { movingSpeed = speed * Time.deltaTime; }

            //Déplacement horizontal (X) activé
            if (moveHorizontal){ controller.Move(transform.right * distance.x * movingSpeed); }
            //Déplacement vertical (Y) activé
            if (moveVertical) { controller.Move(transform.up * distance.y * movingSpeed); }
            //Déplacement en profondeur (Z) activé
            if (moveInDepth) { controller.Move(transform.forward * distance.z * movingSpeed); }

            //Permet de maintient le deplacement dans un rayon
            if (stayInRadius) { lastHandPosition = actualPosition; }
        }
    }

    /*****************************************************
    * MOVE WITH PALM ANGLES
    *
    * INFO:    
    *
    *****************************************************/
    private void MoveWithPalmAngles()
    {
        if (IsReadyToMove())
        {
            Vector3 actualPosition = GetHandPosition();
            Vector3 distance = actualPosition - lastHandPosition2;

            //*** TODO ***
            //Replace transform.position with controller.Move()

            //Déplacement horizontal (X) et en profondeur (Z) activé
            transform.position += new Vector3(distance.x, 0, distance.z);   

            lastHandPosition2 = actualPosition;
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
        //Forward or backward
        if (Input.GetKey(fwdKey)) { speedX += speed * Time.deltaTime; }
        else if (Input.GetKey(backKey)) { speedX -= speed * Time.deltaTime; }

        //Left or right
        if (Input.GetKey(leftKey)) { speedZ -= speed * Time.deltaTime; }
        else if (Input.GetKey(rightKey)) { speedZ += speed * Time.deltaTime; }

        speedX = Mathf.Lerp(speedX, 0, dampingSpeed * Time.deltaTime);
        speedZ = Mathf.Lerp(speedZ, 0, dampingSpeed * Time.deltaTime);

        //Moving is controlled with a minimum and maximum speed
        speedX = Mathf.Clamp(speedX, -maxSpeed * Time.deltaTime, maxSpeed * Time.deltaTime);
        speedZ = Mathf.Clamp(speedZ, -maxSpeed * Time.deltaTime, maxSpeed * Time.deltaTime);

        //Deplacement 
        transform.position = transform.TransformPoint(new Vector3(speedZ, 0, speedX));
    }

    /*****************************************************
    * IS READY TO MOVE
    *
    * INFO:    Retourne vrai si l'utilisateur active
    *          le deplacement de la camera et que
    *          les contraintes sont respectées.
    *
    *****************************************************/
    private bool IsReadyToMove()
    {
        return isMoving && isHandSet && !isMovingPaused && controller != null &&
            DetectionController.GetInstance().IsHandDetected(hand);
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
