using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   SWIPE GESTURE
 *
 * INFO:    Représente le geste de glisser rapidement
 *          la main vers la gauche ou la droite.
 * 
 *****************************************************/
public class SwipeGesture : GestureMediator
{
    // La main qui effectuer le geste
    [SerializeField] private HandsE hand;

    // La direction du glissement de la main entré en paramètre
    [SerializeField] private DirectionsE direction;

    // Parametres du glissement
    [Range(0.1f, 6.0f)]
    [SerializeField] private float velocity = 1.5f;
    [SerializeField] private float cooldownTime = 0.3f;
    private float coolDownLeft = 0.0f;

    // La direction du swipe pour le UI
    static string swipeDirection = "";

    /*****************************************************
    * IS HAND SWIPING
    *
    * INFO:    Valide le sens du glissement effectuée par
    *          la main et retourne la reponse.
    *           
    *****************************************************/
    bool IsHandSwiping(ref DirectionsE _direction)
    {
        // Recupere le controleur de la main détectée
        DetectionController.HandController detectHand = DetectionController.GetInstance().GetHand(hand);

        // Recupere la velocité de la main
        Vector3 velocity = detectHand.GetHandVelocity();
        velocity = Camera.main.transform.InverseTransformDirection(velocity);

        // Glissement vers la droite (+X)
        if (velocity.x >= this.velocity) 
        {
            _direction = DirectionsE.droite;
            return true;
        }
        // Glissement vers la gauche (-X)
        else if (velocity.x <= -this.velocity)
        {
            _direction = DirectionsE.gauche;
            return true;
        }
        // Glissement vers le haut (+Y)
        else if (velocity.y >= this.velocity)
        {
            _direction = DirectionsE.haut;
            return true;
        }
        // Glissement vers le bas (-Y)
        else if (velocity.y <= -this.velocity)
        {
            _direction = DirectionsE.bas;
            return true;
        }
        // Glissement vers l'interieur (+Z)
        else if (velocity.z >= this.velocity) 
        {
            _direction = DirectionsE.interieur;
            return true;
        }
        // Glissement vers l'exterieur (-Z)
        else if (velocity.z <= -this.velocity)
        {
            _direction = DirectionsE.exterieur;
            return true;
        }

        return false;
    }

    /*****************************************************
    * DETECTED FIST GESTURE
    *
    * INFO:    Valide la détection du geste qui consiste 
    *          à effectuer un glissement de la main. 
    *          La fonction communique directement avec le 
    *          médiateur du contrôleur de gestes.
    *          
    *          *** TO CHECK RIGHT SWIPE ***
    *           
    *****************************************************/
    public override bool IsDetectedGesture()
    {
        swipeDirection = "";

        if (DetectionController.GetInstance().IsHandDetected(hand) && coolDownLeft <= 0.0f)
        {
            // Sert simplement de reference  
            DirectionsE _direction = DirectionsE.droite;

            // Vérifie si la reference du geste est de type 'Swipe'
            if (IsHandSwiping(ref _direction))
            {
                // Si la reference equivaut bien au sens du glissement entré en paramètre
                if (_direction == direction)
                {
                    swipeDirection = direction.ToString();
                    coolDownLeft = cooldownTime;
                    return true;
                }
            }
        }
        return false;
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Maintient a jour le cooldown du glissement.
    *           
    *          *** À REVOIR ***
    *           
    *****************************************************/
    void Update()
    {
        if (coolDownLeft > 0.0f)
        {
            coolDownLeft -= Time.deltaTime;
            if (coolDownLeft < 0.0f) { coolDownLeft = 0.0f; }
        } 
    }

    /*****************************************************
    * GET GESTURE HAND
    *
    * INFO:    Retourne (écrase) la main détectéedu geste. 
    *          La fonction communique directement avec le 
    *          médiateur du contrôleur de gestes.
    *          
    *****************************************************/
    public override HandsE DetectedHand()
    {
        return hand;
    }

    /*****************************************************
    * GESTURE TYPE
    *
    * INFO:    Retourne (écrase) le type de geste détecté. 
    *          La fonction communique directement avec le 
    *          médiateur du contrôleur de gestes.
    *          
    *****************************************************/
    public override string DetectedGestureName()
    {
        return "Swipe main " + hand.ToString() + " vers --> " + swipeDirection;
    }

}
