
using UnityEngine;
using UnityEditor;

/*****************************************************
 * CLASS:   CONDITIONAL HIDE PROPERTY DRAWER
 *
 * INFO:    Permet de cacher ou desactiver des elements
 *          du panneau inspecteur dans Unity.
 *          
 * SOURCE: http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
 * 
 *****************************************************/
[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{

    /*****************************************************
    * ON GUI
    *
    * INFO:    When Unity wants to draw the property in the 
    *          inspector we need to:
    *          
    *          - Check the parameters that we used in our custom 
    *            attribute.
    *          - Hide and/or disable the property that is being 
    *            drawn based on the attribute parameters
    *
    *****************************************************/
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled) { EditorGUI.PropertyField(position, property, label, true); }
        GUI.enabled = wasEnabled;
    }

    /*****************************************************
    * GET PROPERTY HEIGHT
    *
    * INFO:    Calculate the height of our property so that 
    *           (when the property needs to be hidden) the 
    *           following properties that are being drawn don’t 
    *           overlap.
    *
    *****************************************************/
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled) { return EditorGUI.GetPropertyHeight(property, label); }
        else { return -EditorGUIUtility.standardVerticalSpacing; }
    }

    /*****************************************************
    * GET CONDITIONAL HIDE ATTRIBUTE RESULT
    *
    * INFO:    In order to check if the property should be 
    *           enabled or not we call GetConditionalHideAttributeResult.
    *
    *****************************************************/
    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt,  SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null) { enabled = sourcePropertyValue.boolValue; }
        else { Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField); }

        return enabled;
    }
}