using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaturnGame.UI
{
    [CreateAssetMenu(fileName = "UIScreen", menuName = "UI Logic/UI Screen")]
    public class UIScreen : ScriptableObject
    {
        public enum UIScreenType
        {
            LinearSimple,
            LinearDetail,
            Radial
        }

        public string Name;
        public UIScreenType ScreenType;
        public List<UIListItem> ListItems;
    }

    [Serializable]
    public class UIListItem
    {
        public enum ItemType
        {
            ValueSetter,
            SubMenu
        }

        public string Title;
        public string Subtitle;
        public ItemType Type;

        // These should only be visible when Type is set to SubMenu.
        public UIScreen NextScreen;

        // These should only be visible when Type is set to ValueSetter.
        public string Paramter;
        public int Value;
        public Sprite OptionPreviewSprite;
    }
}
