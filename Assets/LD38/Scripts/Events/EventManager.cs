using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static readonly EventCallBack<BaseObject> OnObjectChanged = new EventCallBack<BaseObject>();
    public static readonly EventCallBack<BaseObject> OnObjectDestroyed = new EventCallBack<BaseObject>();

    public static readonly EventCallBack<BaseObject> OnItemSpawned = new EventCallBack<BaseObject>();

    public static readonly EventCallBack<WorldSide> OnSideFlipStarted = new EventCallBack<WorldSide>();
    public static readonly EventCallBack<WorldSide> OnSideFlipFinished = new EventCallBack<WorldSide>();

    public static readonly EventCallBack OnWorldRotationStarted = new EventCallBack();
    public static readonly EventCallBack OnWorldRotationFinished = new EventCallBack();

    public static readonly EventCallBack OnViewRotateStarted = new EventCallBack();
    public static readonly EventCallBack OnViewRotateFinished = new EventCallBack();

    public static readonly EventCallBack OnLevelPopulationStarted = new EventCallBack();
    public static readonly EventCallBack OnLevelPopulationFinished = new EventCallBack();

    public static readonly EventCallBack OnGameStart = new EventCallBack();
    public static readonly EventCallBack OnGameOver = new EventCallBack();

    public static readonly EventCallBack OnMenuOpen = new EventCallBack();
    public static readonly EventCallBack OnMenuClose = new EventCallBack();
}
