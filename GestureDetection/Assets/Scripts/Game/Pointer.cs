using System;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public Camera fpsCam;
    private float range = 50f;

    //private float impactForce = -15f;

    private string selectedItemName;    //Name of the selected item
    private int selectedItemPosition;   //Position in the list of hit test
    private bool selectedConfirmed;
    private Transform selectedTransform;

    private Transform right_index_bone2;
    private Transform transformObject;

    private Material mat;
    private Color originalHighlightColor;

    // To be selectable, each object MUST have, 1) Mesh Collider 2) Highlight shader

    private void Start()
    {
        selectedItemName = string.Empty;
        selectedItemPosition = -1;
        selectedConfirmed = false;

        //right_index_bone2 = GameObject.Find("RigidRoundHand_R").gameObject.transform.Find("index/bone2");
        if (gameObject.name == "Pointer")
        {
            transformObject = transform;
        }
        else
        {
            //transformObject = GameObject.Find("RigidRoundHand_R").gameObject.transform.Find("index/bone2");
            transformObject = GameObject.FindWithTag("RightIndexFinger").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Shoot(); // add a breakpoint here, help a lot for debug !
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ConfirmSelection();
        }


        //Vector3 forward = transform.TransformDirection(Vector3.forward) * 100;
        Vector3 forward = transformObject.forward * 1;

        /*
        Debug.DrawRay(transformObject.position, transformObject.forward * 0.05f Color.blue, 0.01f, true);
        Debug.DrawRay(transformObject.position, transformObject.up * 0.05f, Color.green, 0.01f, true);
        Debug.DrawRay(transformObject.position, transformObject.right * 50f, Color.red, 0.01f, true);
        */
        Debug.DrawRay(transformObject.position, transformObject.forward * 50f, Color.blue, 0.01f, true);
        Debug.DrawRay(transformObject.position, transformObject.up * 0.05f, Color.green, 0.01f, true);
        Debug.DrawRay(transformObject.position, transformObject.right * 0.05f, Color.red, 0.01f, true);

        Shoot();
    }

    void ConfirmSelection()
    {

        if (selectedConfirmed)  // Unselect
        {
            mat.SetColor("_HighlightColor", originalHighlightColor);

            Rigidbody body = selectedTransform.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.useGravity = true;
                body.isKinematic = false;
            }

            var rts = selectedTransform.GetComponentInParent<Leap.Unity.LeapRTS>();
            if (rts != null)
            {
                rts.enabled = false;
            }

            selectedConfirmed = false;
            SwitchHighlighted(false);
        }

        if (mat != null && selectedItemName != string.Empty) // Select
        {
            originalHighlightColor = mat.GetColor("_HighlightColor");
            mat.SetColor("_HighlightColor", Color.white);
            selectedConfirmed = true;
            SwitchHighlighted(true);
            Rigidbody body = selectedTransform.GetComponent<Rigidbody>();
            if (body !=null)
            {
                body.useGravity = false;
                body.isKinematic = true;
            }
            var rts = selectedTransform.GetComponentInParent<Leap.Unity.LeapRTS>();
            if (rts!=null)
            {
                rts.enabled = true;
            }

        }
    }

    void Shoot()
    {
        if (selectedConfirmed)
        {
            return; // item is selected, can't select another
        }
        RaycastHit[] allHits;
        allHits = Physics.RaycastAll(transformObject.position, transformObject.forward, range);

        Array.Sort(allHits, (x, y) => x.distance.CompareTo(y.distance));

        if (!Array.Exists(allHits, element => element.transform.name == selectedItemName))
        {
            SwitchHighlighted(false);
        }

        // Test for already highlited item

        bool alreadyHighlitedFound = false;
        for (int i = 0; i < allHits.Length; i++)
        {
            if (i == selectedItemPosition && allHits[i].transform.name == selectedItemName)
            {
                alreadyHighlitedFound = true;
            }
        }

        if (alreadyHighlitedFound)
        {
            return; // nothing to shoot anymore, quit Shoot() method
        }
        else
        {
            SwitchHighlighted(false);
        }

        int counter = 0;
        foreach (var rhit in allHits)
        {


            //Debug.Log(rhit.transform.name);

            if (rhit.transform.CompareTag("Selectable") && rhit.transform.name != selectedItemName)
            {
                if (rhit.transform.GetComponent(nameof(MeshRenderer)))
                {
                    mat = rhit.transform.GetComponent<MeshRenderer>().material;
                    SwitchHighlighted(true);
                    selectedItemName = rhit.transform.name;
                    selectedItemPosition = counter;
                    selectedTransform = rhit.transform;
                }

                return;
            }

            counter++;
        }

    }

    void SwitchHighlighted(bool isHighlighted)
    {
        if (mat != null)
        {
            mat.SetFloat("_Highlighted", (isHighlighted ? 1.0f : 0.0f));

           
        }
        if (isHighlighted == false)
        {
            selectedItemName = string.Empty;
            selectedItemPosition = -1;
        }
    }
}
