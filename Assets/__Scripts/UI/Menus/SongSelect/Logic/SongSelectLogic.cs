using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SaturnGame.Data;
using SaturnGame.Loading;
using UnityEngine;

namespace SaturnGame.UI
{
public class SongSelectLogic : MonoBehaviour
{
    [SerializeField] private SongSelectCardAnimator cardAnimator;
    [SerializeField] private SongSelectPageAnimator pageAnimator;
    [SerializeField] private SongSelectDisplayAnimator displayAnimator;
    [SerializeField] private SongDatabase songDatabase;
    [SerializeField] private ButtonPageManager buttonManager;
    [SerializeField] private BgmPreviewController bgmPreview;

    private int SelectedSongIndex { get; set; }
    private int SelectedDifficulty { get; set; }

    private enum MenuPage
    {
        SongSelect = 0,
        ChartPreview = 1,
        ExitingMenu = 2,
    }

    private MenuPage page = MenuPage.SongSelect;

    [SerializeField] private GameObject diffPlusButton0;
    [SerializeField] private GameObject diffPlusButton1;
    [SerializeField] private GameObject diffMinusButton0;
    [SerializeField] private GameObject diffMinusButton1;
    private static UIAudioController UIAudio => UIAudioController.Instance;

    private void Start()
    {
        Debug.Log($"Coming from {SceneSwitcher.Instance.LastScene}");
        page = SceneSwitcher.Instance.LastScene == "_Options" ? MenuPage.ChartPreview : MenuPage.SongSelect;

        songDatabase.LoadAllSongData();

        SetSongAndDiffFromPersistentState();

        displayAnimator.SetSongData(songDatabase.Songs[SelectedSongIndex], SelectedDifficulty);

        // TODO: async
        LoadAllCards();
        SetBgmValues();

        if (page is MenuPage.ChartPreview)
        {
            buttonManager.SetActiveButtons(1);
            pageAnimator.ToChartPreviewInstant();
        }
    }

    private void SetSongAndDiffFromPersistentState()
    {
        int songIndex = 0;
        int difficultyIndex = 0;

        if (PersistentStateManager.Instance.SelectedSong.FolderPath is string path)
        {
            int foundIndex = songDatabase.Songs.FindIndex(song => song.FolderPath == path);
            // -1 indicates not found
            if (foundIndex != -1)
            {
                songIndex = foundIndex;
                // We aren't guaranteed that this difficulty still exists on this song, but SetSongAndDifficulty will
                // find the nearest difficulty in case this one no longer exists, so it should be fine.
                difficultyIndex = (int)PersistentStateManager.Instance.SelectedDifficulty.Difficulty;
            }
        }

        SetSongAndDifficulty(songIndex, difficultyIndex);
    }

    private void SetSongAndDifficulty(int songIndex, int difficultyIndex)
    {
        SelectedSongIndex = songIndex;
        PersistentStateManager.Instance.SelectedSong = songDatabase.Songs[SelectedSongIndex];
        // Always set difficulty after setting the song to avoid leaving difficultyIndex set to a value that is not
        // valid for the current song.
        SetDifficulty(difficultyIndex);
    }

    private void SetDifficulty(int difficultyIndex)
    {
        SongDifficulty[] diffs = songDatabase.Songs[SelectedSongIndex].SongDiffs;
        SongDifficulty nearestDiff = FindNearestDifficulty(diffs, difficultyIndex);
        SelectedDifficulty = (int)nearestDiff.Difficulty;
        PersistentStateManager.Instance.SelectedDifficulty = nearestDiff;

        diffPlusButton0.SetActive(HigherDiffExists(diffs, SelectedDifficulty));
        diffPlusButton1.SetActive(HigherDiffExists(diffs, SelectedDifficulty));
        diffMinusButton0.SetActive(LowerDiffExists(diffs, SelectedDifficulty));
        diffMinusButton1.SetActive(LowerDiffExists(diffs, SelectedDifficulty));

        displayAnimator.SetSongData(songDatabase.Songs[SelectedSongIndex], SelectedDifficulty);

        SetBgmValues();
    }


    public void OnDifficultyChange(int changeBy)
    {
        if (page == MenuPage.ExitingMenu) return;
        if (SelectedDifficulty + changeBy is < 0 or > 4) return;

        int prevDifficulty = SelectedDifficulty;
        SetDifficulty(SelectedDifficulty + changeBy);

        if (prevDifficulty == SelectedDifficulty) return;

        UIAudio.PlaySound(UIAudioController.UISound.Navigate);

        if (page is not MenuPage.ChartPreview) return;

        bgmPreview.FadeoutBgmPreview();
        bgmPreview.ResetLingerTimer();
    }

    public void OnBack()
    {
        if (page == MenuPage.ExitingMenu) return;
        UIAudio.PlaySound(UIAudioController.UISound.Back);

        switch (page)
        {
            case MenuPage.SongSelect:
            {
                page = MenuPage.ExitingMenu;
                bgmPreview.FadeoutBgmPreview(true);
                SceneSwitcher.Instance.LoadScene("_TitleScreen");
                break;
            }
            case MenuPage.ChartPreview:
            {
                page = MenuPage.SongSelect;

                pageAnimator.Anim_ToSongSelect();
                buttonManager.SwitchButtons(0);
                break;
            }
            case MenuPage.ExitingMenu:
            {
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void OnConfirm()
    {
        switch (page)
        {
            case MenuPage.ExitingMenu:
            {
                break;
            }
            case MenuPage.SongSelect:
            {
                UIAudio.PlaySound(UIAudioController.UISound.Impact);

                bgmPreview.FadeoutBgmPreview();
                bgmPreview.ResetLingerTimer();

                page = MenuPage.ChartPreview;
                pageAnimator.Anim_ToChartPreview();
                buttonManager.SwitchButtons(1);
                break;
            }
            case MenuPage.ChartPreview:
            {
                UIAudio.PlaySound(UIAudioController.UISound.StartGame);
                bgmPreview.FadeoutBgmPreview(true);
                page = MenuPage.ExitingMenu;

                SceneSwitcher.Instance.LoadScene("_RhythmGame");
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }


    private enum NavigateDirection
    {
        Left = -1,
        Right = +1,
    }

    private async Awaitable OnNavigateLeftRight(NavigateDirection direction)
    {
        if (page == MenuPage.ExitingMenu) return;
        if (page is not MenuPage.SongSelect) return;

        UIAudio.PlaySound(UIAudioController.UISound.Navigate);

        SetSongAndDifficulty(SaturnMath.Modulo(SelectedSongIndex + (int)direction, songDatabase.Songs.Count),
            SelectedDifficulty);

        // Update Cards
        SongSelectCardAnimator.ShiftDirection shiftDirection = direction switch
        {
            // If you move left, the cards shift right, and vice versa.
            NavigateDirection.Left => SongSelectCardAnimator.ShiftDirection.Right,
            NavigateDirection.Right => SongSelectCardAnimator.ShiftDirection.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
        };
        cardAnimator.Anim_ShiftCards(shiftDirection);
        int newSongIndex = SaturnMath.Modulo(SelectedSongIndex + cardAnimator.CardHalfCount * (int)direction,
            songDatabase.Songs.Count);
        Song newSong = songDatabase.Songs[newSongIndex];
        Awaitable loadNewJacket = LoadCardJacket(cardAnimator.WrapCardIndex, newSong.JacketPath);

        cardAnimator.SetSongData(cardAnimator.WrapCardIndex, SelectedDifficulty, newSong);

        cardAnimator.SetSelectedJacket(cardAnimator.GetCenterCardJacket());

        // Audio Preview
        bgmPreview.FadeoutBgmPreview();
        bgmPreview.ResetLingerTimer();

        // Await the jacket which has been loading in the background to make sure we rethrow any errors
        await loadNewJacket;
    }

    public async void OnNavigateLeft()
    {
        await OnNavigateLeftRight(NavigateDirection.Left);
    }


    public async void OnNavigateRight()
    {
        await OnNavigateLeftRight(NavigateDirection.Right);
    }

    public void OnSort()
    {
    }

    public void OnFavorite()
    {
    }

    public void OnOptions()
    {
        if (page == MenuPage.ExitingMenu) return;

        if (page != MenuPage.ChartPreview)
        {
            // nope sound
            UIAudio.PlaySound(UIAudioController.UISound.Back);
            return;
        }

        UIAudio.PlaySound(UIAudioController.UISound.Impact);
        bgmPreview.FadeoutBgmPreview(true);
        page = MenuPage.ExitingMenu;

        SceneSwitcher.Instance.LoadScene("_Options");
    }

    private async void LoadAllCards()
    {
        // The loop is async, so SelectedSongIndex can change. However, since all the cards are relative to the initial
        // values, we want to make sure we capture the initial value for the duration of the loop.
        int selectedIndex = SelectedSongIndex;
        List<Awaitable> awaitables = new();

        // Load song data and start loading all the jackets
        for (int i = 0; i < cardAnimator.SongCards.Count; i++)
        {
            int index = SaturnMath.Modulo(selectedIndex + i - cardAnimator.CardHalfCount,
                songDatabase.Songs.Count);
            Song data = songDatabase.Songs[index];
            string path = data.JacketPath;

            awaitables.Add(LoadCardJacket(i, path));
            cardAnimator.SetSongData(i, SelectedDifficulty, data);
        }

        // Make sure to await all the jacket loads
        foreach (Awaitable awaitable in awaitables) await awaitable;

        cardAnimator.SetSelectedJacket(cardAnimator.GetCenterCardJacket());
    }

    private async Awaitable LoadCardJacket(int cardIndex, string jacketPath)
    {
        cardAnimator.CurrentJacketPaths[cardIndex] = jacketPath;
        Texture2D jacket = await ImageLoader.LoadImageWebRequest(jacketPath);

        if (cardAnimator.CurrentJacketPaths[cardIndex] != jacketPath)
        {
            // A new jacket has been loaded to this card while we were waiting for this one to load. Abort.
            Destroy(jacket);
            return;
        }

        cardAnimator.SetCardJacket(cardIndex, jacket);

        if (cardAnimator.CenterCardIndex == cardIndex)
            cardAnimator.SetSelectedJacket(jacket);
    }

    // Important note: it is CALLER'S RESPONSIBILITY to ensure the provided diffs param has a SongDifficulty of
    // [Normal, Hard, Expert, Inferno, Beyond]. This method assumes that the index of the SongDifficulty in the list
    // matches the Difficulty enum value.
    private static SongDifficulty FindNearestDifficulty([NotNull] IList<SongDifficulty> diffs, int selectedIndex)
    {
        if (diffs[selectedIndex].Exists) return diffs[selectedIndex];

        int leftIndex = selectedIndex - 1;
        int rightIndex = selectedIndex + 1;

        while (leftIndex >= 0 || rightIndex < diffs.Count)
        {
            if (leftIndex >= 0 && diffs[leftIndex].Exists) return diffs[leftIndex];
            if (rightIndex < diffs.Count && diffs[rightIndex].Exists) return diffs[rightIndex];

            leftIndex--;
            rightIndex++;
        }

        // yikes, consider throwing
        return default;
    }

    private static bool HigherDiffExists([NotNull] IList<SongDifficulty> diffs, int selectedIndex)
    {
        for (int i = selectedIndex + 1; i < diffs.Count; i++)
        {
            if (diffs[i].Exists)
                return true;
        }

        return false;
    }

    private static bool LowerDiffExists([NotNull] IList<SongDifficulty> diffs, int selectedIndex)
    {
        for (int i = selectedIndex - 1; i >= 0; i--)
        {
            if (diffs[i].Exists)
                return true;
        }

        return false;
    }


    private void SetBgmValues()
    {
        string path = songDatabase.Songs[SelectedSongIndex].SongDiffs[SelectedDifficulty].AudioFilepath;
        float start = songDatabase.Songs[SelectedSongIndex].SongDiffs[SelectedDifficulty].PreviewStart;
        float duration = songDatabase.Songs[SelectedSongIndex].SongDiffs[SelectedDifficulty].PreviewDuration;
        bgmPreview.SetBgmValues(path, start, duration);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) OnNavigateLeft();
        if (Input.GetKeyDown(KeyCode.D)) OnNavigateRight();
        if (Input.GetKeyDown(KeyCode.Space)) OnConfirm();
        if (Input.GetKeyDown(KeyCode.Escape)) OnBack();
        if (Input.GetKeyDown(KeyCode.UpArrow)) OnDifficultyChange(+1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) OnDifficultyChange(-1);
        if (Input.GetKeyDown(KeyCode.O)) OnOptions();

        if (Input.GetKeyDown(KeyCode.X)) bgmPreview.FadeoutBgmPreview();
    }
}
}

// ==== Features ====================
// DiffPlus     -    Increase Difficulty
// DiffMinus    -    Decrease Difficulty
// NavLeft      -    Select song on the left
// NavRight     -    Select song on the right
// ToTitle      -    Title Screen
// ToPage0      -    Song Select
// ToPage1      -    Chart Preview
// ToRhythmGame -    Start Gameplay
// ToOptions    -    Options Menu
// Sort         -    Sort Popup + Sort Songs
// Favorite     -    Add song to favorites list
