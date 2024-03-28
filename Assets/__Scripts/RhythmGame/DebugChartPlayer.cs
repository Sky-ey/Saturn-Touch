using System.IO;
using SaturnGame.Data;
using SaturnGame.RhythmGame;
using UnityEngine;

public class DebugChartPlayer : MonoBehaviour
{
    [SerializeField] private ChartManager chartManager;
    [SerializeField] private string folderInSongPacks = "DONOTSHIP/";
    [SerializeField] private Difficulty difficulty;

    private async void Update()
    {
        string folderPath = Path.Combine(SongDatabase.SongPacksPath, folderInSongPacks, "meta.mer");
        if (Input.GetKeyDown(KeyCode.L))
        {
            Song song = SongDatabase.LoadSongData(folderPath);
            SongDifficulty songDiff = song.SongDiffs[(int)difficulty];
            PersistentStateManager.Instance.SelectedSong = song;
            PersistentStateManager.Instance.SelectedDifficulty = songDiff;
            await chartManager.LoadChart();
        }
    }
}
