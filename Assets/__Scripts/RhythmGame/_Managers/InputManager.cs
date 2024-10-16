using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SaturnGame.Settings;
using UnityEngine;

namespace SaturnGame.RhythmGame
{
/// <summary>
/// InputManager handles **gameplay** inputs - reading from an <see cref="IInputProvider"/> such as <see
/// cref="TouchRingManager"/> or <see cref="keyboardInput"/>, or from a replay (via <see cref="ReplayManager"/>).
/// It pulls current input from these providers, associates it with the current gameplay time, buffers the input if
/// artificial latency is active, and then processes the input by calling into <see cref="scoringManager"/>.
/// </summary>
public class InputManager : Singleton<InputManager>, IInputProvider
{
    public enum InputSource
    {
        TouchRing,
        Keyboard,
        Replay,
    }

    public InputSource CurrentInputSource;

    [CanBeNull]
    private IInputProvider CurrentInputProvider => CurrentInputSource switch
    {
        InputSource.Keyboard => keyboardInput,
        InputSource.TouchRing => TouchRingManager.Instance,
        InputSource.Replay => null, // ReplayManager is not an IInputProvider.
        _ => throw new System.NotImplementedException(),
    };

    private static float LatencyMs => SettingsManager.Instance.PlayerSettings.GameSettings.CalculatedInputLatencyMs;

    private TimedTouchStateQueue queue;

    [Header("MANAGERS")] [SerializeField] private ScoringManager scoringManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private ReplayManager replayManager;

    private readonly KeyboardInput keyboardInput = new();

    public TouchState CurrentTouchState = TouchState.CreateNew();
    public TouchState GetCurrentTouchState() => CurrentTouchState;

    private void Start()
    {
        int queueSize = (int)LatencyMs switch
        {
            0 => 1,
            var n => n, // Queue size should be sufficient up to 1000fps (1ms per frame).
        };
        queue = new(queueSize);
    }


    // Warning: the provided TouchState's underlying data is not guaranteed to be valid past the end of this function's
    // invocation. A persisted reference to the TouchState may not behave as expected. Use TouchState.Copy or .CopyTo.
    // See docs on TouchState.
    private void HandleNewTouchState(TouchState? touchState, float timeMs)
    {
        if (touchState is null || touchState.Value.EqualsSegments(CurrentTouchState))
        {
            scoringManager.HandleInput(null, timeMs);
            // Don't write to replay.
            return;
        }

        if (replayManager != null && !replayManager.PlayingFromReplay)
            replayManager.RecordFrame(touchState.Value, timeMs);
        touchState.Value.CopyTo(ref CurrentTouchState);
        scoringManager.HandleInput(touchState.Value, timeMs);
    }


    /// <summary>
    /// Get touch states up to a given timeMs, INCLUSIVE.
    /// (n.b. float equality is usually not exact, but it's possible that the TimeMs in the queue exactly matches the
    /// current timeMs seen by the InputManager, e.g. if they are both happening on the same frame.)
    /// It can be assumed that the consumer will iterate through the IEnumerable to completion.
    /// </summary>
    /// <param name="timeMs"></param>
    /// <returns></returns>
    private IEnumerable<TimedTouchState> GetTimedTouchStatesUntil(float timeMs)
    {
        return CurrentInputSource switch
        {
            InputSource.Replay => replayManager.GetTimedTouchStatesUntil(timeMs),
            InputSource.TouchRing or InputSource.Keyboard => queue.GetTimedTouchStatesUntil(timeMs),
            _ => throw new System.NotImplementedException(),
        };
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Backslash))
        {
            Debug.Log("Switched to keyboard input.");
            CurrentInputSource = InputSource.Keyboard;
            return;
        }

        if (Input.GetKeyDown(KeyCode.F11) && replayManager != null)
        {
            await replayManager.ReadReplayFile();
            Debug.Log("Loaded replay file, switching to replay input.");
            CurrentInputSource = InputSource.Replay;
            return;
        }

        if (timeManager.State != TimeManager.SongState.Playing) return;

        // Get the current input and queue it.
        switch (CurrentInputProvider?.GetCurrentTouchState())
        {
            case TouchState touchState:
            {
                queue.Enqueue(touchState, timeManager.GameplayTimeMs + LatencyMs);
                break;
            }
            case null:
            {
                // Current input method doesn't use an IInputProvider, e.g. Replay.
                break;
            }
        }

        // Actually handle inputs.
        int handledStates = 0;
        foreach (TimedTouchState timedTouchState in GetTimedTouchStatesUntil(timeManager.GameplayTimeMs))
        {
            handledStates++;
            HandleNewTouchState(timedTouchState.TouchState, timedTouchState.TimeMs);
        }

        if (handledStates == 0)
        {
            // Call HandleNewTouchState with a null touch state so that ScoringManager will update state for the current
            // time. This is idempotent for the ultimate state but will allow certain scoring info like hold notes or
            // passing chain notes to be judged now instead of waiting for the next input.
            HandleNewTouchState(null, timeManager.GameplayTimeMs);
        }
    }
}

public struct TimedTouchState
{
    public TimedTouchState(TouchState touchState, float timeMs)
    {
        TouchState = touchState;
        TimeMs = timeMs;
    }

    public TouchState TouchState;
    public readonly float TimeMs;
}

// Abstract class for frame-synchronized input sources. (Not replays.) If/when we move to a multi-threaded model, this
// will need to be reworked.
public interface IInputProvider
{
    // Get the TouchState as of this frame. The TouchState is only guaranteed to live until the end of the frame, or
    // possibly until the next time GetCurrentTouchState() is called.
    public TouchState GetCurrentTouchState();
}

public class KeyboardInput : IInputProvider
{
    private TouchState currentTouchState = TouchState.CreateNew();

    private static void ReadFromKeyboard(bool[,] segments)
    {
        Debug.Log("ReadCalled！");
        for (int i = 0; i < 60; i++)
        for (int j = 0; j < 4; j++)
            segments[i, j] = false;
        Debug.Log("Get " + Input.touchCount + " touchs!");
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 touchPosition = touch.position;

            // 假设圆心在屏幕中心
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            // 计算触控点相对于圆心的偏移量
            Vector2 offset = touchPosition - screenCenter;

            // 计算半径
            float radius = offset.magnitude;

            // 计算角度（以弧度为单位）
            float angle = Mathf.Atan2(offset.y, offset.x);

            // 将角度转换为0到2PI之间的正值
            if (angle < 0)
            {
                angle += 2 * Mathf.PI;
            }

            // 计算圈数（假设最大半径为屏幕宽度，分为4圈）
            float maxRadius = Screen.width / 2f; // 最大半径为屏幕宽度的一半
            int circleIndex = Mathf.Clamp((int)(radius / (maxRadius / 4)), 0, 3);

            // 计算块索引（整圈60个块）
            int blockIndex = (int)((angle / (2 * Mathf.PI)) * 60) % 60;

            // 将对应的段标记为true
            segments[blockIndex, circleIndex] = true;
            Debug.Log("Touch " + i + "Segment:" + blockIndex +","+ circleIndex);
        }
    }

    public TouchState GetCurrentTouchState()
    {
        TouchState.StealAndUpdateSegments(ref currentTouchState, ReadFromKeyboard);
        return currentTouchState;
    }
}
}
