﻿using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames;

public static class MinigameHandler
{
    public static void TryCompleteMinigame(Minigame minigame, PlayerTask task)
    {
        GameObject coroutineObject = new("Minigame Solver");
        coroutineObject.transform.parent = minigame.transform;

        // DivertPowerMetagame doesn't run any logic by itself
        MonoBehaviour coroutineBehaviour = coroutineObject.AddComponent<DivertPowerMetagame>();
        coroutineBehaviour.StartCoroutine(CoTryCompleteMinigame(minigame, task));
    }

    private static IEnumerator CoTryCompleteMinigame(Minigame minigame, PlayerTask task)
    {
        if (!MinigameSolverAttribute.MinigameSolvers.TryGetValue(minigame.GetIl2CppType().FullName, out MinigameSolver solver))
        {
            Warning($"Cannot solve minigame of type {minigame.GetIl2CppType().FullName}");
            yield break;
        }

        InGameCursor.Instance.HideWhen(() => !minigame);

        yield return new WaitForSeconds(0.4f);
        yield return solver.CompleteMinigame(minigame, task);

        // By this point we expect the solver to have completed the minigame,
        // which means that it will close and be destroyed, so this coroutine
        // will not execute any code below.
    }
}
