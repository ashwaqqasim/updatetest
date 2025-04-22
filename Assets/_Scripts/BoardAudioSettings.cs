using UnityEngine;
using UnityEngine.UI;

public class BoardAudioSettings : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start()
    {
        // تحميل القيمة المحفوظة (أو قيمة افتراضية 1)
        float savedVolume = PlayerPrefs.GetFloat("BoardVolume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        // ربط حدث تغيير القيمة
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("BoardVolume", value);
    }
}
