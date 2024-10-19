using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class TitleScreenLogic : MonoBehaviour
{
    [SerializeField] private TitleScreenBackgroundColorRandomizer colorRandomizer;

	private Locale englishLocale;
	private Locale chineseLocale;

	private void Awake()
    {
        colorRandomizer.RandomizeColor();

		// 获取所有可用的Locale
		var availableLocales = LocalizationSettings.AvailableLocales.Locales;
		// 查找英文和中文的Locale
		englishLocale = availableLocales.Find(locale => locale.Identifier.Code == "en");
		chineseLocale = availableLocales.Find(locale => locale.Identifier.Code == "zh");
        if (englishLocale == null || chineseLocale == null)
        {
            Debug.LogError("Local is lacking.");
        }
	}

    public static void OnConfirm()
    {
        Debug.Log("ConfirmClick!");
        SceneSwitcher.Instance.LoadScene("_SongSelect");
    }

    public void ChangeLocale()
    {
        if (LocalizationSettings.SelectedLocale == englishLocale)
        {
            LocalizationSettings.SelectedLocale = chineseLocale;
        } else
        {
            LocalizationSettings.SelectedLocale = englishLocale;
        }
    }

    private void OnRandomizeColor()
    {
        colorRandomizer.RandomizeColor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) OnConfirm();
        if (Input.GetKeyDown(KeyCode.R)) OnRandomizeColor();
    }
}

