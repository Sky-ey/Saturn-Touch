using DG.Tweening;
using SaturnGame.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SaturnGame.UI
{
    // TODO: A LOT OF CLEANUP

    public class OptionsPanelAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject linearPanelGroup;
        [SerializeField] private GameObject radialPanelGroup;

        [SerializeField] private List<OptionPanelLinear> linearPanels;
        [SerializeField] private List<OptionPanelRadial> radialPanels;

        [SerializeField] private OptionPanelPrimary primaryPanel;
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private RectTransform panelGroupRect;
        [SerializeField] private RectTransform gradientRect;
        [SerializeField] private RectTransform spinnyThingRect;
        [SerializeField] private RectTransform headerRect;
        [SerializeField] private RectTransform navigatorRect;
        [SerializeField] private RectMask2D panelMask;
        [SerializeField] private Image glassImage;
        [SerializeField] private Image radialCenterImage;
        [SerializeField] private GameObject radialCoverRing;
        [SerializeField] private GameObject radialCoverBackground;

        private Sequence currentSequence;

        private float[] angles = { -99, -81, -63, -45, -27, 0, 27, 45, 63, 81, 99, 117, 135, 153, 171, 189, 207, 225 };
        private Vector2 centerPoint = new(0, 0);

        public int LinearCenterIndex { get; private set; } = 0;
        public int LinearWrapIndex { get; private set; } = 0;
        public int LinearHalfCount => (int)(linearPanels.Count * 0.5f);
        public int RadialHalfCount => (int)(radialPanels.Count * 0.5f);

        public enum MoveDirection { Up = -1, Down = 1 }

        public void Anim_ShiftPanels(MoveDirection direction, int currentIndex, UIScreen screen)
        {
            const float duration = 0.05f;
            Ease ease = Ease.Linear;

            if (screen.ScreenType is UIScreen.UIScreenType.Radial) animateRadial();
            else animateLinear();

            void animateLinear()
            {
                LinearCenterIndex = SaturnMath.Modulo(LinearCenterIndex + (int)direction, linearPanels.Count);
                LinearWrapIndex = SaturnMath.Modulo(LinearCenterIndex + LinearHalfCount * (int)direction, linearPanels.Count);

                for (int i = 0; i < linearPanels.Count; i++)
                {
                    if (i >= screen.ListItems.Count) break;

                    var panel = linearPanels[i];
                    
                    if (screen.ListItems.Count > linearPanels.Count)
                    {
                        int index = SaturnMath.Modulo(LinearHalfCount - LinearCenterIndex + i, linearPanels.Count);
                        bool snap = i == LinearWrapIndex;

                        Vector2 position = GetLinearPosition(index);
                        float scale = GetLinearScale(index);
                        float snapDuration = snap ? 0 : duration;

                        panel.rect.DOAnchorPos(position, snapDuration).SetEase(ease);
                        panel.rect.DOScale(scale, snapDuration).SetEase(ease);

                        
                        int newItemIndex = currentIndex + LinearHalfCount * (int)direction;

                        if (!snap) continue;

                        if (newItemIndex < 0 || newItemIndex >= screen.ListItems.Count)
                        {
                            panel.gameObject.SetActive(false);
                        }
                        else
                        {
                            panel.gameObject.SetActive(true);
                            SetPanelLinear(screen, screen.ListItems[newItemIndex], panel);
                        }
                    }
                    else
                    {
                        int index = Mathf.Clamp(LinearHalfCount - LinearCenterIndex + i, 0, 6);

                        Vector2 position = GetLinearPosition(index);
                        float scale = GetLinearScale(index);

                        panel.rect.DOAnchorPos(position, duration).SetEase(ease);
                        panel.rect.DOScale(scale, duration).SetEase(ease);
                    }
                }
            }

            void animateRadial()
            {
                for (int i = 0; i < radialPanels.Count; i++)
                {
                    // Will handle radial soon.
                }
            }

            /*for (int i = 0; i < panelPool.ActiveObjects.Count; i++)
            {
                

                if (screen.ScreenType is not UIScreen.UIScreenType.Radial)
                {
                    int index = Mathf.Clamp(Mathf.Abs(distance), 0, 3);
                    Vector2 position = new(positionsX[index], sign * positionsY[index]);
                    float scale = scales[index];

                    panel.gameObject.SetActive(Mathf.Abs(distance) >= 0 && Mathf.Abs(distance) <= 3);
                    panel.rect.DOAnchorPos(position, duration).SetEase(ease);
                    panel.rect.DOScale(scale, duration).SetEase(ease);
                }

                if (screen.ScreenType is UIScreen.UIScreenType.Radial)
                {
                    panel.rect.anchoredPosition = centerPoint;
                    panel.rect.localScale = Vector3.one;

                    int index = Mathf.Clamp(distance + 5, 0, 17);
                    float angle = angles[index];

                    panel.gameObject.SetActive(distance + 5 >= 0 && distance + 5 <= 17);
                    panel.rect.DORotate(new Vector3(0, 0, angle), duration, RotateMode.Fast).SetEase(ease);
                }
            }*/
        }

        public void Anim_ShowPanels(UIScreen previous, UIScreen next)
        {
            bool prevLinear = previous.ScreenType is UIScreen.UIScreenType.LinearSimple or UIScreen.UIScreenType.LinearDetailed;
            bool nextLinear = next.ScreenType is UIScreen.UIScreenType.LinearSimple or UIScreen.UIScreenType.LinearDetailed;

            if (!nextLinear)
            {
                Anim_ShowPanelsRadial();
                return;
            }

            if (prevLinear)
            {
                Anim_ShowPanelsLinearPartial();
            }
            else
            {
                Anim_ShowPanelsLinearFull();
            }

            // Linear -> Linear => Partial
            // Linear -> Radial => Radial
            // Radial -> Linear => Full
            // Radial -> Radial => Radial
        }

        public void Anim_HidePanels(UIScreen previous, UIScreen next)
        {
            bool prevLinear = previous.ScreenType is UIScreen.UIScreenType.LinearSimple or UIScreen.UIScreenType.LinearDetailed;
            bool nextLinear = next.ScreenType is UIScreen.UIScreenType.LinearSimple or UIScreen.UIScreenType.LinearDetailed;

            if (!prevLinear)
            {
                Anim_HidePanelsRadial();
                return;
            }
            
            if (nextLinear)
            {
                Anim_HidePanelsLinearPartial();
            }
            else
            {
                Anim_HidePanelsLinearFull();
            }

            // Linear -> Linear => Partial
            // Linear -> Radial => Full
            // Radial -> Linear => Radial
            // Radial -> Radial => Radial
        }


        public void Anim_ShowPanelsLinearPartial()
        {
            // 1 frame = 32ms
            // Move panels 6 frames OutQuad
            // Fade Panels 6 frames OutQuad

            const float frame = 0.032f;
            currentSequence.Kill(true);

            panelMask.enabled = true;
            radialCenterImage.gameObject.SetActive(false);
            radialCoverRing.SetActive(false);
            radialCoverBackground.SetActive(false);

            panelGroup.alpha = 0;
            panelGroupRect.anchoredPosition = new(-250, 0);
            panelGroupRect.eulerAngles = new(0, 0, 0);

            currentSequence = DOTween.Sequence();
            currentSequence.Join(panelGroup.DOFade(1, frame * 6).SetEase(Ease.OutQuad));
            currentSequence.Join(panelGroupRect.DOAnchorPosX(0, frame * 6).SetEase(Ease.OutQuad));
        }

        public void Anim_HidePanelsLinearPartial()
        {
            // 1 frame = 32ms
            // Move panels 4 frames InQuad

            // wait 2 frames
            // Fade panels out 2 frames Linear

            const float frame = 0.032f;
            currentSequence.Kill(true);

            panelMask.enabled = true;
            radialCenterImage.gameObject.SetActive(false);
            radialCoverRing.SetActive(false);
            radialCoverBackground.SetActive(false);

            panelGroupRect.anchoredPosition = new(0, 0);
            panelGroupRect.eulerAngles = new(0, 0, 0);
            panelGroup.alpha = 1;

            currentSequence = DOTween.Sequence();
            currentSequence.Join(panelGroupRect.DOAnchorPosX(-250, frame * 4).SetEase(Ease.InQuad));
            currentSequence.Insert(frame * 2, panelGroup.DOFade(0, frame * 2).SetEase(Ease.Linear));
        }


        public void Anim_ShowPanelsLinearFull()
        {
            // 1 frame = 32ms
            // Move panels 6 frames OutQuad
            // Fade Panels 6 frames OutQuad
            // Move Navigator 6 frames OutQuad

            // wait 2 frames
            // Scale Spinnything 4 frames OutQuad
            // Scale glass 4 frames OutQuad
            // Fade Glass 4 frames OutQuad
            // Move Gradient 4 frames OutQuad

            const float frame = 0.032f;
            currentSequence.Kill(true);

            panelMask.enabled = true;
            radialCenterImage.gameObject.SetActive(false);
            radialCoverRing.SetActive(false);
            radialCoverBackground.SetActive(false);

            panelGroup.alpha = 0;
            panelGroupRect.eulerAngles = new(0, 0, 0);
            panelGroupRect.anchoredPosition = new(-250, 0);
            navigatorRect.anchoredPosition = new(1250, -400);

            spinnyThingRect.localScale = Vector3.one * 2;
            glassImage.rectTransform.localScale = Vector3.zero;
            glassImage.DOFade(0, 0);
            gradientRect.anchoredPosition = new(0, 400);

            currentSequence = DOTween.Sequence();
            currentSequence.Join(panelGroup.DOFade(1, frame * 3).SetEase(Ease.Linear));
            currentSequence.Join(panelGroupRect.DOAnchorPosX(0, frame * 6).SetEase(Ease.OutQuad));
            currentSequence.Join(navigatorRect.DOAnchorPosX(270, frame * 6).SetEase(Ease.OutQuad));

            currentSequence.Insert(frame * 2, spinnyThingRect.DOScale(1, frame * 4).SetEase(Ease.OutQuad));
            currentSequence.Insert(frame * 2, glassImage.rectTransform.DOScale(1, frame * 4).SetEase(Ease.OutQuad));
            currentSequence.Insert(frame * 2, glassImage.DOFade(1, frame * 4).SetEase(Ease.OutQuad));
            currentSequence.Insert(frame * 2, gradientRect.DOAnchorPosY(652.5f, frame * 4));
        }

        public void Anim_HidePanelsLinearFull()
        {
            // 1 frame = 32ms
            // Move panels 4 frames InQuad
            // Scale SpinnyThing 4 frames InQuad
            // Scale Glass 4 frames InQuad
            // Fade Glass 4 frames InQuad
            // Move Gradient 4 frames InQuad

            // wait 2 frames
            // Fade panels out 2 frames Linear
            // Move Navigator 6 frames 
            // Fade Navigator 6 frames Linear

            const float frame = 0.032f;
            currentSequence.Kill(true);

            panelMask.enabled = true;
            radialCenterImage.gameObject.SetActive(false);
            radialCoverRing.SetActive(false);
            radialCoverBackground.SetActive(false);

            panelGroupRect.anchoredPosition = new(0, 0);
            panelGroupRect.eulerAngles = new(0, 0, 0);
            glassImage.rectTransform.localScale = Vector3.one;
            glassImage.DOFade(1, 0);
            gradientRect.anchoredPosition = new(0, 652.5f);

            panelGroup.alpha = 1;
            navigatorRect.anchoredPosition = new(270, -400);

            currentSequence = DOTween.Sequence();
            currentSequence.Join(panelGroupRect.DOAnchorPosX(-250, frame * 4).SetEase(Ease.InQuad));
            currentSequence.Join(spinnyThingRect.DOScale(2, frame * 4).SetEase(Ease.InQuad));
            currentSequence.Join(glassImage.rectTransform.DOScale(0.5f, frame * 4).SetEase(Ease.InQuad));
            currentSequence.Join(glassImage.DOFade(0, frame * 4).SetEase(Ease.InQuad));
            currentSequence.Join(gradientRect.DOAnchorPosY(400, frame * 4).SetEase(Ease.InQuad));

            currentSequence.Insert(frame * 2, panelGroup.DOFade(0, frame * 2).SetEase(Ease.Linear));
            currentSequence.Insert(frame * 2, navigatorRect.DOAnchorPosX(1250, frame * 6).SetEase(Ease.InQuad));
        }


        public void Anim_ShowPanelsRadial()
        {
            // Scale Glass 8 frames OutQuad
            // Fade Glass 8 frames OutQuad
            // Fade radial center 4 frames InQuad

            // wait 4 frames
            // Spin panels 6 frames OutQuad
            // Fade panels 6 frames OutQuad

            const float frame = 0.032f;
            currentSequence.Kill(true);

            panelMask.enabled = false;
            radialCenterImage.gameObject.SetActive(true);
            radialCoverRing.SetActive(true);
            radialCoverBackground.SetActive(true);

            panelGroupRect.anchoredPosition = new(0, 0);
            panelGroupRect.eulerAngles = new(0, 0, 270);
            panelGroup.alpha = 0;
            glassImage.rectTransform.localScale = Vector3.zero;
            glassImage.DOFade(0, 0);
            radialCenterImage.DOFade(0, 0);

            currentSequence = DOTween.Sequence();
            currentSequence.Join(glassImage.rectTransform.DOScale(1, frame * 8).SetEase(Ease.OutQuad));
            currentSequence.Join(glassImage.DOFade(1, frame * 8).SetEase(Ease.OutQuad));
            currentSequence.Join(radialCenterImage.DOFade(1, frame * 4).SetEase(Ease.InQuad));

            currentSequence.Insert(frame * 4, panelGroupRect.DORotate(new(0, 0, 0), frame * 6).SetEase(Ease.OutQuad));
            currentSequence.Insert(frame * 4, panelGroup.DOFade(1, frame * 6).SetEase(Ease.OutQuad));

        }

        public void Anim_HidePanelsRadial()
        {
            // Spin panels 3 frames Linear
            // Fade panels 3 frames OutQuad
            // Scale glass 6 frames Linear
            // Fade glass 6 frames Linear

            // wait 6 frames
            // Scale preview 4 frames InBounce

            const float frame = 0.032f;
            currentSequence.Kill(true);

            panelMask.enabled = false;
            radialCenterImage.gameObject.SetActive(true);
            radialCoverRing.SetActive(true);
            radialCoverBackground.SetActive(true);

            panelGroupRect.eulerAngles = new(0, 0, 0);
            panelGroup.alpha = 1;
            glassImage.rectTransform.localScale = Vector3.one;
            glassImage.DOFade(1, 0);

            currentSequence = DOTween.Sequence();
            currentSequence.Join(panelGroup.DOFade(0, frame * 3).SetEase(Ease.OutQuad));
            currentSequence.Join(glassImage.rectTransform.DOScale(0, frame * 6).SetEase(Ease.Linear));
            currentSequence.Join(glassImage.DOFade(0, frame * 6).SetEase(Ease.Linear));
        }


        public void SetPrimaryPanel(UIListItem item)
        {
            bool dynamic = item.ItemType is UIListItem.ItemTypes.SubMenu && item.SubtitleType is UIListItem.SubtitleTypes.Dynamic;

            primaryPanel.Title = item.Title;
            primaryPanel.Subtitle = dynamic ? GetSelectedString(item) : item.Subtitle;
            primaryPanel.SetRadialPanelColor(item);
        }

        public void GetPanels(UIScreen screen, int currentIndex = 0)
        {
            // TODO: Make GetPanels set panels active/inactive properly.

            currentIndex = Mathf.Clamp(currentIndex, 0, screen.ListItems.Count);

            int itemCount = screen.ListItems.Count;
            if (itemCount == 0) return;

            if (screen.ScreenType is UIScreen.UIScreenType.Radial) getRadial();
            else getLinear();

            void getLinear()
            {
                linearPanelGroup.SetActive(true);
                radialPanelGroup.SetActive(false);

                SetPrimaryPanel(screen.ListItems[currentIndex]);
                primaryPanel.SetType(screen.ScreenType);

                LinearCenterIndex = SaturnMath.Modulo(currentIndex, linearPanels.Count);

                for (int i = 0; i < linearPanels.Count; i++)
                {
                    var panel = linearPanels[i];

                    if (i >= screen.ListItems.Count)
                    {
                        panel.gameObject.SetActive(false);
                        continue;
                    }

                    int distance = i - currentIndex;
                    var item = screen.ListItems[i];
                    SetPanelLinear(screen, item, panel);

                    int index = Mathf.Clamp(LinearHalfCount - LinearCenterIndex + i, 0, 6);

                    Vector2 position = GetLinearPosition(index);
                    float scale = GetLinearScale(index);

                    panel.rect.anchoredPosition = position;
                    panel.rect.localScale = Vector3.one * scale;
                    panel.gameObject.SetActive(true);
            }
        }

            void getRadial()
            {
                linearPanelGroup.SetActive(false);
                radialPanelGroup.SetActive(true);

                SetPrimaryPanel(screen.ListItems[currentIndex]);
                primaryPanel.SetType(screen.ScreenType);

                for (int i = 0; i < radialPanels.Count; i++)
                {
                    var panel = radialPanels[i];

                    if (i >= screen.ListItems.Count)
                    {
                        panel.gameObject.SetActive(false);
                        continue;
                    }

                    int distance = i - currentIndex;
                    var item = screen.ListItems[i];

                    SetPanelRadial(item, panel);

                    int index = Mathf.Clamp(Mathf.Abs(distance + 5), 0, 17);
                    float angle = angles[index];
                    panel.rect.eulerAngles = new Vector3(0, 0, angle);
                    panel.gameObject.SetActive(true);
                }
            }
        }

        private string GetSelectedString(UIListItem item)
        {
            int settingsIndex = SettingsManager.Instance.PlayerSettings.GetParameter(item.SettingsBinding);

            if (settingsIndex == -1)
            {
                Debug.LogWarning($"Setting \"{item.SettingsBinding}\" was not found!");
                return "???";
            }

            if (item.NextScreen == null)
            {
                Debug.LogWarning($"NextScreen of [{item.Title}] has not been set!");
                return "???";
            }

            if (item.NextScreen.ListItems.Count == 0)
            {
                Debug.LogWarning($"NextScreen of [{item.Title}] has no List Items!");
                return "???";
            }

            var selectedItem = item.NextScreen.ListItems.FirstOrDefault(x => x.SettingsValue == settingsIndex);

            if (selectedItem == null)
            {
                Debug.LogWarning($"No item with matching index [{settingsIndex}] was found!");
                return "???";
            }

            return selectedItem.Title;
        }


        private Vector2 GetLinearPosition(int index)
        {
            float[] posX = { 120,   55,   20,   20,   20,   55,  120 };
            float[] posY = { 350,  250,  150,    0, -150, -250, -350 };
            return new(posX[index], posY[index]);
        }

        private float GetLinearScale(int index)
        {
            float[] scales = {0.85f, 0.85f, 1, 1, 1, 0.85f, 0.85f };
            return scales[index];
        }

        private void SetPanelLinear(UIScreen screen, UIListItem item, OptionPanelLinear panel)
        {
            bool dynamic = item.ItemType is UIListItem.ItemTypes.SubMenu && item.SubtitleType is UIListItem.SubtitleTypes.Dynamic;

            panel.Title = item.Title;
            panel.Subtitle = dynamic ? GetSelectedString(item) : item.Subtitle;
            panel.SetType(screen.ScreenType);
        }

        private void SetPanelRadial(UIListItem item, OptionPanelRadial panel)
        {
            panel.Title = item.Title;
            panel.SetRadialPanelColor(item);
        }
    }
}
