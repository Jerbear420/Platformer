using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthGui : MonoBehaviour
{
    [SerializeField] private GameObject _healthIcon;

    private RectTransform[] _currentHealth;

    private Player _player;
    private bool _registered;


    void Start()
    {
        _player = transform.parent.parent.gameObject.GetComponent<Player>();

        _currentHealth = new RectTransform[(int)_player.Health.MaxHealth];
        int column = 1;
        int row = 1;
        float columnGap = .25f;
        float rowGap = .25f;
        int maxColumns = 6;
        for (int i = 0; i < _player.Health.MaxHealth; i++)
        {
            GameObject health = Instantiate(_healthIcon);
            health.transform.SetParent(transform);
            RectTransform rect = health.GetComponent<RectTransform>();
            _currentHealth[i] = rect;
            rect.anchoredPosition = new Vector2(rect.rect.width * (column - 1) + (columnGap), -rect.rect.height * (row - 1) - (rowGap));
            column++;
            if (column >= maxColumns)
            {
                row++;
                column = 1;
            }
        }

        Refresh();
        if (!_registered)
        {
            OnEnable();
        }
    }

    void OnEnable()
    {
        if (_player != null)
        {
            _player.Health.OnHurt += Refresh;
            _registered = true;
        }
        else
        {
            _registered = false;
        }
    }

    void OnDisable()
    {
        _player.Health.OnHurt -= Refresh;
        _registered = false;
    }

    private void Refresh()
    {
        Debug.Log("Refresh!");
        for (int i = 0; i < _player.Health.MaxHealth; i++)
        {
            var remainder = _player.Health.CurrentHealth - Mathf.Floor(_player.Health.CurrentHealth);
            if (Mathf.Floor(_player.Health.CurrentHealth) <= i)
            {
                _currentHealth[i].gameObject.SetActive(false);
            }
            else
            {
                _currentHealth[i].gameObject.SetActive(true);
            }
            if (remainder > 0f)
            {
                Debug.Log("has a remainder");
            }
        }

    }

}
