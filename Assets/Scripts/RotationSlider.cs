using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RotationSlider : MonoBehaviour
{
    [SerializeField] private Slider _rotationSlider;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private GameConfig _gameConfig;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _valueText.text = _rotationSlider.value.ToString();
    }

    private void OnEnable()
    {
        _rotationSlider.value = _gameConfig.GetRotationSpeed();
    }

    private void OnDisable()
    {
        _gameConfig.SetRotationSpeed(_rotationSlider.value);
    }
}
