using System;
using SaturnGame;
using SaturnGame.Data;
using SaturnGame.Loading;
using SaturnGame.RhythmGame;
using TMPro;
using UnityEngine;

public class SongResultsLogic : MonoBehaviour
{
	[SerializeField] private SongResultsAnimator animator;

    private async void Start()
    {
        Song song = PersistentStateManager.Instance.SelectedSong;
        Texture2D jacket = await ImageLoader.LoadImageWebRequest(song.JacketPath);
		SongDifficulty songDifficulty = PersistentStateManager.Instance.SelectedDifficultyInfo;
        ScoreData scoreData = PersistentStateManager.Instance.LastScoreData;
        animator.SetSongInfo(song.Title, song.Artist, songDifficulty.Level.ToString(), jacket, songDifficulty.Difficulty);
        int noteCount = scoreData.JudgementCounts.Total.Marvelous.Count + scoreData.JudgementCounts.Total.Great.Count + scoreData.JudgementCounts.Total.Good.Count + scoreData.JudgementCounts.Total.MissCount;
        animator.SetJudgementCount(noteCount,scoreData.JudgementCounts.Total.Marvelous.Count,scoreData.JudgementCounts.Total.Great.Count,scoreData.JudgementCounts.Total.Good.Count,scoreData.JudgementCounts.Total.MissCount, scoreData.JudgementCounts.Total.TotalEarlyLate.EarlyCount, scoreData.JudgementCounts.Total.TotalEarlyLate.LateCount,0);
		int performanceLevel =
			(scoreData.JudgementCounts.Total.Marvelous.Count == noteCount) ? 4 :
			(scoreData.JudgementCounts.Total.MissCount == 0) ? 3 :
			(scoreData.JudgementCounts.Total.MissCount <= 5) ? 2 : 1;
		animator.SetSpecialPerformance(performanceLevel);       // TODO Fail
		animator.SetDetailInfo(scoreData.JudgementCounts.Touch, scoreData.JudgementCounts.Chain, scoreData.JudgementCounts.Hold, scoreData.JudgementCounts.Swipe, scoreData.JudgementCounts.Snap);
		animator.SetScoreRank(GetScoreRank(scoreData.Score));
		if (scoreData.Score > scoreData.MaxScore)
		{
			animator.SetNewRecord(true);
		}
        await Awaitable.WaitForSecondsAsync(0.7f);
		animator.Anim_ShowResults();
		await Awaitable.WaitForSecondsAsync(1f);
		animator.AnimateScore(scoreData.Score);
		await Awaitable.WaitForSecondsAsync(1.5f);
        animator.Anim_ShouButton();
	}

    public void OnContinue()
    {
        SceneSwitcher.Instance.LoadScene("_SongSelect");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) OnContinue();
    }

	public int GetScoreRank(int score)
	{
		if (score >= 1000000)
			return 12; // MASTER
		else if (score >= 990000)
			return 11; // SSS+
		else if (score >= 980000)
			return 10; // SSS
		else if (score >= 970000)
			return 9;  // SS+
		else if (score >= 950000)
			return 8;  // SS
		else if (score >= 930000)
			return 7;  // S+
		else if (score >= 900000)
			return 6;  // S
		else if (score >= 850000)
			return 5;  // AAA
		else if (score >= 800000)
			return 4;  // AA
		else if (score >= 700000)
			return 3;  // A
		else if (score >= 10000)
			return 2;  // B
		else if (score >= 1)
			return 1;  // C
		else
			return 0;  // D
	}

}
