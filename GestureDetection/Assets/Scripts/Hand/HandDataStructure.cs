using UnityEngine;
using System.Collections;

/*****************************************************
 * CLASS:   HAND DATA STRUCTURE
 *
 * INFO:    Structure de données du système de détection
 *          des gestes de la main. Les données de cette
 *          classe représentent celles de la main gauche 
 *          et de la main droite ainsi que des doigts de la main.
 * 
 *****************************************************/
public class HandDataStructure : MonoBehaviour
{

    /*****************************************************
    * HANDS
    *
    * ENUM (3):     Main gauche et main droite et une valeur 
    *               négative pour une main inconnu (mal détecté).
    * 
    *****************************************************/
    public enum HandsE
    {
        inconnu = -1,
        gauche = 0,
        droite = 1
    }

    /*****************************************************
    * FINGERS
    *
    * ENUM (6):    Les 5 doigts de la main et une valeur 
    *              négative pour un doigt inconnu (mal détecté).
    * 
    *****************************************************/
    public enum FingersE
    {
        inconnu = -1,
        pouce = 0,
        index = 1,
        majeur = 2,
        annulaire = 3,
        auriculaire = 4
    }

    /*****************************************************
    * BONES FINGER
    *
    * ENUM (3):    Les trois os de la main placés en ordre. 
    *              Celui de gauche (metacarpien) représente le
    *              premier os à l'intérieur de la main.
    * 
    *****************************************************/
    public enum BonesE
    {
        inconnu = -1,
        metacarpien = 0,
        proximal = 1,
        intermediaire = 2,
        distal = 3
    }

    /*****************************************************
    * HAND AXIS
    *
    * ENUM (3):    Composants de la main pour lesquels on
    *              cherche souvent l'axe de deplacement.
    * 
    *****************************************************/
    public enum HandAxisE
    {
        paume, pouce, doigt
    }

    /*****************************************************
    * ROTATION TYPE
    *
    * ENUM (3):    Le type de rotation de la main.
    * 
    *****************************************************/
    public enum RotationE { yaw, pitch, roll }


    /*****************************************************
    * LOOK TYPE
    *
    * ENUM :    Le type de rotation de la camera (vue).
    * 
    *****************************************************/
    public enum LookE { FingersPointing, HandPos, Raycast, YawPitch, Mouse, None }

    /*****************************************************
    * DIRECTIONS
    *
    * ENUM (6):    Les différents sens que la main pourrait
    *              effectuer un glissement (vélocité/swipe).
    * 
    *****************************************************/
    public enum DirectionsE
    {
        haut,
        bas,
        gauche,
        droite,
        exterieur,
        interieur, 
        inconnu
    }

    /*****************************************************
    * STRUCT:   FINGERS ACTIVATION
    *
    * INFO:     Permet de changer l'état des doigts d'une 
    *           main. Supporte l'identification de gestes 
    *           spécifiques avec l'utilisation des doigts 
    *           de la main.
    * 
    *****************************************************/
    [System.Serializable]
    public struct FingersActivation
    {
        //Les 5 doigts de la main
        public bool unPouce;
        public bool unIndex;
        public bool unMajeur;
        public bool unAnnulaire;
        public bool unAuriculaire;

        /*****************************************************
        * FINGERS ACTIVATION
        *
        * INFO:     Constructeur de la structure de donnée qui 
        *           itialise l'état (l'utilisation) des doigts 
        *           de la main.
        * 
        *****************************************************/
        public FingersActivation(bool _pouce, bool _index, bool _majeur, bool _annulaire, bool _auriculaire)
        {
            unPouce = _pouce;
            unIndex = _index;
            unMajeur = _majeur;
            unAnnulaire = _annulaire;
            unAuriculaire = _auriculaire;
        }

        /*****************************************************
        * GET FINGERS ACTIVATION
        *
        * INFO:     Retourne une liste (boolean) qui représente
        *           l'état (l'utilisation) des doigts de la main.
        * 
        *****************************************************/
        public bool[] GetFingersActivation()
        {
            bool[] fingersActivation = new bool[5];
            fingersActivation[0] = unPouce;
            fingersActivation[1] = unIndex;
            fingersActivation[2] = unMajeur;
            fingersActivation[3] = unAnnulaire;
            fingersActivation[4] = unAuriculaire;
            return fingersActivation;
        }
    }
}