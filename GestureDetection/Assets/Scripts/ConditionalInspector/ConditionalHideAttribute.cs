using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

/*****************************************************
 * CLASS:   CONDITIONAL HIDE ATTRIBUTE
 *
 * INFO:    Permet de cacher ou desactiver des elements
 *          du panneau inspecteur dans Unity. Par exemple,
 *          si on active un checkbox dans l'inspecteur,
 *          on peut activer ou afficher une serie de 
 *          parametres pour un script C# quelconque.
 *          
 * SOURCE: http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
 * 
 *****************************************************/
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    //The name of the bool field that will be in control
    public string ConditionalSourceField = "";
    public string ConditionalSourceField2 = "";

    //TRUE = Hide in inspector / FALSE = Disable in inspector 
    public bool HideInInspector = false;

    /*****************************************************
    * CONDITIONAL HIDE ATTRIBUTE 1.0
    *
    * INFO:    Désactive l'objet de l'inspecteur en se basant
    *          sur un seul attribut (bool).
    *
    *****************************************************/
    public ConditionalHideAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = false;
    }

    /*****************************************************
    * CONDITIONAL HIDE ATTRIBUTE 1.1
    *
    * INFO:    Désactive l'objet de l'inspecteur en se basant
    *          sur deux attributs (bool).
    *
    *****************************************************/
    public ConditionalHideAttribute(string conditionalSourceField, string conditionalSourceField2)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.ConditionalSourceField2 = conditionalSourceField2;
        this.HideInInspector = false;
    }

    /*****************************************************
    * CONDITIONAL HIDE ATTRIBUTE 2.0
    *
    * INFO:    Cache l'objet de l'inspecteur en se basant 
    *          sur un attribut (bool) et un bool pour cacher
    *          ou non l'objet.
    *
    *****************************************************/
    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
    }
}