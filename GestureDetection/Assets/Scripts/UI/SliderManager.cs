using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider sliderPose;
    public Image sliderPoseProgress;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI poseNameText;

    public void UpdateSlider(string name, float fPercent, float progressPose)
    {
        progressText.text = fPercent * 100f + "%";
        poseNameText.text = name;
        sliderPose.value = fPercent;
        if (progressPose >= 1)
        {
            sliderPose.fillRect.GetComponent<Image>().color = new Color(214f / 255f, 17f / 255f, 143f / 255f);
        }
        else
        {
            sliderPose.fillRect.GetComponent<Image>().color = new Color(17f / 255f, 214f / 255f, 191f / 255f);
        }
        sliderPoseProgress.fillAmount = progressPose;
    }
}
