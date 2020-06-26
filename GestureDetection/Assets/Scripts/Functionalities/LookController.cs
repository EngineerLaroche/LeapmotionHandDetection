
using UnityEngine;
using Leap;
using Leap.Unity;


/*****************************************************
 * CLASS:   LOOK CONTROLLER
 *
 * INFO:    Permet au joueur de se changer l'angle de
 *          la caméra en bougeant sa main sur l'axe X 
 *          ou Y ou Z. Le déplacement de la main lui 
 *          permet de regarder en haut/bas ou à
 *          gauche/droite. Permet aussi d'orienter la
 *          direction du déplacement lorsque l'utilisateur
 *          bouge dans l'espace.
 * 
 *****************************************************/
public class LookController : HandDataStructure
{
    [Header("Rotate settings")]
    public bool canLookAround = true;
    public LookE lookType = LookE.FingersPointing;
    [Space(10)]
    [ConditionalHide("allowLook")]
    public Vector2 clampInDegrees = new Vector2(360, 180);
    [ConditionalHide("allowLook")]
    public Vector2 sensitivity = new Vector2(2, 2);
    [ConditionalHide("allowLook")]
    public Vector2 smoothing = new Vector2(3, 3);
    [ConditionalHide("allowLook")]
    public Vector2 targetDirection;

    //Hand
    [Header("Rotate view with hand")]
    [Space(10)]
    [ConditionalHide("allowLook")]
    public HandsE hand = HandsE.droite;
    [ConditionalHide("allowLook")]
    public bool lookHorizontal = true;
    [ConditionalHide("allowLook")]
    public bool lookVertical = false;

    //Mouse
    [Header("Rotate view with mouse")]
    [Space(10)]
    [ConditionalHide("allowLook")]
    public bool isHeadMounted = false;
    [ConditionalHide("allowLook")]
    public CursorLockMode lockCursor;
    [ConditionalHide("allowLook")]
    public Vector2 targetCharacterDirection;


    //Position initiale
    private Vector3 startHandPosition;

    //Parametres de rotation
    private Vector3 moveAbsolute;
    private Vector3 smoothRotation;

    //Contrôle d'activation de la fonctionnalité
    private bool isHandSet = false;
    private bool isLooking = false;
    private bool isLookingPaused = true;
    private Quaternion targetRot;


    /*****************************************************
    * AWAKE
    *
    * INFO:    Instance de la classe et parametres de
    *          rotation.
    *
    *****************************************************/
    void Start()
    {
        // Set target direction to the hands initial euler orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target rotation of the hands.
        targetRot = transform.rotation;

        // Set target direction for the character body to its inital state.
        if (isHeadMounted) { targetCharacterDirection = transform.localRotation.eulerAngles; }
    }

    /*****************************************************
    * GET HAND POSITION
    *
    * INFO:    Retourne la position actuelle de la main.
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
    public Vector3 GetHandAxis(HandAxisE axis)
    {
        if (DetectionController.GetInstance().IsHandDetected(hand))
        {
            return DetectionController
                .GetInstance()
                .GetHand(hand)
                .GetAxis(axis);
        }
        return Vector3.zero;
    }

    /*****************************************************
    * PREPARE TO LOOK
    *
    * INFO:    Prepare la fonctionnalité en initialisant
    *          une première position de la main afin de 
    *          garder une référence. 
    *
    *****************************************************/
    public void PrepareToLook()
    {
        startHandPosition = GetHandPosition();
        isHandSet = isHandSet ? false : true;
    }

    /*****************************************************
    * TOGGLE LOOKING
    *
    * INFO:    Début ou arret de la rotation (vue) du joueur.
    *          Effectuer une fois le geste démarre la
    *          fonctionnalité et le refaire cesse la
    *          fonctionnalité.
    *
    *****************************************************/
    public void ToggleLooking()
    {
        isLooking = isLooking ? false : true;
    }

    /*****************************************************
    * PAUSE LOOKING
    *
    * INFO:    Figer temporairement la vue.
    *
    *****************************************************/
    public void PauseLooking()
    {
        isLookingPaused = isLookingPaused ? false : true;
    }

    /*****************************************************
    * TURN WITH FINGERS ORIENTATION
    *
    * INFO:    Change la direction de la vue (rotation) en
    *          fonction de la direction que pointe les doigts.
    *          Rotation 'smooth' entre 0 et 360 degrés.
    * 
    *****************************************************/
    private void LookWithFingersOrientation()
    {
        if (IsReadyToLook())
        {
            //L'axe (direction) actuel des doigts de la main
            Vector3 axisFingers = GetHandAxis(HandAxisE.doigt);
            
            float rotHorizontale = 0f, rotVerticale = 0f;

            if (lookHorizontal)
            {
                if (axisFingers.x > 0.3f || axisFingers.x < -0.5f) rotHorizontale = axisFingers.x;
            }
            if (lookVertical)
            {
                if (axisFingers.y > 0.3f || axisFingers.y < -0.5f) rotVerticale = axisFingers.y;
            }

            //Applique la rotation horizontale
            targetRot *= Quaternion.Euler(rotVerticale, rotHorizontale, 0f);
            transform.rotation = targetRot;
        }
    }

    /*****************************************************
    * TURN WITH HAND
    *
    * INFO:    Initialise et maintient à jour la rotation
    *          de la caméra en fonction du déplacement de
    *          la main. Permet aussi d'orienter les
    *          déplacements du joueur.
    *
    *****************************************************/
    private void LookWithHandPosition()
    {
        if (IsReadyToLook())
        {
            Vector3 distance = GetHandPosition() - startHandPosition;

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            Vector3 handDelta = Vector3.Scale(distance, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            //Si la vue horizontale est activé
            if (lookHorizontal)
            {
                // Interpolate hand movement over time to apply smoothing delta.
                handDelta.x = -(distance.y - 0.1f) * 100.0f;
                smoothRotation.x = Mathf.Lerp(smoothRotation.x, handDelta.x, 1f / smoothing.x);

                //Controling min and max rotation X angle
                if (clampInDegrees.x < 360)
                    smoothRotation.x = Mathf.Clamp(smoothRotation.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

                // Apply rotation (X) on main object
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(smoothRotation.x, 0f, 0f), 0.1f);
            }
            //Si la vue verticale est activé
            if (lookVertical)
            {
                // Interpolate hand movement over time to apply smoothing delta.
                handDelta.y = (distance.x - 0.1f) * 100.0f;
                smoothRotation.y = Mathf.Lerp(smoothRotation.y, handDelta.y, 1f / smoothing.y);

                //Controling min and max rotation Y angle
                if (clampInDegrees.y < 360)
                    smoothRotation.y = Mathf.Clamp(smoothRotation.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

                // Apply rotation (Y) on main object
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, smoothRotation.y, 0f), 0.1f);
            }
        }
    }

    /*****************************************************
    * TURN WITH MOUSE
    *
    * INFO:    Initialise et maintient à jour la rotation
    *          de la caméra en fonction du déplacement de
    *          la souris. Permet aussi d'orienter les
    *          déplacements du joueur.
    *
    *****************************************************/
    private void LookWithMouse()
    {
        // Ensure the cursor is always locked when set
        Cursor.lockState = lockCursor;

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        //Debug.Log("mouseDelta:  " + mouseDelta.x);

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        smoothRotation.x = Mathf.Lerp(smoothRotation.x, mouseDelta.x, 1f / smoothing.x);
        smoothRotation.y = Mathf.Lerp(smoothRotation.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        moveAbsolute += smoothRotation;

        // Allow the script to clamp based on a desired target value.
        Quaternion targetOrientation = Quaternion.Euler(targetDirection);

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            moveAbsolute.x = Mathf.Clamp(moveAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        //VERTICAL
        transform.localRotation = Quaternion.AngleAxis(-moveAbsolute.y, targetOrientation * Vector3.right);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            moveAbsolute.y = Mathf.Clamp(moveAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        transform.localRotation *= targetOrientation;

        // If there's a character body that acts as a parent to the camera
        if (isHeadMounted)
        {
            transform.localRotation = Quaternion.AngleAxis(moveAbsolute.x, transform.up);
            transform.localRotation *= Quaternion.Euler(targetCharacterDirection);
        }
        else
        {   //HORIZONTAL
            transform.localRotation *= Quaternion.AngleAxis(moveAbsolute.x, transform.InverseTransformDirection(Vector3.up));
        }
    }

    /*****************************************************
    * TEST LOOK
    *
    * INFO:    Zone de test.
    *
    *****************************************************/
    private void TestLook()
    {
        if (IsReadyToLook())
        {
            /*
            //L'axe (direction) actuel des doigts de la main
            Vector3 axisFingers = GetHandAxis(HandAxisE.doigt);
            Vector3 deltaAxis = new Vector3(-handAxis.x, -handAxis.y, 0f);

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            deltaAxis = Vector3.Scale(deltaAxis, new Vector3(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            smoothRotation.x = Mathf.Lerp(smoothRotation.x, deltaAxis.x, 1f / smoothing.x);
            smoothRotation.y = Mathf.Lerp(smoothRotation.y, deltaAxis.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            moveAbsolute += smoothRotation;

            // Contrôle de l'angle min et max
            if (clampInDegrees.x < 360) moveAbsolute.x = Mathf.Clamp(moveAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
            if (clampInDegrees.y < 360) moveAbsolute.y = Mathf.Clamp(moveAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

            // Rotation Verticale
            Quaternion verticalRotation = Quaternion.AngleAxis(-moveAbsolute.y, Vector3.right);
            transform.localRotation = verticalRotation;

            // Rotation Horizontale
            Quaternion horizontalRotation = Quaternion.AngleAxis(moveAbsolute.x, Vector3.up);

            if (lookVertical) { transform.localRotation *= horizontalRotation; }
            else { transform.localRotation = horizontalRotation; } */
        }
    }

    /*****************************************************
    * IS READY TO LOOK
    *
    * INFO:    Retourne vrai si l'utilisateur active
    *          la rotation de la camera (vue) et que
    *          les contraintes sont respectées.
    *
    *****************************************************/
    private bool IsReadyToLook()
    {
        return isLooking && isHandSet && !isLookingPaused &&
            DetectionController.GetInstance().IsHandDetected(hand);
    }


    /*****************************************************
    * UPDATE
    *
    * INFO:    Met a jour la rotation de la caméra du joueur
    *          en utilisant la main ou en utilisant la souris.
    *          
    *          L'utilisateur doit choisir un mode d'orientation.
    *          Par defaut c'est avec les angles de la paume de main.
    *
    *****************************************************/
    void Update()
    {
        if (canLookAround)
        {
            if (lookType == LookE.FingersPointing) { LookWithFingersOrientation(); }
            if (lookType == LookE.HandPos) { LookWithHandPosition(); }
            if (lookType == LookE.Mouse) { LookWithMouse(); }
            if (lookType == LookE.Test) { TestLook(); }
        }
    }
}
