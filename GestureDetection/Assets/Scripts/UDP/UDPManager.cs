using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UDPManager : MonoBehaviour
{
    private TextMeshProUGUI m_Text;
    private UDPSocket socket;
    // Start is called before the first frame update
    void Start()
    {
        m_Text = GameObject.Find("udp_values").GetComponent<TextMeshProUGUI>();
        socket = new UDPSocket();
        //socket.ReadAsyncStart();
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Text.text = socket.readData;
    }
}
