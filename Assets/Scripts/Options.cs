using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public TextMeshProUGUI volumeNumberText;
    private Slider volumeSlider;

    void Start()
    {
        volumeSlider = GetComponent<Slider>();
        SetVolumeText(volumeSlider.value);
    }

    public void SetVolumeText(float nr)
    {
        volumeNumberText.text = nr.ToString();
    }
}
