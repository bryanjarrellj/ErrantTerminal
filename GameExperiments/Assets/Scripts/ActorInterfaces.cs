using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is a basic interface with a single required
//method.
public interface IGameActor {
    void TakeMessage(string message);
}

public interface IEnemyActor {
    EnemyStatus getStatus();

    void ActivateEnemy();

    void DestroySelf(string message);

    Bounds GetBounds();

    bool BoxFits(Bounds killBounds);
}

public interface IColorActor {
    bool CorrectColor(SpriteRenderer a, SpriteRenderer b);
}