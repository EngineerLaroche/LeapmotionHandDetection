using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoseUIManager : MonoBehaviour
{
    private TextMeshProUGUI m_Text;
    private UDPSocket socket;
    public SliderManager[] sliderManagers;
    public GameObject[] cubesFx;
    public LeapServiceProvider leapServiceController;

    private string lastMaxPose;
    private float currentPosePercentValue;
    private bool maxPoseFound;
    // Start is called before the first frame update
    void Start()
    {
        m_Text = GameObject.Find("udp_values").GetComponent<TextMeshProUGUI>();
        socket = new UDPSocket();
        currentPosePercentValue = 0;
        maxPoseFound = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (socket == null) return;

        string data = socket.readData;
        m_Text.text = data;

        if (maxPoseFound == false)
            ValueFromPose(data);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            maxPoseFound = false;
        }
    }

    void ValueFromPose(string poseData)
    {
        if (poseData.Length < 30) return;

        Hand hand = new Hand { Id = -1 };
        if (leapServiceController.GetLeapController() == null) return;
        foreach (var item in leapServiceController.GetLeapController().Frame(0).Hands) // Get last frame
        {
            if (item.IsRight) hand = item;
        }
        if (hand.Id == -1) return; // If no hand detected, return

        string maxPose = string.Empty;
        float maxPoseUDP = 0;

        for (int i = 0; i < poseData.Split(',').Length; i++)
        {
            string pose = poseData.Split(',')[i];
            var percent = pose.Split(':')[1];
            var name = pose.Split(':')[0];
            float fPercent = float.Parse(percent, System.Globalization.CultureInfo.InvariantCulture);

            if (fPercent > maxPoseUDP)
            {
                maxPose = name;
                maxPoseUDP = fPercent;
            }
        }

        if (maxPose == lastMaxPose)
        {
            currentPosePercentValue += 0.45f * Time.deltaTime;
        }
        else
        {
            currentPosePercentValue = 0;
        }


        for (int i = 0; i < poseData.Split(',').Length; i++)
        {
            string pose = poseData.Split(',')[i];
            var percent = pose.Split(':')[1];
            var name = pose.Split(':')[0];
            float fPercent = float.Parse(percent, System.Globalization.CultureInfo.InvariantCulture);

            float p = name == maxPose ? currentPosePercentValue : 0;

            currentPosePercentValue = Mathf.Clamp(currentPosePercentValue, 0, 1);

            if (p >= 1)
            {
                maxPoseFound = true;

                DoActionAfterPoseAccepted(name);
            }

            if (i < sliderManagers.Length) // Valide si le nombre de pose n'excède pas le nombre de sliders
            {
                sliderManagers[i].UpdateSlider(name, fPercent, p);
            }
        }

        lastMaxPose = maxPose;
    }

    void DoActionAfterPoseAccepted(string poseName)
    {
        foreach (var cube in cubesFx)
        {
            cube.SetActive(false);
            
        }
        switch (poseName)
        {
            case "ferme":
                // call something then hand is closed
                cubesFx[0].SetActive(true);
                break;
            case "ouvert":
                // call something then hand is open
                cubesFx[2].SetActive(true);
                break;
            case "pouce":
                // call something then thumb is extended
                cubesFx[3].SetActive(true);
                break;

            default:
                // not implemented
                break;
        }
    }
}
