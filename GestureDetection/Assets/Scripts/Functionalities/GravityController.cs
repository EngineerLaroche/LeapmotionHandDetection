using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * Class:   GRAVITY CONTROLLER
 *
 * INFO:    Permet de changer la gravité des objets
 *          sélectionnés par raycast ou de tous les
 *          objets sélectionnables de la scène.
 *           
 *****************************************************/
public class GravityController : MonoBehaviour
{
    private static GravityController instance = null;

    public bool allowGravityChange = true;
    public bool onlySelected = true;

    private bool isCheckInitiated = false;
    private bool isGravityDisabled = false;
    private bool canPlaySound = true;

    /*****************************************************
    * AWAKE
    *
    * INFO:    Instance de la classe.
    *
    *****************************************************/
    void Awake()
    {
        //Initialise l'instance de cette classe
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }

    /*****************************************************
    * START TOGGLE GRAVITY
    *
    * INFO:    Change l'état du déclencheur qui démarre
    *          le changement de la gravité.
    *
    *****************************************************/
    public void StartToggleGravity()
    {
        isGravityDisabled = !isGravityDisabled;
        isCheckInitiated = true;
    }

    /*****************************************************
    * STOP TOGGLE GRAVITY
    *
    * INFO:    Change l'état du déclencheur qui démarre
    *          le changement de la gravité.
    *
    *****************************************************/
    public void StopToggleGravity()
    {
        isCheckInitiated = false;
    }

    /*****************************************************
    * MANAGE ALL SELECTABLE OBJECTS GRAVITY
    *
    * INFO:    Boucle autour de tous les objets dela scène 
    *          qui ont le bon tag pour changer l'état de 
    *          la gravité de ceux-ci.
    *
    *****************************************************/
    private void ManageAllSelectableObjectsGravity()
    {
        //La liste de tous lobjets sélectionnés avec le raycast
        GameObject[] selectableObjects = GameObject.FindGameObjectsWithTag("Selectable");
        if (selectableObjects.Length > 0)
        {
            foreach (GameObject obj in selectableObjects) { ToggleGravity(obj, isGravityDisabled); }
        }
    }

    /*****************************************************
    * MANAGE SELECTED OBJECTS GRAVITY
    *
    * INFO:    Boucle autour de tous les objets sélectionnés 
    *          pour changer l'état de la gravité de ceux-ci.
    *
    *****************************************************/
    private void ManageSelectedObjectsGravity()
    {
        List<SelectedObject> selectedObjects = SelectionController.GetInstance().GetSelectedObjects();
        if (selectedObjects.Count > 0)
        {
            foreach (SelectedObject obj in selectedObjects)
            {
                obj.ShouldHaveGravity = isGravityDisabled;
                ToggleGravity(obj.TransformObject.gameObject, isGravityDisabled);
            }
        }
    }

    /*****************************************************
    * TOGGLE GRAVITY
    *
    * INFO:    Change l'etat de la gravité des objets.
    *          Attrape les Objets qui n'ont pas les Components
    *          qui auraient du être configuté.
    *
    *****************************************************/
    private void ToggleGravity(GameObject _gameObject, bool _isEnabled)
    {
        Rigidbody rigidBody = _gameObject.GetComponent<Rigidbody>();
        MeshCollider collider = _gameObject.GetComponent<MeshCollider>();

        //Ajout d'un RigidBody et Collider si ce n'est pas deja fait
        if (rigidBody == null) { rigidBody = _gameObject.AddComponent<Rigidbody>(); }
        if (collider == null) { collider = _gameObject.AddComponent<MeshCollider>(); }
        
        if (rigidBody != null)
        {
            rigidBody.isKinematic = _isEnabled ? false : true;
            rigidBody.useGravity = _isEnabled;
        }
        //Devrait toujours etre convex
        if (collider != null) { collider.convex = true; }
    }

    /*****************************************************
    * TOGGLE PAUSE GRAVITY
    *
    * INFO:    Permet d'arreter temporairement la gravité
    *          des objets sélectionnés et manipulésjusqu'à 
    *          ce que l'utilisateur relache les objets.
    *
    *****************************************************/
    public void ToAllowGravity(bool enableGravity, bool playSound)
    {
        canPlaySound = false;

        //La liste des objets sélectionnés avec le raycast
        List<SelectedObject> selectedObjects = SelectionController.GetInstance().GetSelectedObjects();
        if (selectedObjects.Count > 0)
        {
            int i = 1;
            foreach (SelectedObject obj in selectedObjects)
            {
                if (enableGravity)
                {
                    //Si l'objet avait de la gravité avant qu'on l'enlève temporairement
                    if (obj.ShouldHaveGravity) { ToggleGravity(obj.TransformObject.gameObject, true); }
                    else { ToggleGravity(obj.TransformObject.gameObject, false); }
                    if (i == selectedObjects.Count) { canPlaySound = true; }
                }
                else
                {
                    ToggleGravity(obj.TransformObject.gameObject, false);
                    if (i == selectedObjects.Count) { canPlaySound = true; }
                }
                i++;
            }
        }
        if (canPlaySound && playSound)
        {
            //SoundController.GetInstance().PlaySound(AudioE.grab);
            canPlaySound = false;
        }
    }

    /*****************************************************
    * UPDATE
    *
    * INFO:    Change l'état de la gravité de tous les objets
    *          ou seulement des objets sélectionnés.
    *          C'est l'utilisateur qui décide de l'option.
    *
    *****************************************************/
    void Update()
    {
        if (allowGravityChange && isCheckInitiated)
        {
            //Desactiver la gravité seulement pour les objets sélectionnés
            if (onlySelected) { ManageSelectedObjectsGravity(); }
            //Desactiver la gravité pour tous les objets sélectable
            else { ManageAllSelectableObjectsGravity(); }
        }
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static GravityController GetInstance()
    {
        return instance;
    }
}
