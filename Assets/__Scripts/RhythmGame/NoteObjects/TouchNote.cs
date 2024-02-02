using UnityEngine;

namespace SaturnGame.RhythmGame
{
    [System.Serializable]
    public class TouchNote : Note
    {
        public TouchNote(
            int measure,
            int tick,
            int position,
            int size,
            ObjectEnums.BonusType bonusType = ObjectEnums.BonusType.None,
            bool isSync = false
            ) : base(measure, tick, position, size, bonusType, isSync)
        {
        }

        public override ObjectEnums.NoteType NoteType => ObjectEnums.NoteType.Touch;
    }
}
