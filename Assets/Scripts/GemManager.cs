using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class GemManager : MonoBehaviour
{
    private List<Gem> _gems = new List<Gem>();

    public event Action GemsCollected;
    public event Action NeedToEnableIndicator;

    public void SetGems(List<GameObject> gemObjects)
    {
        foreach (GameObject gemObject in gemObjects)
        {
            if (gemObject.TryGetComponent(out Gem gem))
            {
                gem.GemCollected += OnGemCollected;
                _gems.Add(gem);
            }
        }
    }

    private void OnGemCollected(Gem gem)
    {
        RemoveGem(gem);
    }

    private void RemoveGem(Gem gem)
    {
        if (_gems.Contains(gem))
        {
            gem.GemCollected -= OnGemCollected;
            _gems.Remove(gem);
            Destroy(gem.gameObject);
        }

        CheckGems();
    }

    public void RemoveRandomGem()
    {
        if (_gems.Count <= 0)
            return;
        else if (_gems.Count == 1)
            NeedToEnableIndicator?.Invoke();

        Gem randomGem = _gems[Random.Range(0, _gems.Count)];
        RemoveGem(randomGem);
    }

    private void CheckGems()
    {
        if (_gems.Count <= 0)
        {
            GemsCollected?.Invoke();
        }
    }
}
