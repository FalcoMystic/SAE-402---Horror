using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrequencyPuzzle : MonoBehaviour
{
    [Header("Sliders")]
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;

    [Header("Target Values")]
    public int targetValue1 = 440;
    public int targetValue2 = 60;
    public int targetValue3 = 120;

    [Header("UI")]
    public TextMeshProUGUI successMessageText;
    [TextArea]
    public string successMessage = "";
    public TextMeshProUGUI slider1ValueText;
    public TextMeshProUGUI slider2ValueText;
    public TextMeshProUGUI slider3ValueText;

    [Header("Settings")]
    public bool forceWholeNumbers = true;
    public int sliderStep = 10;

    private bool isSnappingSlider;

    private void Start()
    {
        ConfigureSliders();

        if (slider1 != null)
        {
            slider1.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (slider2 != null)
        {
            slider2.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (slider3 != null)
        {
            slider3.onValueChanged.AddListener(OnSliderValueChanged);
        }

        UpdateFrequencyLabels();
        CheckValues();
    }

    private void OnDestroy()
    {
        if (slider1 != null)
        {
            slider1.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        if (slider2 != null)
        {
            slider2.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        if (slider3 != null)
        {
            slider3.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float _)
    {
        SnapAllSlidersToStep();
        UpdateFrequencyLabels();
        CheckValues();
    }

    private void ConfigureSliders()
    {
        if (!forceWholeNumbers)
        {
            return;
        }

        if (slider1 != null) slider1.wholeNumbers = true;
        if (slider2 != null) slider2.wholeNumbers = true;
        if (slider3 != null) slider3.wholeNumbers = true;
    }

    private void SnapAllSlidersToStep()
    {
        if (isSnappingSlider || sliderStep <= 1)
        {
            return;
        }

        isSnappingSlider = true;
        SnapSliderToStep(slider1);
        SnapSliderToStep(slider2);
        SnapSliderToStep(slider3);
        isSnappingSlider = false;
    }

    private void SnapSliderToStep(Slider slider)
    {
        if (slider == null)
        {
            return;
        }

        float snapped = Mathf.Round(slider.value / sliderStep) * sliderStep;
        snapped = Mathf.Clamp(snapped, slider.minValue, slider.maxValue);
        if (!Mathf.Approximately(slider.value, snapped))
        {
            slider.SetValueWithoutNotify(snapped);
        }
    }

    private void UpdateFrequencyLabels()
    {
        if (slider1 != null && slider1ValueText != null)
        {
            slider1ValueText.text = Mathf.RoundToInt(slider1.value) + " Hz";
        }

        if (slider2 != null && slider2ValueText != null)
        {
            slider2ValueText.text = Mathf.RoundToInt(slider2.value) + " Hz";
        }

        if (slider3 != null && slider3ValueText != null)
        {
            slider3ValueText.text = Mathf.RoundToInt(slider3.value) + " Hz";
        }
    }

    public void CheckValues()
    {
        if (slider1 == null || slider2 == null || slider3 == null || successMessageText == null)
        {
            return;
        }

        bool isCorrect = Mathf.RoundToInt(slider1.value) == targetValue1
            && Mathf.RoundToInt(slider2.value) == targetValue2
            && Mathf.RoundToInt(slider3.value) == targetValue3;

        if (isCorrect)
        {
            successMessageText.text = successMessage;
        }
        else
        {
            successMessageText.text = string.Empty;
        }
    }

    private void Update()
    {
        // Fallback check in case slider values are changed by script instead of UI drag.
        SnapAllSlidersToStep();
        CheckValues();
    }
}