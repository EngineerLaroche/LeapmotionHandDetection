
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

    public LeapServiceProvider leapServiceController;
    //private FirstPersonController controller;

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
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        //controller = gameObject.GetComponent<FirstPersonController>();
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
    * INFO:    
    * 
    *****************************************************/
    private void LookWithFingersOrientation()
    {
        if (IsReadyToLook())
        {

            //L'axe (direction) actuel des doigts de la main
            Vector3 handAxis = GetHandAxis();
            //Debug.Log("handAxis:  " + handAxis.x + "   " + handAxis.y + "   " + handAxis.z);

           Vector3 deltaAxis = new Vector3(-handAxis.x, -handAxis.y);
           Debug.Log("deltaAxis:  " + deltaAxis.x + "   " + deltaAxis.y + "   " + deltaAxis.z);


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

            // Vertical
            Quaternion verticalRotation = Quaternion.AngleAxis(-moveAbsolute.y, Vector3.right);
            transform.localRotation = verticalRotation;

            // Horizontal
            Quaternion horizontalRotation = Quaternion.AngleAxis(moveAbsolute.x, Vector3.up);

            if (lookVertical) { transform.localRotation *= horizontalRotation; }
            else { transform.localRotation = horizontalRotation; }
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
    * TURN WITH YAW DIRECTION
    *
    * INFO:    Zone de test.
    *
    *****************************************************/
    private void TurnWithYawDirection()
    {
        if (IsReadyToLook())
        {
            Hand leapHand = new Hand { Id = -1 };
            if (leapServiceController.GetLeapController() == null) return;
            foreach (var item in leapServiceController.GetLeapController().Frame(0).Hands) // Get last frame
            {
                if (item.IsRight) leapHand = item;
            }
            if (leapHand.Id == -1) return;

            float yawDirection = leapHand.Direction.Yaw;

            /*float rotHorizontale;
            if (yawDirection > -0.2f && yawDirection < 1.2f) rotHorizontale = 0f;
            else rotHorizontale = yawDirection; */

            float rotHorizontale;
            if (yawDirection > 1) rotHorizontale = -0.8f;
            else if (yawDirection < 0) rotHorizontale = 0.8f;
            else rotHorizontale = 0;

            targetRot *= Quaternion.Euler(0f, rotHorizontale, 0f);
            transform.rotation = targetRot;
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
            /*Hand leapHand = new Hand { Id = -1 };
            if (DetectionController.GetInstance().GetLeapService().GetLeapController() == null) return;
            foreach (var item in DetectionController.GetInstance().GetLeapFrame().Hands) 
            {
                if (item.IsRight) leapHand = item;
            }
            if (leapHand.Id == -1) return;
   
            float yaw = leapHand.Direction.Yaw;
            float rotationY = 0f;

            if (yaw > 1) rotationY = -0.8f;
            else if (yaw < 0) rotationY = 0.8f;

            targetRot *= Quaternion.Euler(0f, rotationY, 0f);
            controller.transform.localRotation = targetRot;
            */
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
            if (lookType == LookE.YawDirection) { TurnWithYawDirection(); }
            if (lookType == LookE.Mouse) { LookWithMouse(); }
            if (lookType == LookE.Test) { TestLook(); }
        }
    }
}
