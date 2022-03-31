using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCountSlider : MonoBehaviour
{
    [SerializeField] private Slider _countSlider;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private GameConfig _gameConfig;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _valueText.text = _countSlider.value.ToString();
    }

    private void OnEnable()
    {
        _countSlider.value = _gameConfig.GetEnemyCount();
    }

    private void OnDisable()
    {
        _gameConfig.SetEnemyCount((int)_countSlider.value);
    }
}
