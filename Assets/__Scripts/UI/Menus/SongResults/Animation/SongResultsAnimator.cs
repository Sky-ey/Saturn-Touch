using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;
using SaturnGame.RhythmGame;

namespace SaturnGame
{
    public class SongResultsAnimator : MonoBehaviour
    {
        [SerializeField] private RawImage rawImgJacket;
        [SerializeField] private Image imgDiffLabel;
        [SerializeField] private Image imgBackgroundBlur;
        
        [SerializeField] private TextMeshProUGUI tmpTitle;
        [SerializeField] private TextMeshProUGUI tmpArtist;
        [SerializeField] private TextMeshProUGUI tmpLevel;
        
        [SerializeField] private TextMeshProUGUI tmpMarvelousCount;
        [SerializeField] private TextMeshProUGUI tmpGreatCount;
        [SerializeField] private TextMeshProUGUI tmpGoodCount;
        [SerializeField] private TextMeshProUGUI tmpMissCount;
        
        [SerializeField] private TextMeshProUGUI tmpMarvelousRNoteCount;
        [SerializeField] private TextMeshProUGUI tmpGreatRNoteCount;
        [SerializeField] private TextMeshProUGUI tmpGoodRNoteCount;
        [SerializeField] private TextMeshProUGUI tmpMissRNoteCount;
        
        [SerializeField] private TextMeshProUGUI tmpMarvelousPercentage;
        [SerializeField] private TextMeshProUGUI tmpGreatPercentage;
        [SerializeField] private TextMeshProUGUI tmpGoodPercentage;
        [SerializeField] private TextMeshProUGUI tmpMissPercentage;

		[SerializeField] private TextMeshProUGUI tmpTouchDetail;
		[SerializeField] private TextMeshProUGUI tmpChainDetail;
		[SerializeField] private TextMeshProUGUI tmpHoldDetail;
		[SerializeField] private TextMeshProUGUI tmpSwipeDetail;
		[SerializeField] private TextMeshProUGUI tmpSnapDetail;

		[SerializeField] private TextMeshProUGUI tmpFastCount;
        [SerializeField] private TextMeshProUGUI tmpLateCount;
        
        [SerializeField] private TextMeshProUGUI tmpCombo;
        [SerializeField] private TextMeshProUGUI tmpScore;

        [SerializeField] private GameObject tmpNewRecord;

        [SerializeField] private List<GameObject> tmpDiffLabels;
        [SerializeField] private List<GameObject> performanceIcons;
        [SerializeField] private List<GameObject> scoreRanks;
        private readonly List<Color> diffColors = new()
        {
            new(0.1019f, 0.4823f, 1.0000f, 1.0000f),
            new(1.0000f, 0.7647f, 0.0000f, 1.0000f),
            new(1.0000f, 0.0000f, 0.5176f, 1.0000f),
            new(0.2509f, 0.0000f, 0.2627f, 1.0000f),
            new(0.0000f, 0.0000f, 0.0000f, 1.0000f),
        };
        
        [SerializeField] private GameObject tmpFailed;
        [SerializeField] private GameObject tmpClear;
        [SerializeField] private GameObject tmpMissless;
        [SerializeField] private GameObject tmpFullCombo;
        [SerializeField] private GameObject tmpAllMarvelous;

        [SerializeField] private RectTransform rectLeftPanel;
        [SerializeField] private RectTransform rectResults;
        [SerializeField] private RectTransform rectDetails;
        [SerializeField] private RectTransform rectConfirmButton;
        [SerializeField] private RectTransform rectDetailButton;
        [SerializeField] private RectTransform rectProfile;
        
        [SerializeField] private float animDuration = 0.5f;
        [SerializeField] private Ease animEase = Ease.OutQuad;

        private async void Start()
        {
            rectLeftPanel.anchoredPosition = new(-1500, 0);
            rectResults.anchoredPosition = new(-1500, 0);
            rectDetails.anchoredPosition = new(-1500, 0);
            rectConfirmButton.anchoredPosition = new(0, -550);
			rectDetailButton.anchoredPosition = new(0, -550);
			rectProfile.anchoredPosition = new(0, 230);
			imgBackgroundBlur.DOFade(0, 0);
		}

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Anim_ShowResults();
            if (Input.GetKeyDown(KeyCode.Alpha2)) Anim_HideResults();
            if (Input.GetKeyDown(KeyCode.Alpha3)) AnimateScore(1000000);
            if (Input.GetKeyDown(KeyCode.Alpha4)) Anim_ShowDetails();
            if (Input.GetKeyDown(KeyCode.Alpha5)) Anim_HideDetails();
            if (Input.GetKeyDown(KeyCode.Alpha6)) Anim_ShouButton();
        }

        public void Anim_ShowResults()
        {
            rectProfile.anchoredPosition = new(0, 230);
            rectLeftPanel.anchoredPosition = new(-1500, 0);
            rectResults.anchoredPosition = new(-1500, -10);

            rectLeftPanel.DOAnchorPosX(0, animDuration).SetEase(animEase);
            rectResults.DOAnchorPosX(0, animDuration).SetEase(animEase);
            rectProfile.DOAnchorPosY(-8, animDuration).SetEase(animEase);
        }

        public void Anim_ShouButton()
        {
			rectConfirmButton.anchoredPosition = new(0, -550);
			rectDetailButton.anchoredPosition = new(0, -550);

			rectConfirmButton.DOAnchorPosY(-400,animDuration).SetEase(animEase);
			rectDetailButton.DOAnchorPosY(-450,animDuration).SetEase(animEase);
		}
        
        public void Anim_HideResults()
        {
            rectLeftPanel.anchoredPosition = new(0, 0);
            rectResults.anchoredPosition = new(0, 0);

            rectLeftPanel.DOAnchorPosX(-1500, animDuration).SetEase(animEase);
            rectResults.DOAnchorPosX(-1500, animDuration).SetEase(animEase);
            
            rectDetails.DOAnchorPosX(-1500, animDuration).SetEase(animEase);
            imgBackgroundBlur.DOFade(0, animDuration * 0.5f).SetEase(animEase);
        }

        public void Anim_ShowDetails()
        {
            rectDetails.anchoredPosition = new(-1500, 0);
            imgBackgroundBlur.DOFade(0, 0);
            
            rectDetails.DOAnchorPosX(0, animDuration).SetEase(animEase);
            imgBackgroundBlur.DOFade(1, animDuration * 0.5f).SetEase(animEase);
        }

        public void Anim_HideDetails()
        {
            rectDetails.anchoredPosition = new(0, 0);
            imgBackgroundBlur.DOFade(1, 0);
            
            rectDetails.DOAnchorPosX(-1500, animDuration).SetEase(animEase);
            imgBackgroundBlur.DOFade(0, animDuration * 0.5f).SetEase(animEase);
        }

        private bool detailStatus = false;
        public void SwitchDetail()
        {
            if (detailStatus)
            {
                Anim_HideDetails();
                detailStatus = false;
            } else
            {
                Anim_ShowDetails();
                detailStatus = true;
            }
        }

        public void SetSongInfo(string title, string artist, string level, [CanBeNull] Texture2D jacketTexture, Difficulty diff)
        {
            tmpTitle.text = title;
            tmpArtist.text = artist;
            tmpLevel.text = level;

            if (jacketTexture != null) rawImgJacket.texture = jacketTexture;

            foreach (GameObject label in tmpDiffLabels)
            {
                label.SetActive(false);
            }

            tmpDiffLabels[(int)diff].SetActive(true);
            imgDiffLabel.color = diffColors[(int)diff];
        }
        
        public void SetJudgementCount(int totalNotes, int marvelous, int great, int good, int miss, int fastCount, int lateCount, int maxCombo)
        {
			tmpFastCount.text = fastCount.ToString();
			tmpLateCount.text = lateCount.ToString();
			tmpCombo.text = maxCombo.ToString();

			tmpMarvelousCount.text = $"<mspace=0.75em>{marvelous:0000}</mspace>";
            tmpGreatCount.text = $"<mspace=0.75em>{great:0000}</mspace>";
            tmpGoodCount.text = $"<mspace=0.75em>{good:0000}</mspace>";
            tmpMissCount.text = $"<mspace=0.75em>{miss:0000}</mspace>";

            float marvelousPercentage = 100.0f * marvelous / totalNotes;
            float greatPercentage = 100.0f * great / totalNotes;
            float goodPercentage = 100.0f * good / totalNotes;
            float missPercentage = 100.0f * miss / totalNotes;
            
            tmpMarvelousPercentage.text = $"<mspace=0.84em>{marvelousPercentage:000}</mspace>.<mspace=1em>{(marvelousPercentage - (int)marvelousPercentage) * 10:0}%</mspace>";
            tmpGreatPercentage.text = $"<mspace=0.84em>{greatPercentage:000}</mspace>.<mspace=1em>{(greatPercentage - (int)greatPercentage) * 10:0}%</mspace>";
            tmpGoodPercentage.text = $"<mspace=0.84em>{goodPercentage:000}</mspace>.<mspace=1em>{(goodPercentage - (int)goodPercentage) * 10:0}%</mspace>";
            tmpMissPercentage.text = $"<mspace=0.84em>{missPercentage:000}</mspace>.<mspace=1em>{(missPercentage - (int)missPercentage) * 10:0}%</mspace>";
        }

		public void SetDetailInfo(JudgementCountTableRow touch, JudgementCountTableChainRow chain, JudgementCountTableRow hold, JudgementCountTableRow swipe, JudgementCountTableRow snape)
		{
            tmpTouchDetail.text = $"<mspace=0.75em>{touch.Marvelous.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{touch.Great.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{touch.Good.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{touch.MissCount:0000}</mspace>";
            tmpChainDetail.text = $"<mspace=0.75em>{chain.MarvelousCount:0000}</mspace><space=0.65em/><mspace=0.75em>0000</mspace><space=0.65em/><mspace=0.75em>0000</mspace><space=0.65em/><mspace=0.75em>{chain.MissCount:0000}</mspace>";
            tmpHoldDetail.text = $"<mspace=0.75em>{hold.Marvelous.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{hold.Great.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{hold.Good.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{hold.MissCount:0000}</mspace>";
            tmpSwipeDetail.text = $"<mspace=0.75em>{swipe.Marvelous.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{swipe.Great.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{swipe.Good.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{swipe.MissCount:0000}</mspace>";
            tmpSnapDetail.text = $"<mspace=0.75em>{snape.Marvelous.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{snape.Great.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{snape.Good.Count:0000}</mspace><space=0.65em/><mspace=0.75em>{snape.MissCount:0000}</mspace>";
		}

		public async void AnimateScore(int score)
        {
            int counter = 0;
            
            while (counter < score)
            {
                counter = Mathf.Min(counter + 1111, score);
                tmpScore.text = $"<mspace=0.765em>{counter:0000000}</mspace>";
                await Awaitable.WaitForSecondsAsync(0.001f);
            }
        }

        // Special performance type
        // 0: Fail  1: Clear  2: Missless  3: FullCombo  4: AllMarvelous
        public void SetSpecialPerformance(int type)
        {
			Debug.Log("Set SpecialPerformance: " + type);
			foreach (GameObject o in performanceIcons)
            {
                o.SetActive(false);
            }
			performanceIcons[type].SetActive(true);
        }

		public void SetScoreRank(int type)
		{
			Debug.Log("Set ScoreRan: " + type);
			foreach (GameObject o in scoreRanks)
			{
				o.SetActive(false);
			}
			scoreRanks[type].SetActive(true);
		}

        public void SetNewRecord(bool n)
        {
            Debug.Log("Set NewRecord!");
            tmpNewRecord.SetActive(n);
        }
	}
}
