using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Leap.Unity;
using TMPro;
using Leap;
using System;

/*****************************************************
* CLASS:    SYSTEM UI CONTROLLER
*
* INFO:    Permet a l'utilisateur d'afficher ou cacher
*          le UI en appuyant sur la touche D.
*          
*          Principalement, la classe permet de maintenir
*          à jour les objets du UI qui affiche de 
*          l'information sur la détection des gestes et 
*          la performance du système en temps réel.
*
*****************************************************/
public class SystemUIController : HandDataStructure
{
    //Instance de la classe
    private static SystemUIController instance = null;

    // Pour afficher ou non le UI
    public bool isDisplayed = false;

    //Press this key to display or hide system UI
    public KeyCode keyToggleUI = KeyCode.M;

    //Objet qui contient tous les elements du UI (gauche, droite et haut) 
    [Header("Displacement UI objects")]
    public GameObject leftObjectsUI;
    public GameObject rightObjectsUI;
    public GameObject topObjectsUI;

    [Header("System labels")]
    public TextMeshProUGUI fpsLabel;
    public TextMeshProUGUI gestureLabel;

    [Header("Fist sliders")]
    public Slider leftFistSlider;
    public Slider rightFistSlider;

    [Header("Pinch sliders")]
    public Slider leftPinchSlider;
    public Slider rightPinchSlider;

    [Header("Palm Hands")]
    public UnityEngine.UI.Image L_Hand;
    public UnityEngine.UI.Image R_Hand;

    [Header("Left Fingertip")]
    public UnityEngine.UI.Image L_Thumb;
    public UnityEngine.UI.Image L_Index;
    public UnityEngine.UI.Image L_Majeur;
    public UnityEngine.UI.Image L_Annulaire;
    public UnityEngine.UI.Image L_Auriculaire;

    // Listes des images de la main gauche
    private UnityEngine.UI.Image[] leftHandImages;

    [Header("Right Fingertip")]
    public UnityEngine.UI.Image R_Thumb;
    public UnityEngine.UI.Image R_Index;
    public UnityEngine.UI.Image R_Majeur;
    public UnityEngine.UI.Image R_Annulaire;
    public UnityEngine.UI.Image R_Auriculaire;

    // Listes des images de la main droite
    private UnityEngine.UI.Image[] rightHandImages;

    [Header("Swipe left hand sliders")]
    public Slider swipeRightSliderL;
    public Slider swipeLeftSliderL;
    public Slider swipeUpSliderL;
    public Slider swipeDownSliderL;

    // Listes des sliders 'swipe' de la main gauche
    private Slider[] leftHandSwipeSliders;

    [Header("Swipe right hand sliders")]
    public Slider swipeRightSliderR;
    public Slider swipeLeftSliderR;
    public Slider swipeUpSliderR;
    public Slider swipeDownSliderR;

    // Listes des sliders 'swipe' de la main droie
    private Slider[] rightHandSwipeSliders;

    [Header("Clap hand sliders")]
    public Slider clapLeftHand;
    public Slider clapRightHand;

    [Header("Object coord. labels")]
    public GameObject headMounted;
    public TextMeshProUGUI posX;
    public TextMeshProUGUI posY;
    public TextMeshProUGUI posZ;
    public TextMeshProUGUI rotX;
    public TextMeshProUGUI rotY;
    public TextMeshProUGUI rotZ;
    public TextMeshProUGUI scaleX;
    public TextMeshProUGUI scaleY;
    public TextMeshProUGUI scaleZ;

    //Affiche la distance du raycast
    [Header("Raycast Distance label")]
    public TextMeshProUGUI raycastLabel;

    // *** Tolerance du Fist & Pinch temporairement entrée manuellement ***
    private readonly float tolerance = 0.80f;

    // Instance Leap
    private LeapProvider leapProvider;
    private SmoothedFloat smooth = new SmoothedFloat();

    //Display liste of detected gestures
    private float displayedTime = 0f;
    private List<GestureData> detectedGestureList = new List<GestureData>();


    /*****************************************************
    * AWAKE
    *
    * INFO:    Recupere l'instance du Leap et désactive
    *          les bout de doigt de la main gauche et droite
    *          affiché au UI.
    *
    *****************************************************/
    void Awake()
    {
        //Initialise l'instance de cette classe
        if (instance == null) { instance = this; }

        //Affiche ou non le UI dès le depart
        leftObjectsUI.SetActive(isDisplayed);
        rightObjectsUI.SetActive(isDisplayed);
        topObjectsUI.SetActive(isDisplayed);

        // Instance Leap pour afficher le FPS en temps reel
        if (leapProvider == null) { leapProvider = Hands.Provider; }
        smooth.delay = 0.3f;
        smooth.reset = true;

        // Listes des sliders (swipe) de la main gauche et droite pour faciliter la gestion
        leftHandSwipeSliders = new Slider[] { swipeLeftSliderL, swipeRightSliderL, swipeUpSliderL, swipeDownSliderL };
        rightHandSwipeSliders = new Slider[] { swipeLeftSliderR, swipeRightSliderR, swipeUpSliderR, swipeDownSliderR };

        // Listes des images de la main gauche et droite pour faciliter la gestion
        leftHandImages = new UnityEngine.UI.Image[] { L_Hand, L_Thumb, L_Index, L_Majeur, L_Annulaire, L_Auriculaire };
        rightHandImages = new UnityEngine.UI.Image[] { R_Hand, R_Thumb, R_Index, R_Majeur, R_Annulaire, R_Auriculaire };
    }

    /*****************************************************
    * UPDATE FPS LABEL
    *
    * INFO:    Maintient a jour le label pour afficher
    *          en temps reel l'information du FPS.
    *
    *****************************************************/
    private void UpdateFPSLabel()
    {
        fpsLabel.text = "";
        if (leapProvider != null)
        {
            Leap.Frame frame = leapProvider.CurrentFrame;
            if (frame != null)
            {
                fpsLabel.text += "Data FPS:       " + leapProvider.CurrentFrame.CurrentFramesPerSecond.ToString("f2");
                fpsLabel.text += System.Environment.NewLine;
            }
        }
        if (Time.smoothDeltaTime > Mathf.Epsilon) { smooth.Update(1.0f / Time.smoothDeltaTime, Time.deltaTime); }
        fpsLabel.text += "Render FPS:   " + Mathf.RoundToInt(smooth.value).ToString("f2");
    }

    /*****************************************************
     * UPDATE FIST AND PINCH SLIDERS
     *
     * INFO:    Affiche en temps réel le niveau de détection 
     *          des gestes Fist et Pinch pour les deux mains.
     *          
     *          *** PROBLEM *** 
     *          Les sliders de la main gauche fonctionnent
     *          seulement si la main droite est détectée.
     *
     *****************************************************/
    private void UpdateFistPinchSliders()
    {
        // Main gauche détectée
        if (DetectionController.GetInstance().IsHandDetected(HandsE.gauche))
        {
            // Recupere la force du Fist et du Pinch pour la main gauche
            Hand leftHand = DetectionController.GetInstance().GetHand(HandsE.gauche).GetLeapHand();
            leftFistSlider.value = leftHand.GetFistStrength();
            leftPinchSlider.value = -leftHand.PinchDistance;

            leftFistSlider.image.color = leftFistSlider.value >= tolerance ? Color.green : Color.gray;

            if (leftHand.GetFistStrength() > tolerance)
            {
                leftPinchSlider.value = -100;
                leftPinchSlider.image.color = Color.gray;
            }
            else { leftPinchSlider.image.color = leftPinchSlider.value >= -20f ? Color.green : Color.gray; }
        }

        // Main droite détectée
        if (DetectionController.GetInstance().IsHandDetected(HandsE.droite))
        {
            // Recupere la force du Fist et du Pinch pour la main droite
            Hand rightHand = DetectionController.GetInstance().GetHand(HandsE.droite).GetLeapHand();
            rightFistSlider.value = rightHand.GetFistStrength();
            rightPinchSlider.value = -rightHand.PinchDistance;

            rightFistSlider.image.color = rightFistSlider.value >= tolerance ? Color.green : Color.gray;

            if (rightHand.GetFistStrength() > tolerance)
            {
                rightPinchSlider.value = -100;
                rightPinchSlider.image.color = Color.gray;
            }
            else { rightPinchSlider.image.color = rightPinchSlider.value >= -20f ? Color.green : Color.gray; }
        }
    }

    /*****************************************************
     * UPDATE HAND & FINGERTIP IMAGE
     *
     * INFO:    Affiche en temps réel la detection de la
     *          main gauche et droite.
     *          Affiche les doigts qui sont détectés par
     *          le LeapMotion pour la main gauche et droite.
     *          
     *****************************************************/
    private void UpdateHandFingertipImage()
    {
        // Main gauche
        if (DetectionController.GetInstance().IsHandDetected(HandsE.gauche))
        {
            // Controleur de la main gauche
            Hand leftHand = DetectionController.GetInstance().GetHand(HandsE.gauche).GetLeapHand();

            //Un point vert (image) est affiché si le doigt de la main gauche est détecté
            L_Hand.enabled = true;
            L_Thumb.enabled = leftHand.GetThumb().IsExtended ? true : false;
            L_Index.enabled = leftHand.GetIndex().IsExtended ? true : false;
            L_Majeur.enabled = leftHand.GetMiddle().IsExtended ? true : false;
            L_Annulaire.enabled = leftHand.GetRing().IsExtended ? true : false;
            L_Auriculaire.enabled = leftHand.GetPinky().IsExtended ? true : false;
        }

        // Main droite
        if (DetectionController.GetInstance().IsHandDetected(HandsE.droite))
        {
            // Controleur de la main droite
            Hand rightHand = DetectionController.GetInstance().GetHand(HandsE.droite).GetLeapHand();

            //Un point vert (image) est affiché si le doigt de la main droite est détecté
            R_Hand.enabled = true;
            R_Thumb.enabled = rightHand.GetThumb().IsExtended ? true : false;
            R_Index.enabled = rightHand.GetIndex().IsExtended ? true : false;
            R_Majeur.enabled = rightHand.GetMiddle().IsExtended ? true : false;
            R_Annulaire.enabled = rightHand.GetRing().IsExtended ? true : false;
            R_Auriculaire.enabled = rightHand.GetPinky().IsExtended ? true : false;
        }
    }

    /*****************************************************
     * UPDATE SWIPE SLIDERS
     *
     * INFO:    Permet de demarrer le Timer du slider pour 
     *          le type de geste swipe détecté.
     *          
     *          *** TO OPTIMIZE ***
     *
     *****************************************************/
    private void UpdateSwipeSliders()
    {
        //Recupere le type de geste détecté
        string gesture = GestureMediator.GetGestureType();
        HandsE hand = GestureMediator.GetDetectedHand();

        // Glissements de la main gauche
        if (hand == HandsE.gauche && gesture.Contains("Swipe"))
        {
            if (gesture == "Swipe gauche") { StartCoroutine(SwipeSliderTimer(leftHandSwipeSliders[0])); }
            if (gesture == "Swipe droite") { StartCoroutine(SwipeSliderTimer(leftHandSwipeSliders[1])); }
            if (gesture == "Swipe haut") { StartCoroutine(SwipeSliderTimer(leftHandSwipeSliders[2])); }
            if (gesture == "Swipe bas") { StartCoroutine(SwipeSliderTimer(leftHandSwipeSliders[3])); }
        }

        // Glissements de la main droite
        if (hand == HandsE.droite && gesture.Contains("Swipe"))
        {
            if (gesture == "Swipe gauche") { StartCoroutine(SwipeSliderTimer(rightHandSwipeSliders[0])); }
            if (gesture == "Swipe droite") { StartCoroutine(SwipeSliderTimer(rightHandSwipeSliders[1])); }
            if (gesture == "Swipe haut") { StartCoroutine(SwipeSliderTimer(rightHandSwipeSliders[2])); }
            if (gesture == "Swipe bas") { StartCoroutine(SwipeSliderTimer(rightHandSwipeSliders[3])); }
        }
    }

    /*****************************************************
     * UPDATE CLAP HAND SLIDERS
     *
     * INFO:    ...
     *
     *****************************************************/
    private void UpdateClapSliders()
    {
        //Recupere le type de geste détecté
        string gesture = GestureMediator.GetGestureType();

        if (gesture.Contains("Clap"))
        {
            if (DetectionController.GetInstance().IsHandDetected(HandsE.gauche) &&
                GestureMediator.GetDetectedHand() == HandsE.gauche)
            {
                StartCoroutine(SwipeSliderTimer(clapLeftHand));
            }
            if (DetectionController.GetInstance().IsHandDetected(HandsE.droite) &&
                GestureMediator.GetDetectedHand() == HandsE.droite)
            {
                StartCoroutine(SwipeSliderTimer(clapRightHand));
            }
        }
    }

    /*****************************************************
     * UPDATE USER COORD.
     *
     * INFO:    Met a jour la position, l'angle de rotation
     *          et le scaling de l'utilisateur sur des labels.
     *
     *****************************************************/
    private void UpdateUserCoord()
    {
        if (SelectionController.GetInstance() != null)
        {
            //La liste des objets sélectionnés avec le raycast
            List<SelectedObject> objectsToRotateList = SelectionController.GetInstance().GetSelectedObjects();

            if (objectsToRotateList.Count > 0)
            {
                float meanPosX = 0f;
                float meanPosY = 0f;
                float meanPosZ = 0f;

                float meanRotX = 0f;
                float meanRotY = 0f;
                float meanRotZ = 0f;

                float meanScaleX = 0f;
                float meanScaleY = 0f;
                float meanScaleZ = 0f;

                //Calculate Mean values for all objects coord.
                foreach (SelectedObject objectToRotate in objectsToRotateList)
                {
                    meanPosX += objectToRotate.TransformObject.position.x;
                    meanPosY += objectToRotate.TransformObject.position.y;
                    meanPosZ += objectToRotate.TransformObject.position.z;

                    meanRotX += objectToRotate.TransformObject.eulerAngles.x;
                    meanRotY += objectToRotate.TransformObject.eulerAngles.y;
                    meanRotZ += objectToRotate.TransformObject.eulerAngles.z;

                    meanScaleX += objectToRotate.TransformObject.localScale.x;
                    meanScaleY += objectToRotate.TransformObject.localScale.y;
                    meanScaleZ += objectToRotate.TransformObject.localScale.z;
                }

                //Coord. position de l'utilisateur
                posX.text = "x:  " + (meanPosX / objectsToRotateList.Count).ToString("F2");
                posY.text = "y:  " + (meanPosY / objectsToRotateList.Count).ToString("F2");
                posZ.text = "z:  " + (meanPosZ / objectsToRotateList.Count).ToString("F2");
                //Coord. rotation de l'utilisateur
                rotX.text = "x:  " + (meanRotX / objectsToRotateList.Count).ToString("F2");
                rotY.text = "y:  " + (meanPosY / objectsToRotateList.Count).ToString("F2");
                rotZ.text = "z:  " + (meanPosZ / objectsToRotateList.Count).ToString("F2");
                //Coord. scaling de l'utilisateur
                scaleX.text = "x:  " + (meanScaleX / objectsToRotateList.Count).ToString("F2");
                scaleY.text = "y:  " + (meanScaleY / objectsToRotateList.Count).ToString("F2");
                scaleZ.text = "z:  " + (meanScaleZ / objectsToRotateList.Count).ToString("F2");
            }
        }
    }

    /*****************************************************
     * RESET SLIDER
     *
     * INFO:    Permet de remettre un slider à son état
     *          original.
     *
     *****************************************************/
    public void ResetSlider(Slider slider)
    {
        slider.value = 0;
        slider.image.color = Color.gray;
    }

    /*****************************************************
     * SWIPE SLIDER TIMER
     *
     * INFO:    Permet de decrementer la valeur du slider 
     *          automatiquement de 100% à 0% en 1 sec. 
     *
     *****************************************************/
    public IEnumerator SwipeSliderTimer(Slider slider)
    {
        slider.value = 1f;
        float newValue = 0f;

        do
        {
            newValue = Time.deltaTime; //en 0.5 sec
            slider.value -= newValue;
            slider.image.color = Color.green;
            yield return null;
        } while (slider.value > 0);

        slider.value = 0f;
        slider.image.color = Color.gray;

        yield return null;
    }

    /*****************************************************
     * UPDATE RAYCAST DISTANCE
     *
     * INFO:    Affiche la distance entre le bout du doigt
     *          qui a le raycast et l'objet 3D pointé.
     *
     *****************************************************/
    private void UpdateRaycastDistance()
    {
        //*** TO OPTIMIZE ***

        if (SelectionController.GetInstance() != null)
        {
            if (DetectionController.GetInstance().IsHandDetected(SelectionController.GetInstance().GetHand()))
            {
                float raycastDistance = SelectionController.GetInstance().GetRaycastDistance();
                if (raycastDistance > 0f)
                {
                    raycastLabel.text = "Contact dist.: " +
                        SelectionController.GetInstance().GetRaycastDistance().ToString("F2") + " m";
                }
                else { raycastLabel.text = "Contact dist.: n/a"; }
            }
        }
    }

    /*****************************************************
     * RESET UI ELEMENTS
     *
     * INFO:    Si la main gauche et/ou droite n'est pas
     *          détectée, on remet les éléments du UI par
     *          defaut selon la main.
     *
     *****************************************************/
    private void ResetUIElements()
    {
        bool leftHandDetected = DetectionController.GetInstance().IsHandDetected(HandsE.gauche);
        bool rightHandDetected = DetectionController.GetInstance().IsHandDetected(HandsE.droite);

        // Reset les objets UI si la main gauche n'es pas détectée
        if (!leftHandDetected)
        {
            ResetSlider(clapLeftHand);
            ResetSlider(leftFistSlider);
            leftPinchSlider.value = -100;
            leftPinchSlider.image.color = Color.gray;
            for (int i = 0; i < leftHandSwipeSliders.Length; i++) { ResetSlider(leftHandSwipeSliders[i]); }
            for (int i = 0; i < leftHandImages.Length; i++) { leftHandImages[i].enabled = false; }
        }

        // Reset les objets UI si la main droite n'es pas détectée
        if (!rightHandDetected)
        {
            ResetSlider(clapRightHand);
            ResetSlider(rightFistSlider);
            rightPinchSlider.value = -100;
            rightPinchSlider.image.color = Color.gray;
            for (int i = 0; i < rightHandSwipeSliders.Length; i++) { ResetSlider(rightHandSwipeSliders[i]); }
            for (int i = 0; i < rightHandImages.Length; i++) { rightHandImages[i].enabled = false; }
        }
    }

    /*****************************************************
    * ADD GESTURE
    *
    * INFO:    Lorsqu'un geste est détecté, on appel cette
    *          fonction et le type de geste est ajouté dans 
    *          la liste.
    *
    *****************************************************/
    public void AddGesture(string gestureType)
    {
        GestureData results = new GestureData(gestureType, displayedTime);
        foreach (GestureData d in detectedGestureList)
        {
            if (d.gesture.Equals(gestureType)) { return; }
        }
        detectedGestureList.Add(results);
    }

    /*****************************************************
    * UPDATE DETECTED GESTURE
    *
    * INFO:    Maintient a jour l'affichage des gestes 
    *          détectés et construit le texte a afficher.
    *
    *****************************************************/
    private void UpdateDetectedGestures()
    {
        string message = "Gestes : \n";
        List<GestureData> removeList = new List<GestureData>();

        for (int i = 0; i < detectedGestureList.Count; i++)
        {
            GestureData data = detectedGestureList[i];
            message += data.gesture + "\n";

            float time = data.time;
            time -= Time.deltaTime;

            //Lorsque le temps est écoulé
            if (time <= 0.0f) { removeList.Add(data); }
            data.time = time;
            detectedGestureList[i] = data;
        }
        // Affiche les gestes détectés
        gestureLabel.text = message;

        //Retire le geste lorsque son temps d'affichage est écoulé
        foreach (GestureData data in removeList)
        {
            detectedGestureList.Remove(data);
        }
        displayedTime = 0f;
    }

    /*****************************************************
    * SET DISPLAY TIME
    *
    * INFO:    Modifie le temps que sont affichés les
    *          gestes détectés. Surtout utilisé par les
    *          gestes de type Swipe ou Clap car ils ne
    *          sont pas des gestes qui peuvent être
    *          maintenu par l'utilisateur. 
    *
    *****************************************************/
    public void SetDisplayTime(float displayedTime)
    {
        this.displayedTime = displayedTime;
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static SystemUIController GetInstance()
    {
        return instance;
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Appuyer sur 'M' (clavier) permet d'afficher 
    *          ou cacher le UI qui donne en temps reel de 
    *          l'information sur le systeme et la détection 
    *          des gestes.
    * 
    *          Maintient a jour tous les objets du UI en 
    *          appelant les fonctions séparées en type d'objets.
    *
    *****************************************************/
    void Update()
    {
        // Affiche ou chache le UI en appuyant sur 'M' 
        if (Input.GetKeyDown(keyToggleUI))
        {
            isDisplayed = isDisplayed ? false : true;
            leftObjectsUI.SetActive(isDisplayed);
            rightObjectsUI.SetActive(isDisplayed);
            topObjectsUI.SetActive(isDisplayed);
        }

        // Met a jour les éléments du UI seulement lorsque le UI est activé.
        if (isDisplayed)
        {
            UpdateDetectedGestures();
            UpdateFPSLabel();
            UpdateFistPinchSliders();
            UpdateHandFingertipImage();
            UpdateSwipeSliders();
            UpdateUserCoord();
            UpdateRaycastDistance();
            UpdateClapSliders();
        }

        //Remet les éléments du UI à l'état par defaut si la main n'est pas détectée
        ResetUIElements();
    }
}

/*****************************************************
* GESTURE DATA
*
* INFO:    Pour chaque geste détecté, on utilise
*          cette classe pour sauvegarder le type
*          et le moment auquel le geste a été détecté.
*
*****************************************************/
public class GestureData
{
    public string gesture;
    public float time;

    public GestureData(string gesture, float time)
    {
        this.gesture = gesture;
        this.time = time;
    }
}
