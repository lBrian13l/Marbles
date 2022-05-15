using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGlobalSubscriber
{

}

public interface IGameOverHandler : IGlobalSubscriber
{
    void HandleGameOver();
}

public interface IOnGameOverHandler : IGlobalSubscriber
{
    void HandleOnGameOver();
}

public interface IEnemyDiedHandler : IGlobalSubscriber
{
    void HandleEnemyDied(GameObject enemy, bool powerupIsActive);
}

public interface IGemCollectedHandler : IGlobalSubscriber
{
    void HandleGemCollected(GameObject gem);
}

public interface IFinishWaveHandler : IGlobalSubscriber
{
    void HandleFinishWave();
}

public interface IPlayerSpawnedHandler : IGlobalSubscriber
{
    void HandlePlayerSpawned(GameObject player);
}

public interface IOnVictoryHandler : IGlobalSubscriber
{
    void HandleOnVictory();
}
