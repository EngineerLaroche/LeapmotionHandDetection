using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRoomManager : MonoBehaviour
{
    private GameObject showRoomSam;
    private GameObject showRoomAlex;
    // Start is called before the first frame update
    void Start()
    {
        showRoomSam = GameObject.Find("ShowRoomImplementation").transform.GetChild(0).gameObject;
        showRoomAlex = GameObject.Find("ShowRoomImplementation").transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showRoomAlex.SetActive(!showRoomAlex.activeInHierarchy);
            showRoomSam.SetActive(!showRoomSam.activeInHierarchy);
        }
    }
}
