using TMPro;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings Reference")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMP_Text volumeValue;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] TMP_Text brightnessValue;

    [SerializeField] Image brightnessFilter;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateVolumeText(savedVolume);
        volumeSlider.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f);
        brightnessSlider.value = savedBrightness;
        brightnessFilter.color = new Color32(0,0,0, (byte)(200 - 200 * savedBrightness));
        UpdateBrightnessText(savedBrightness);
        brightnessSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
        UpdateVolumeText(value);
    }

    public void UpdateVolumeText(float value)
    {
        volumeValue.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void OnBrightnessChanged(float value)
    {
        brightnessFilter.color = new Color32(0, 0, 0, (byte)(200 - 200 * value));
        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
        UpdateBrightnessText(value);
    }

    public void UpdateBrightnessText(float value)
    {
        brightnessValue.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void OpenSettings()
    {
        gameObject.SetActive(true);
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
