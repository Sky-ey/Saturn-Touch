using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaturnGame.RhythmGame
{
    [System.Serializable]
    public class ChartObject
    {
        public int Measure;
        [Range(0, 1919)] public int Tick;
    }

    public class ObjectEnums
    {
        public enum NoteType
        {
            None,
            Touch,
            SnapForward,
            SnapBackward,
            SwipeClockwise,
            SwipeCounterclockwise,
            HoldStart,
            HoldSegment,
            HoldEnd,
            Chain,
            MaskAdd,
            MaskRemove,
            EndChart
        }
        public enum BonusType
        {
            None,
            Bonus,
            R_Note
        }
        public enum GimmickType
        {
            None,
            Note,
            BeatsPerMinute,
            TimeSignature,
            HiSpeed,
            ReverseEffectStart,
            ReverseEffectEnd,
            ReverseNoteEnd,
            StopStart,
            StopEnd
        }
    }
}
