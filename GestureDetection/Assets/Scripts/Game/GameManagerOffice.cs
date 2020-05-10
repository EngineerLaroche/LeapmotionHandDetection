using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerOffice : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] selectableObjets;
        selectableObjets = GameObject.FindGameObjectsWithTag("Selectable");

        Leap.Unity.PinchDetector pinch_L = GameObject.Find("FPSController/FirstPersonCharacter/Hands/HandModels/Capsule Hand Left/PinchDetector_L").gameObject.GetComponent<Leap.Unity.PinchDetector>();
        Leap.Unity.PinchDetector pinch_R = GameObject.Find("FPSController/FirstPersonCharacter/Hands/HandModels/Capsule Hand Right/PinchDetector_R").gameObject.GetComponent<Leap.Unity.PinchDetector>();

        //Material highlitedMaterial = Resources.Load("Glow SimpleOfficeInteriors", typeof(Material)) as Material;
        Material highlitedMaterial = Resources.Load<Material>("Materials/m_Glow_SimpleOfficeInteriors");

        foreach (var item in selectableObjets)
        {
            if (item.GetComponent<Rigidbody>() == null)
            {
                var rigidbody = item.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }

            if (item.GetComponent<MeshCollider>() != null){ item.GetComponent<MeshCollider>().convex = true; }
            else{ item.AddComponent<MeshCollider>().convex = true; }

            var leapRTS = item.GetComponent<Leap.Unity.LeapRTS>();
            if (leapRTS != null)
            {
                leapRTS.PinchDetectorA = pinch_L;
                leapRTS.PinchDetectorB = pinch_R;
                leapRTS.enabled = false;
                leapRTS.useGUILayout = false;
            }
            else
            {
                var rts = item.AddComponent<Leap.Unity.LeapRTS>();
                rts.PinchDetectorA = pinch_L;
                rts.PinchDetectorB = pinch_R;
                rts.enabled = false;
                rts.useGUILayout = false;                
            }

            //item.gameObject.GetComponent<Renderer>().material = highlitedMaterial;
            item.gameObject.GetComponent<MeshRenderer>().material = highlitedMaterial;
            //item.GetComponent().

            //if (item.GetComponent<MeshCollider>() != null)
            //{
            //    item.GetComponent<MeshCollider>().convex = true;
            //}
            //if (item.GetComponent<MeshCollider>() == null)
            //{
            //    item.AddComponent<MeshCollider>().convex = true;
            //}
            //if (item.GetComponent<Rigidbody>() == null)
            //{
            //    item.AddComponent<Rigidbody>().isKinematic = false;
            //}

            //if (item.GetComponent<Leap.Unity.LeapRTS>() == null)
            //{
            //    var rts = item.AddComponent<Leap.Unity.LeapRTS>();
            //    rts.enabled = false;
            //    rts.useGUILayout = false;
            //}
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
