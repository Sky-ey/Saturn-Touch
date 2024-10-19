using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Localization.Settings;

namespace SaturnGame.UI
{
public class MenuWipeAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI splashText;
    [SerializeField] private RectTransform viewMask;
    [SerializeField] private RectTransform textMask;
    [SerializeField] private RectTransform ring1;
    [SerializeField] private RectTransform ring2;
    [SerializeField] private RectTransform ring3;
    private const float MaskDuration = 0.65f;
    private const float Ring1Duration = 0.12f;
    private const float Ring2Duration = 0.12f;
    private const float Ring3Duration = 0.12f;
    private readonly Vector2 viewMaskMin = new(0, 0);
    private readonly Vector2 viewMaskMed = new(350, 350);
    private readonly Vector2 viewMaskMax = new(1080, 1080);
    private readonly Vector2 textMaskMin = new(0, 120);
    private readonly Vector2 textMaskMax = new(500, 120);
    private readonly Vector2 ring1Max = new(500, 500);
    private readonly Vector2 ring1Min = new(0, 0);
    private readonly Vector2 ring2Max = new(450, 450);
    private readonly Vector2 ring2Min = new(0, 0);
    private readonly Vector2 ring3Max = new(400, 400);
    private readonly Vector2 ring3Min = new(0, 0);
    private readonly Ease viewEase = Ease.InOutBack;
    private readonly Ease ringEase = Ease.InOutQuad;

    private Sequence currentSequence;

    public void Anim_StartTransition()
    {
        currentSequence.Kill(true);
        currentSequence = DOTween.Sequence();

        RandomizeSplashText();
        viewMask.gameObject.SetActive(true);
        textMask.gameObject.SetActive(true);
        viewMask.sizeDelta = viewMaskMax;
        textMask.sizeDelta = textMaskMin;
        ring1.sizeDelta = ring1Min;
        ring2.sizeDelta = ring2Min;
        ring3.sizeDelta = ring3Min;

        currentSequence.Append(viewMask.DOSizeDelta(viewMaskMed, MaskDuration).SetEase(viewEase));
        currentSequence.Insert(0.3f, ring1.DOSizeDelta(ring1Max, Ring1Duration).SetEase(ringEase));
        currentSequence.Insert(0.4f, ring2.DOSizeDelta(ring2Max, Ring2Duration).SetEase(ringEase));
        currentSequence.Insert(0.45f, ring3.DOSizeDelta(ring3Max, Ring3Duration).SetEase(ringEase));

        currentSequence.Insert(0.9f, viewMask.DOSizeDelta(viewMaskMin, 0.12f));
        currentSequence.Insert(0.95f, ring3.DOSizeDelta(ring3Min, 0.12f));
        currentSequence.Insert(1.0f, ring2.DOSizeDelta(ring2Min, 0.12f));
        currentSequence.Insert(1.0f, ring1.DOSizeDelta(ring1Min, 0.12f));
        currentSequence.Insert(1.0f, textMask.DOSizeDelta(textMaskMax, 0.12f).SetEase(Ease.OutQuad));
    }

    public void Anim_EndTransition()
    {
        currentSequence.Kill(true);
        currentSequence = DOTween.Sequence();

        viewMask.sizeDelta = viewMaskMin;
        textMask.sizeDelta = textMaskMax;
        ring1.sizeDelta = ring1Min;
        ring2.sizeDelta = ring2Min;
        ring3.sizeDelta = ring3Min;

        currentSequence.Insert(0.0f, ring1.DOSizeDelta(viewMaskMax, 0.55f).SetEase(ringEase));
        currentSequence.Insert(0.0f, textMask.DOSizeDelta(textMaskMin, 0.12f).SetEase(Ease.OutQuad));
        currentSequence.Insert(0.025f, ring2.DOSizeDelta(viewMaskMax, 0.65f).SetEase(ringEase));
        currentSequence.Insert(0.05f, ring3.DOSizeDelta(viewMaskMax, 0.65f).SetEase(ringEase));
        currentSequence.Insert(0.075f, viewMask.DOSizeDelta(viewMaskMax, 0.65f).SetEase(viewEase)).OnComplete(() =>
            {
                viewMask.gameObject.SetActive(false);
                textMask.gameObject.SetActive(false);
            }
        );
    }

    public void Anim_ForceEnd()
    {
        currentSequence.Kill(true);

        viewMask.sizeDelta = viewMaskMax;
        textMask.sizeDelta = textMaskMin;
        ring1.sizeDelta = ring1Min;
        ring2.sizeDelta = ring2Min;
        ring3.sizeDelta = ring3Min;
        viewMask.gameObject.SetActive(false);
        textMask.gameObject.SetActive(false);
    }

    private void RandomizeSplashText()
    {
        string[] messagesEn =
        {
            "び",
            ":3",
            ":3c",
            ">:3",
            ">:3c",
            "100% Artificial!",
            "100% Organic!",
            "ALL Marvelous!",
            "Also try AstroDX!",
            "Also try Umiguri!",
            // ReSharper disable once StringLiteralTypo
            "Carg!",
            "Closing game... just kidding.",
            "Don't forget to hydrate!",
            "Drink some water!",
            "Dude that's fucking crazy yo",
            "Entering Dive Realm...",
            "Everybody fucking jump!",
            "Every Night, Every Day, Every Night, E",
            "Fixing bugs...",
            "Fixing memory leaks...",
            "Full Combo!",
            "Good Luck! :]",
            "Have Fun! :]",
            "HELP IM TRAPPED IN THE MACHINE LET ME OUT",
            "Here comes the bass",
            "You should totally play MNK Inferno",
            "It's behind you.",
            "It's not a touchscreen!",
            "KILL KILL KILL KILL KILL KILL",
            "MASTER",
            "Missless!",
            // ReSharper disable once StringLiteralTypo
            "nihao bro wo ai SaturnGame.exe",
            "Not a washing machine!",
            "Okay let's do this thing!",
            "Removing herobrine...",
            "R-Notes are worth double!",
            "SSS+",
            "Support your local arcades!",
            "Thank you for playing! :]",
            "Unexpected item in bagging area",
        };

        string[] messagesZh =
        {
			"ALL Marvelous!",
            "Huh?",
            "真的有人看这个吗？",
            "要玩的开心哦 ~",
            "打不过？开摆！",
            "记得喝口水哦！",
            "起来活动下吧！",
            "你又在打电动哦 ~",
			"OOkay let's do this thing!",
			"我去！划卡鹦鹉！",
            "哈↑",
            "某强烈脱水的滚筒洗衣机",
            "不要凉啊 ~ 我的划卡 ~",
            "Wacca Never End !",
            "听好了！嗯，听好了",
            "zijixie!",
			"哥们对着那台长得像喇叭的机子哭成泪人",
			"喂！那不是触摸屏啊！",
			"记得切hold",
            "ipa when ?",
            "The way to the right",
            "为什么Todo这么长啊啊啊啊",
            "Wacca，享年三岁",
			"滑键直接放hold里会蹭"
		};
        if (LocalizationSettings.SelectedLocale.Identifier.Code == "en")
        {
	        int randomID = Random.Range(0, messagesEn.Length - 1);
			splashText.text = messagesEn[randomID];
		} else
        {
		    int randomID = Random.Range(0, messagesZh.Length - 1);
			splashText.text = messagesZh[randomID];
		}

    }
}
}