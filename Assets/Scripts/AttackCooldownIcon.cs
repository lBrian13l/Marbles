using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AttackCooldownIcon : MonoBehaviour
{
    private Image _cooldownIcon;
    private PlayerController _player;
    [SerializeField] TMP_Text _cooldownTimer;
    private TimeSpan _timePassed;
    private bool _cooldownIsActive;
    private DateTime _startingTime;
    private float _attackCooldownInMilliseconds;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _cooldownIcon = transform.Find("Cooldown Icon").GetComponent<Image>();
        _attackCooldownInMilliseconds = _player.AttackCooldown * 1000;
    }

    void Update()
    {
        if (_cooldownIsActive)
        {
            _timePassed = DateTime.Now - _startingTime;
            _cooldownIcon.fillAmount = (_timePassed.Seconds * 1000 + _timePassed.Milliseconds) / _attackCooldownInMilliseconds;
            _cooldownTimer.text = Mathf.Ceil((_attackCooldownInMilliseconds - _timePassed.Seconds * 1000 - _timePassed.Milliseconds) / 1000f).ToString();
            if (_cooldownIcon.fillAmount >= 1)
            {
                _cooldownIsActive = false;
                _cooldownTimer.text = "M1";
            }
        }
    }

    public void Attacked()
    {
        _cooldownIsActive = true;
        _startingTime = DateTime.Now;
    }
}
