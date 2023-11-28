using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UltEvents;
using System.Linq;
using UnityEngine.Serialization;

public class SelectUI : MonoBehaviour
{
    public UltEvent<Selectable> OnValueChange;

    [field: SerializeField, FormerlySerializedAs("_id")] public string ID { get; private set; }
    public Selectable Value { get; private set; }

    public interface Selectable
    {
        string Label { get; }
        bool Equals(Selectable other);
    }

    [SerializeField] List<Selectable> _selectableValues;

    UIDocument _ui;
    VisualElement _root;
    VisualElement _left;
    VisualElement _right;
    Label _valueLabel;

    void OnEnable()
    {
        Init();
    }

    public void SetList(List<Selectable> list)
    {
        _selectableValues = list;
        Init();
    }

    public void SetValue(Selectable value)
    {
        Value = value;
        Init();
    }

    void Init()
    {
        _ui = GetComponent<UIDocument>();
        _root = _ui.rootVisualElement.Q(ID);
        _left = _root.Q("left");
        _right = _root.Q("right");
        _valueLabel = _root.Q<Label>("value");
        _valueLabel.text = Value?.Label ?? "";

        _root.UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
        _root.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
    }

    void OnNavigationMove(NavigationMoveEvent e)
    {
        if (e.direction == NavigationMoveEvent.Direction.Left)
        {
            SelectLeft();
        }
        else if (e.direction == NavigationMoveEvent.Direction.Right)
        {
            SelectRight();
        }
    }

    void SelectLeft()
    {
        var currentIndex = _selectableValues.FindIndex(selectable => selectable.Equals(Value));
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = _selectableValues.Count - 1;
        }

        Value = _selectableValues[currentIndex];
        Init();
        OnValueChange?.Invoke(Value);
    }

    void SelectRight()
    {
        var currentIndex = _selectableValues.FindIndex(selectable => selectable.Equals(Value));
        currentIndex++;

        if (currentIndex >= _selectableValues.Count)
        {
            currentIndex = 0;
        }

        Value = _selectableValues[currentIndex];
        Init();
        OnValueChange?.Invoke(Value);
    }
}
