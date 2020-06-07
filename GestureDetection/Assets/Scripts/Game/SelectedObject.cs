using System;
using UnityEngine;

/*****************************************************
 * Class:   SELECTED OBJECT
 *
 * INFO:    Garde en memoire l'information de rotation,
 *          de position et de dimension d'un objet 3D
 *          pour gérer la sélection et manipulation de
 *          plusieurs objets 3D en même temps ou non. 
 *           
 *****************************************************/
[Serializable]
public class SelectedObject : MonoBehaviour
{
    //Object's transform (RTS)
    [SerializeField]
    private Transform transformObject = null;
    [SerializeField]
    private readonly int objectID = -1;

    //Objet's rotation, position and scale 
    [SerializeField]
    private Quaternion initialRotation;
    [SerializeField]
    private Vector3 initialPosition;
    [SerializeField]
    private Vector3 initialScale;

    //Object's anchor
    [SerializeField]
    private bool hasAnchor = false;
    [SerializeField]
    private Transform anchor = null;

    [SerializeField]
    private bool shouldHaveGravity = false;


    /*****************************************************
    * CONSTRUCTEUR
    *
    * INFO:    Initialise les parametres de l'objet 3D pris
    *          en parametres.
    *           
    *****************************************************/
    public SelectedObject(Transform _transform)
    {
        //Garde en mémoire les paramètres de l'objet
        transformObject = _transform;
        objectID = _transform.GetInstanceID();
        initialRotation = _transform.rotation;
        initialPosition = _transform.position;
        initialScale = _transform.localScale;

        //if(_transform.gameObject.GetComponent<Rigidbody>() != null)
            //shouldHaveGravity = _transform.gameObject.GetComponent<Rigidbody>().useGravity;
    }

    /*****************************************************
    * Object's core settings       
    *****************************************************/
    //Transform
    public Transform TransformObject { get => transformObject; set => transformObject = value; }   
    //ID
    public int GetID() { return objectID; }

    /*****************************************************
    * Object's transform settings       
    *****************************************************/
    //Rotation
    public Quaternion GetRotation() { return transformObject.rotation; }
    //Position
    public Vector3 GetPosition() { return transformObject.position; }
    //Scale
    public Vector3 GetScale() { return transformObject.localScale; }

    /*****************************************************
    * Object's initial transform settings       
    *****************************************************/
    //Rotation initiale
    public Quaternion GetInitialRotation() { return initialRotation; }
    //Position initiale
    public Vector3 GetInitialPosition() { return initialPosition; }
    //Scale initial
    public Vector3 GetInitialScale() { return initialScale; }

    /*****************************************************
    * Reset to initial transform settings    
    *****************************************************/
    //Remet la rotation à son état initial
    public void ResetRotation() { transformObject.rotation = Quaternion.Slerp(transformObject.rotation, initialRotation, 1f); }
    //Remet la position à son état initial
    public void ResetPosition() { transformObject.position = initialPosition; }
    //Remet le scaling à son état initial
    public void ResetScale() { transformObject.localScale = initialScale; }

    /*****************************************************
    * Object's anchor  
    *****************************************************/
    //RTS has anchor
    public bool HasAnchor { get => hasAnchor; set => hasAnchor = value; }
    //RTS Transform anchor
    public Transform Anchor { get => anchor; set => anchor = value; }
    
    //Object Gravity Component
    public bool ShouldHaveGravity { get => shouldHaveGravity; set => shouldHaveGravity = value; }
}
