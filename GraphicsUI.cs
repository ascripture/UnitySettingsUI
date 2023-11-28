using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UltEvents;

public class GraphicsUI : MonoBehaviour
{
    UIDocument _ui;
    SelectUI _displayResolutionSelect;
    SelectUI _qualitySelect;

    public class SelectableQuality : SelectUI.Selectable
    {
        public int Index { get; set; }
        public string Label { get; set; }

        public bool Equals(SelectUI.Selectable other)
        {
            if (other is SelectableQuality quality)
            {
                return quality.Index == Index;
            }

            return false;
        }
    }

    public struct SelectableResolution : SelectUI.Selectable
    {
        public Resolution Resolution { get; set; }
        public readonly string Label => $"{Resolution.width}x{Resolution.height}";

        public bool Equals(SelectUI.Selectable other)
        {
            if (other is SelectableResolution resolution)
            {
                return resolution.Resolution.width == Resolution.width &&
                    resolution.Resolution.height == Resolution.height &&
                    resolution.Resolution.refreshRateRatio.Equals(Resolution.refreshRateRatio);
            }

            return false;
        }
    }

    void OnEnable()
    {
        _ui = GetComponent<UIDocument>();

        var selectUIs = GetComponents<SelectUI>();

        _displayResolutionSelect = selectUIs.First(ui => ui.ID == "displayResolution");
        _displayResolutionSelect.SetList(Screen.resolutions.Select(resolution => new SelectableResolution()
        {
            Resolution = resolution
        } as SelectUI.Selectable).ToList());
        _displayResolutionSelect.SetValue(new SelectableResolution() { Resolution = Screen.currentResolution });

        _qualitySelect = selectUIs.First(ui => ui.ID == "quality");
        var qualityLevels = Enumerable
            .Range(0, QualitySettings.count)
            .Select(index => new SelectableQuality() { Index = index, Label = QualitySettings.names[index] });
        _qualitySelect.SetList(qualityLevels.Select(quality => quality as SelectUI.Selectable).ToList());
        _qualitySelect.SetValue(new SelectableQuality() { Index = QualitySettings.GetQualityLevel(), Label = QualitySettings.names[QualitySettings.GetQualityLevel()] });
    }

    public void Apply()
    {
        if (_displayResolutionSelect.Value is SelectableResolution selectableResolution)
        {
            var resolution = selectableResolution.Resolution;
            Debug.Log($"Apply resolution {resolution.width}x{resolution.height}");
            Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.ExclusiveFullScreen, resolution.refreshRateRatio);
        }

        if (_qualitySelect.Value is SelectableQuality selectableQuality)
        {
            var index = selectableQuality.Index;
            Debug.Log($"Apply quality {index}: {selectableQuality.Label}");
            QualitySettings.SetQualityLevel(index, true);
        }
    }
}
