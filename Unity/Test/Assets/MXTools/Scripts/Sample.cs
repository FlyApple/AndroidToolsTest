using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Sample : MonoBehaviour
{
    [SerializeField]
    private Slider _slider_amplitude;
    [SerializeField]
    private Slider _slider_duration;
    [SerializeField]
    private Toggle _toggle_obsolete;
    [SerializeField]
    private Toggle _toggle_legacy;
    [SerializeField]
    private ToggleGroup _group_vibrationType;
    [SerializeField]
    private Text _label_desc;
    [SerializeField]
    private Button _button_vibrate;

    // Start is called before the first frame update
    void Start()
    {
        //
        var label_amplitude = this._slider_amplitude.transform.Find("Label").GetComponent<Text>();
        this._slider_amplitude.onValueChanged.AddListener((v) => {
            int amplitude = Mathf.RoundToInt(v * 255.0f);
            string text = string.Format("Amplitude {0:D}", amplitude); ;
            if (amplitude == 0)
            {
                text = string.Format("Amplitude Click", amplitude);
            }
            else if (amplitude == 1)
            {
                text = string.Format("Amplitude Double Click", amplitude);
            }
            else if (amplitude == 2)
            {
                text = string.Format("Amplitude Tick", amplitude);
            }
            else if (amplitude == 5)
            {
                text = string.Format("Amplitude Heavy Tick", amplitude);
            }
            else if (amplitude == 255)
            {
                text = string.Format("Amplitude (default)", amplitude);
            }
            label_amplitude.text = text;
        });

        var label_duration = this._slider_duration.transform.Find("Label").GetComponent<Text>();
        this._slider_duration.onValueChanged.AddListener((v) => {
            float seconds = (float)v * 1.0f;
            if (seconds < 0.01f)
            {
                seconds = 0.01f;
            }
            string text = string.Format("{0:F2} second", seconds);
            label_duration.text = text;
        });

        this._toggle_legacy.onValueChanged.AddListener((v) => {
            if (v)
            {
                this._toggle_obsolete.interactable = true;

                Toggle[] toggles = this._group_vibrationType.GetComponentsInChildren<Toggle>();
                foreach(var toggle in toggles)
                {
                    toggle.interactable = false;
                }
            }
            else
            {
                this._toggle_obsolete.interactable = false;
                this._toggle_obsolete.isOn = false;

                Toggle[] toggles = this._group_vibrationType.GetComponentsInChildren<Toggle>();
                foreach (var toggle in toggles)
                {
                    toggle.interactable = true;
                }
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleClickedVibrate()
    {
        Toggle active_toggle = null;
        foreach(var toggle in this._group_vibrationType.ActiveToggles())
        {
            active_toggle = toggle;
            break;
        }

        bool alarm = true;
        if(active_toggle != null && active_toggle.name.ToUpper() == "RINGTONE")
        {
            alarm = false;
        }

        int amplitude = Mathf.RoundToInt(this._slider_amplitude.value * 255.0f);
        if (amplitude >= 255) { amplitude = -1; }

        float seconds = (float)this._slider_duration.value * 1.0f;
        if (seconds < 0.01f)
        {
            seconds = 0.01f;
        }
        int milliseconds = Mathf.RoundToInt(seconds * 1000.0f);

        this._label_desc.text = string.Format("Device SDK {0:D} Android {1}. milliseconds {2:D}, amplitude {3:D}",
        DeviceUtils.Device.SDK_INT, DeviceUtils.Device.RELEASE,
        milliseconds, amplitude);


        DeviceUtils.Vibration.vibrate(milliseconds, amplitude, alarm, this._toggle_legacy.isOn, this._toggle_obsolete.isOn);
    }
}
