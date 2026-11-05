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

    [SerializeField] Toggle bgmToggle;
    [SerializeField] Toggle sfxToggle;

    private bool isOn;

    public void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        isOn = false;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }
    private void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.SetValueWithoutNotify(savedVolume);
        AudioListener.volume = savedVolume;
        UpdateVolumeText(savedVolume);
        volumeSlider.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f);
        brightnessSlider.SetValueWithoutNotify(savedBrightness);
        brightnessFilter.color = new Color32(0,0,0, (byte)(200 - 200 * savedBrightness));
        UpdateBrightnessText(savedBrightness);
        brightnessSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);

        int savedBGM = PlayerPrefs.GetInt("savedBGM", 1);
        int savedSFX = PlayerPrefs.GetInt("savedSFX", 1);

        bgmToggle.SetIsOnWithoutNotify(savedBGM == 1);
        sfxToggle.SetIsOnWithoutNotify(savedSFX == 1);

        ApplyBGM(savedBGM == 1);
        ApplySFX(savedSFX == 1);

        bgmToggle.onValueChanged.AddListener(OnBGMChanged);
        sfxToggle.onValueChanged.AddListener(OnSFXChanged);
    }

    public void OnBGMChanged(bool isEnabled)
    {
        PlayerPrefs.SetInt("savedBGM", isEnabled ? 1 : 0);
        PlayerPrefs.Save();

        ApplyBGM(isEnabled);
    }

    public void OnSFXChanged(bool isEnabled)
    {
        PlayerPrefs.SetInt("savedSFX", isEnabled ? 1 : 0);
        PlayerPrefs.Save();

        ApplySFX(isEnabled);
    }

    public void ApplyBGM(bool enabled)
    {
        GameObject[] bgmObjects = GameObject.FindGameObjectsWithTag("BGM");

        foreach (GameObject obj in bgmObjects)
        {
            AudioSource source = obj.GetComponent<AudioSource>();

            if (source != null)
                source.mute = !enabled;
        }
    }

    public void ApplySFX(bool enabled)
    {
        GameObject[] sfxObjects = GameObject.FindGameObjectsWithTag("SFX");

        foreach (GameObject obj in sfxObjects)
        {
            AudioSource source = obj.GetComponent<AudioSource>();

            if (source != null)
                source.mute = !enabled;
        }
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
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        isOn = true;
    }

    public void CloseSettings()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        isOn = false;
    }

    public void ToggleSettings()
    {
        if (isOn)
            CloseSettings();
        else 
            OpenSettings();
    }
}
