using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Leap.Unity;
using TMPro;
using Leap;

/*****************************************************
 * Class:   PALM UI CONTROLLER
 *
 * INFO:    Le menu de la main gauche lorsqu celle-ci
 *          a la paume de la main vers le haut.
 *           
 *****************************************************/
public class PalmUIController : HandDataStructure
{
    //Instance de la classe
    private static PalmUIController instance = null;

    //Pour le label 'angle' (rotation)
    private float angle = 90f;
    private readonly float incrRotation = 10f;
    private bool isControlRotation = true;
    private TextMeshPro angleLabel;

    //Pour le label 'amplification' (translation)
    private float amplifyTranslation = 3f;
    private readonly float incrTranslation = 1f;

    //Pour le label 'amplification' (scale)
    private float amplifyScale = 5f;
    private readonly float incrScale = 1f;

    //L'etat des boutons Rotation, Translation et Scaling (Home menu)
    private bool isFunctionalitiesActive = true;

    //Etat de la manipulation libre d'objet 3D
    private static bool isControlledManipulation = false;

    //Pour Sprites et Labels
    private Color black = new Color32(0, 0, 0, 255);
    private Color blackTrans = new Color32(0, 0, 0, 60);
    private Color white = new Color32(255, 255, 255, 255);
    private Color whiteTrans = new Color32(255, 255, 255, 60);

    //Si le PalmUI est affiché
    private bool isPalmUIOpen = false;
    private bool isPalmUIInitialized = false;
    private bool palmUIWasOpen = false;

    //Bouton (type de manipulation - Home menu) du Palm UI
    private GameObject manipulationType;


    /*****************************************************
    * AWAKE
    *
    * INFO:    Instance de la classe
    *
    *****************************************************/
    private void Awake()
    {
        //Initialise l'instance de cette classe
        if (instance == null) { instance = this; }
        else { Destroy(this); }

        //Le seul bouton de départ du Palm UI ne devrait pas etre affiché
        manipulationType = GameObject.FindWithTag("ManipulationType");
        if (manipulationType != null) manipulationType.SetActive(false);
    }

    /*****************************************************
    * TOGGLE MANIPULATION SPRITE
    *
    * INFO:    Inverse le sprite (blanc / noir) du bouton
    *          qui active ou non la manipulation d'objet
    *          libre ou contrôlé. Action effectuée 
    *          lorsqu'on appui sur le bouton en question.
    *
    *****************************************************/
    public void ToggleManipulationSprite()
    {
        GameObject objectManip = GameObject.FindWithTag("FreeManipulation");
        if (objectManip != null)
        {
            SpriteRenderer freeManip = objectManip.GetComponent<SpriteRenderer>();
            if (freeManip != null)
            {
                freeManip.color = isControlledManipulation ? black : blackTrans;
                isControlledManipulation = isControlledManipulation ? false : true;

                //Desactive ou non la manipulation libre sur les objets 3D
                SetLeapRTS(isControlledManipulation);
            }
        }
    }

    /*****************************************************
    * SET LEAP RTS
    *
    * INFO:    Desactive ou active la manipulation libre
    *          (Leap RTS) des objets 3D. C'est en changeant
    *          l'etat du script LeapRTS des objets qu'on
    *          y parvient.
    *
    *****************************************************/
    private void SetLeapRTS(bool _isControlled)
    {
        GameObject objectManip = GameObject.FindWithTag("Selectable");
        if(objectManip != null)
        {
            LeapRTS leapRTS = objectManip.GetComponent<LeapRTS>();
            if(leapRTS != null)
            {
                leapRTS.enabled = _isControlled ? false : true;
            }
        }
    }

    /*****************************************************
    * INCREASE ANGLE
    *
    * INFO:    Augmente l'angle de rotation (swipe) lorsque
    *          l'utilisateur appui sur le bouton (+), puis
    *          met a jour la valeur affiché sur le label.
    *          L'angle maximum est de 180°.
    *
    *****************************************************/
    public void IncreaseAngle()
    {
        angleLabel = GameObject.FindWithTag("AngleLabel").GetComponent<TextMeshPro>();
        if (angle < 180f && angleLabel != null)
        {
            angle += incrRotation;
            angleLabel.text = angle.ToString() + "°";
            RotationController.GetInstance().SetAngle(angle);
        }
    }

    /*****************************************************
    * DECREASE ANGLE
    *
    * INFO:    Diminue l'angle de rotation (swipe) lorsque
    *          l'utilisateur appui sur le bouton (-), puis
    *          met a jour la valeur affiché sur le label.
    *          L'angle minimum est de 10°.
    *
    *****************************************************/
    public void DecreaseAngle()
    {
        angleLabel = GameObject.FindWithTag("AngleLabel").GetComponent<TextMeshPro>();
        if (angle > 10f && angleLabel != null)
        {
            angle -= incrRotation;
            angleLabel.text = angle.ToString() + "°";
            RotationController.GetInstance().SetAngle(angle);
        }
    }

    /*****************************************************
    * INCREASE TRANSLATION AMPLIFICATION
    *
    * INFO:    Permet d'augmenter la valeur qui amplifie
    *          la translation d'un objet et mettre a jour
    *          le label. Non concus pour la manipulation libre.
    *          L'amplification doit avoir un maximum de 10.
    *
    *****************************************************/
    public void IncreaseTranslationAmplification()
    {
        TextMeshPro amplifyLabel = GameObject.FindWithTag("AmplifyTranslationLabel").GetComponent<TextMeshPro>();
        if (amplifyTranslation < 10f && amplifyLabel != null)
        {
            amplifyTranslation += incrTranslation;
            amplifyLabel.text = amplifyTranslation.ToString();
            TranslationController.GetInstance().SetTranslationAmplification(amplifyTranslation);
        }
    }

    /*****************************************************
    * DECREASE TRANSLATION AMPLIFICATION
    *
    * INFO:    Permet de réduire la valeur qui amplifie
    *          la translation d'un objet et mettre a jour
    *          le label. Non concus pour la manipulation libre.
    *          L'amplification doit avoir un minimum de 1.
    *
    *****************************************************/
    public void DecreaseTranslationAmplification()
    {
        TextMeshPro amplifyLabel = GameObject.FindWithTag("AmplifyTranslationLabel").GetComponent<TextMeshPro>();
        if (amplifyTranslation > 1f && amplifyLabel != null)
        {
            amplifyTranslation -= incrTranslation;
            amplifyLabel.text = amplifyTranslation.ToString();
            TranslationController.GetInstance().SetTranslationAmplification(amplifyTranslation);
        }
    }

    /*****************************************************
    * INCREASE SCALE AMPLIFICATION
    *
    * INFO:    Permet d'augmenter la valeur qui amplifie
    *          le scaling d'un objet et mettre a jour
    *          le label. Non concus pour la manipulation libre.
    *          L'amplification doit avoir un maximum de 10.
    *
    *****************************************************/
    public void IncreaseScaleAmplification()
    {
        TextMeshPro amplifyLabel = GameObject.FindWithTag("AmplifyScaleLabel").GetComponent<TextMeshPro>();
        if (amplifyScale < 10f && amplifyLabel != null)
        {
            amplifyScale += incrTranslation;
            amplifyLabel.text = amplifyScale.ToString();
            ScaleController.GetInstance().SetScaleAmplification(amplifyScale);
        }
    }

    /*****************************************************
    * DECREASE SCALE AMPLIFICATION
    *
    * INFO:    Permet de réduire la valeur qui amplifie
    *          le scaling d'un objet et mettre a jour
    *          le label. Non concus pour la manipulation libre.
    *          L'amplification doit avoir un minimum de 1.
    *
    *****************************************************/
    public void DecreaseScaleAmplification()
    {
        TextMeshPro amplifyLabel = GameObject.FindWithTag("AmplifyScaleLabel").GetComponent<TextMeshPro>();
        if (amplifyScale > 1f && amplifyLabel != null)
        {
            amplifyScale -= incrTranslation;
            amplifyLabel.text = amplifyScale.ToString();
            ScaleController.GetInstance().SetScaleAmplification(amplifyScale);
        }
    }

    /*****************************************************
    * CHECK TOGGLE X
    *
    * INFO:    Limite ou non la transformation (manipulation) 
    *          d'un objet sur l'axe des X.
    *
    *****************************************************/
    public void CheckToggleX()
    {
        /*SpriteRenderer toggleSprite;

        if (GetComponentInChildren<SpriteRenderer>() != null)
        {
            toggleSprite = GetComponentInChildren<SpriteRenderer>();
        }
        toggleSprite.enabled = toggleSprite.enabled ? false : true;
        RotationController.GetInstance().SetRotationX(toggleSprite.enabled);
        */
    }

    /*****************************************************
    * CHECK TOGGLE Y
    *
    * INFO:    Limite ou non la transformation (manipulation) 
    *          d'un objet sur l'axe des Y.
    *
    *****************************************************/
    public void CheckToggleY()
    {
        /*SpriteRenderer toggleSprite;

        if (GetComponentInChildren<SpriteRenderer>() != null)
        {
            toggleSprite = GetComponentInChildren<SpriteRenderer>();
        }
        toggleSprite.enabled = toggleSprite.enabled ? false : true;
        RotationController.GetInstance().SetRotationY(toggleSprite.enabled);
        */
    }

    /*****************************************************
    * RESET OBJECT ROTATION
    *
    * INFO:    Remet l'objet avec sa rotation (angles) initiale.
    *
    *****************************************************/
    public void ResetObjectRotation()
    {
        RotationController.GetInstance().ResetObjectRotation();
    }

    /*****************************************************
    * RESET OBJECT POSITION
    *
    * INFO:    Remet l'objet a sa position initiale.
    *
    *****************************************************/
    public void ResetObjectPosition()
    {
        TranslationController.GetInstance().ResetObjectPosition();
    }

    /*****************************************************
    * RESET OBJECT SCALE
    *
    * INFO:    Remet l'objet a sa dimension initiale.
    *
    *****************************************************/
    public void ResetObjectScale()
    {
        ScaleController.GetInstance().ResetObjectScale();
    }

    /*****************************************************
   * SET FREE ROTATION
   *
   * INFO:    Permet d'activer ou désactiver la rotation 
   *          par swipe sans contrôle d'angle. 
   *
   *****************************************************/
    public void SetFreeRotation()
    {
        SpriteRenderer lockedRotation = GameObject.FindWithTag("LockedRotation").GetComponent<SpriteRenderer>();
        SpriteRenderer unlockedRotation = GameObject.FindWithTag("UnlockedRotation").GetComponent<SpriteRenderer>();

        if (lockedRotation != null && unlockedRotation != null)
        {
            //Si la rotation contrôlé est active, on desactive la rotation contrôlé par angle
            RotationController.GetInstance().SetLockRotation(isControlRotation ? false : true);
            lockedRotation.enabled = isControlRotation ? false : true;
            unlockedRotation.enabled = isControlRotation ? true : false;
            isControlRotation = isControlRotation ? false : true;

            //Pour changer la couleur du label 'angle' (rotation)
            angleLabel = GameObject.FindWithTag("AngleLabel").GetComponent<TextMeshPro>();
            if (angleLabel != null) angleLabel.color = isControlRotation ? whiteTrans : white;           
        }
    }

    /*****************************************************
    * UPDATE FUNCTIONALITIES BUTTON
    *
    * INFO:    Active ou desactive une serie de composants 
    *          des boutons (fonctionalités) du Home menu.
    *          Permet de préserver les animations en ne
    *          désactivant pas les GameObjects. On change
    *          l'état des composants de type visuel ou script.
    *          
    *          A utiliser uniquement avec les boutons du
   *           Home menu (Rotation, Translation, Scale)
    *
    *****************************************************/
    public void UpdateFunctionalitiesButton()
    {
        //Les boutons de fonctionalités du Home menu
        GameObject rotation = GameObject.FindWithTag("RotationButton");
        GameObject translation = GameObject.FindWithTag("TranslationButton");
        GameObject scaling = GameObject.FindWithTag("ScalingButton");

        if (rotation != null && translation != null && scaling != null)
        {
            //Si les boutons sont actifs, on désactive les Mesh, Sprite et scripts des enfants
            IterateOverChild(rotation, isFunctionalitiesActive ? false : true);
            IterateOverChild(translation, isFunctionalitiesActive ? false : true);
            IterateOverChild(scaling, isFunctionalitiesActive ? false : true);
            isFunctionalitiesActive = isFunctionalitiesActive ? false : true;
        }
    }

    /*****************************************************
    * ITERATE OVER CHILD
    *
    * INFO:    Parcours les enfants (GameObjects) pour
    *          activer ou desactiver les composants visuels
    *          ainsi que les scripts ciblés.
    *
    *****************************************************/
    private void IterateOverChild(GameObject _gameObject, bool _state)
    {
        //Change l'état des scripts de type MonoBehavior dans le GameObject
        MonoBehaviour[] monobehaviorScripts = _gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in monobehaviorScripts) script.enabled = _state;

        //Itère a travers les enfants et change l'etat des composants visuels
        foreach (Transform buttonCube in _gameObject.transform)
        {
            buttonCube.GetComponent<MeshRenderer>().enabled = _state;
            foreach (Transform cube in buttonCube)
            {
                cube.GetComponent<MeshRenderer>().enabled = _state;
                foreach (Transform img in cube)
                {
                    img.GetComponent<SpriteRenderer>().enabled = _state;
                }
            }
        }
    }

    /*****************************************************
    * SET IS PALM UI OPEN
    *
    * INFO:    Permet de mettre a jour le statut d'affichage
    *          du Palm UI.
    *
    *****************************************************/
    public void SetIsPalmUIOpen()
    {
        if (isPalmUIInitialized)
        {
            isPalmUIOpen = isPalmUIOpen ? false : true;
            SoundController.GetInstance().PlaySound(AudioE.menu);
        }
    }

    /*****************************************************
    * IS PALM UI OPEN
    *
    * INFO:    Retourne l'état d'affichage du Palm UI.
    *
    *****************************************************/
    public bool IsPalmUIOpen()
    {
        return isPalmUIOpen;
    }

    /*****************************************************
    * GET IS CONTROLL MANIPULATION
    *
    * INFO:    Retourne vrai si l'option pour manipuler
    *          un objet de façon libre (sans contraintes) 
    *          est désactivée.
    *
    *****************************************************/
    public bool IsManipulationControlled()
    {
        return isControlledManipulation;
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static PalmUIController GetInstance()
    {
        return instance;
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    
    *
    *****************************************************/
    void Update()
    {
        if (DetectionController.GetInstance().IsHandDetected(HandsE.gauche) && !isPalmUIInitialized)
        {
            if (DetectionController.GetInstance().IsHandDetected(HandsE.gauche) && !isPalmUIInitialized)
            {
                //Activation de depart du Palm UI
                manipulationType.SetActive(true);
                isPalmUIInitialized = true;
            }
        }
    }
}
