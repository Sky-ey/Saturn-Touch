using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaturnGame.UI
{
    public class OptionsLogic : MonoBehaviour
    {
        [SerializeField] private OptionsPanelAnimator panelAnimator;
        [SerializeField] private UIScreen startScreen;
        private UIAudioController UIAudio => UIAudioController.Instance;

        private Stack<UIScreen> screenStack = new();
        private UIScreen currentScreen => screenStack.Peek();
        private Stack<int> indexStack = new(new List<int>{0});
        private int currentIndex
        {
            get => indexStack.Peek();
            set
            {
                indexStack.Pop();
                indexStack.Push(value);
            }
        }

        void Awake()
        {
            screenStack.Push(startScreen);
            panelAnimator.GetPanels(startScreen);
            panelAnimator.SetSelectedPanel(startScreen.ListItems[currentIndex]);
        }

        public async void OnConfirm()
        {   
            var nextScreen = currentScreen.ListItems[currentIndex].NextScreen;
            if (nextScreen == null) return;

            UIAudio.PlaySound(UIAudioController.UISound.Confirm);
            panelAnimator.Anim_HidePanels(currentScreen);

            screenStack.Push(nextScreen);
            indexStack.Push(0);

            await Awaitable.WaitForSecondsAsync(0.15f);
            panelAnimator.GetPanels(currentScreen);
            panelAnimator.SetSelectedPanel(currentScreen.ListItems[0]);
            panelAnimator.Anim_ShowPanels(currentScreen);
        }

        public async void OnBack()
        {
            UIAudio.PlaySound(UIAudioController.UISound.Back);

            if (screenStack.Count <= 1 || indexStack.Count <= 1)
            {
                panelAnimator.Anim_HidePanels(currentScreen);
                SceneSwitcher.Instance.LoadScene("_SongSelect");
                return;
            }

            panelAnimator.Anim_HidePanels(currentScreen);

            screenStack.Pop();
            indexStack.Pop();

            await Awaitable.WaitForSecondsAsync(0.15f);
            panelAnimator.GetPanels(currentScreen, currentIndex);
            panelAnimator.SetSelectedPanel(currentScreen.ListItems[currentIndex]);
            panelAnimator.Anim_ShowPanels(currentScreen);
        }

        public void OnNavigateLeft()
        {
            if (screenStack.Count == 0 || indexStack.Count == 0) return;

            int newIndex = Mathf.Max(currentIndex - 1, 0);
            if (currentIndex != newIndex)
            {
                UIAudio.PlaySound(UIAudioController.UISound.Navigate);
                currentIndex = newIndex;
            }

            panelAnimator.Anim_ShiftPanels(currentIndex, currentScreen);
            panelAnimator.SetSelectedPanel(currentScreen.ListItems[currentIndex]);
        }
        
        public void OnNavigateRight()
        {
            if (screenStack.Count == 0 || indexStack.Count == 0) return;

            int newIndex = Mathf.Min(currentIndex + 1, currentScreen.ListItems.Count - 1);
            if (currentIndex != newIndex)
            {
                UIAudio.PlaySound(UIAudioController.UISound.Navigate);
                currentIndex = newIndex;
            }

            panelAnimator.Anim_ShiftPanels(currentIndex, currentScreen);
            panelAnimator.SetSelectedPanel(currentScreen.ListItems[currentIndex]);
        }

        public void OnDefault() {}

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) OnNavigateLeft();
            if (Input.GetKeyDown(KeyCode.D)) OnNavigateRight();
            if (Input.GetKeyDown(KeyCode.Space)) OnConfirm();
            if (Input.GetKeyDown(KeyCode.Escape)) OnBack();
        }
    }
}
