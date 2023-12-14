using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SaturnGame.UI
{
    public class MaintenanceMenuManager : MonoBehaviour
    {
        [Header("BUTTONS")]
        [SerializeField] private List<Button> mainMenuButtons;
        [SerializeField] private List<Button> creditLogButtons;
        [SerializeField] private List<Button> hardwareTestButtons;
        [SerializeField] private List<Button> hardwareInfoButtons;
        [SerializeField] private List<Button> systemSettingsButtons;
        [Space(10)]
        [SerializeField] private List<Button> audioTestButtons;
        [SerializeField] private List<Button> inputTestButtons;
        [SerializeField] private List<Button> lightTestButtons;
        [SerializeField] private List<Button> displayTestButtons;
        [SerializeField] private List<Button> networkTestButtons;
        [SerializeField] private List<Button> cardReaderTestButtons;
        [Space(10)]
        [SerializeField] private List<Button> gameSettingsButtons;
        [SerializeField] private List<Button> audioSettingsButtons;
        [SerializeField] private List<Button> inputSettingsButtons;
        [SerializeField] private List<Button> lightSettingsButtons;
        [SerializeField] private List<Button> displaySettingsButtons;
        [SerializeField] private List<Button> networkSettingsButtons;
        [SerializeField] private List<Button> graphicsSettingsButtons;

        [Header("MENUS")]
        [SerializeField] private List<GameObject> mainMenus;
        [SerializeField] private List<GameObject> settingsMenus;
        [SerializeField] private List<GameObject> hardwareTestMenus;
        [Header("OTHER")]
        [SerializeField] private AudioTester audioTester;

        private int mainMenuIndex = 0;
        private int settingsMenuIndex = 0;
        private int hardwareTestIndex = 0;

        void Awake()
        {
            // Main Menu Buttons
            mainMenuButtons[0].onClick.AddListener(OpenCreditLog);
            mainMenuButtons[1].onClick.AddListener(OpenHardwareTest);
            mainMenuButtons[2].onClick.AddListener(OpenHardwareInfo);
            mainMenuButtons[3].onClick.AddListener(OpenSystemSettings);
            mainMenuButtons[4].onClick.AddListener(ReturnToGame);

            // Credit Log Buttons
            //creditLogButtons[0].onClick.AddListener();
            creditLogButtons[1].onClick.AddListener(OpenMain);

            // Hardware Test Buttons
            hardwareTestButtons[0].onClick.AddListener(delegate{OpenHardwareTestSubMenu(0);});
            hardwareTestButtons[1].onClick.AddListener(delegate{OpenHardwareTestSubMenu(1);});
            hardwareTestButtons[2].onClick.AddListener(delegate{OpenHardwareTestSubMenu(2);});
            hardwareTestButtons[3].onClick.AddListener(delegate{OpenHardwareTestSubMenu(3);});
            hardwareTestButtons[4].onClick.AddListener(delegate{OpenHardwareTestSubMenu(4);});
            hardwareTestButtons[5].onClick.AddListener(delegate{OpenHardwareTestSubMenu(5);});
            hardwareTestButtons[6].onClick.AddListener(OpenMain);

            audioTestButtons[0].onClick.AddListener(delegate{audioTester.PlayLeft();});
            audioTestButtons[1].onClick.AddListener(delegate{audioTester.PlayRight();});
            audioTestButtons[2].onClick.AddListener(delegate{audioTester.PlayStereo();});
            audioTestButtons[3].onClick.AddListener(OpenHardwareTest);

            inputTestButtons[0].onClick.AddListener(OpenHardwareTest);

            lightTestButtons[0].onClick.AddListener(OpenHardwareTest);

            displayTestButtons[0].onClick.AddListener(OpenHardwareTest);

            networkTestButtons[0].onClick.AddListener(OpenHardwareTest);

            cardReaderTestButtons[0].onClick.AddListener(OpenHardwareTest);

            // Hardware Info Buttons
            hardwareInfoButtons[0].onClick.AddListener(OpenMain);

            // Settings Menu Buttons
            systemSettingsButtons[0].onClick.AddListener(delegate{OpenSettingsSubMenu(0);});
            systemSettingsButtons[1].onClick.AddListener(delegate{OpenSettingsSubMenu(1);});
            systemSettingsButtons[2].onClick.AddListener(delegate{OpenSettingsSubMenu(2);});
            systemSettingsButtons[3].onClick.AddListener(delegate{OpenSettingsSubMenu(3);});
            systemSettingsButtons[4].onClick.AddListener(delegate{OpenSettingsSubMenu(4);});
            systemSettingsButtons[5].onClick.AddListener(delegate{OpenSettingsSubMenu(5);});
            systemSettingsButtons[6].onClick.AddListener(delegate{OpenSettingsSubMenu(6);});
            systemSettingsButtons[7].onClick.AddListener(OpenMain);

            gameSettingsButtons[0].onClick.AddListener(OpenSystemSettings);

            audioSettingsButtons[0].onClick.AddListener(OpenSystemSettings);

            inputSettingsButtons[0].onClick.AddListener(OpenSystemSettings);

            lightSettingsButtons[0].onClick.AddListener(OpenSystemSettings);

            //displaySettingsButtons[0].onClick.AddListener();
            //displaySettingsButtons[1].onClick.AddListener();
            //displaySettingsButtons[2].onClick.AddListener();
            //displaySettingsButtons[3].onClick.AddListener();
            displaySettingsButtons[4].onClick.AddListener(OpenSystemSettings);

            networkSettingsButtons[0].onClick.AddListener(OpenSystemSettings);

            graphicsSettingsButtons[0].onClick.AddListener(OpenSystemSettings);
        }

        void CloseAll()
        {
            foreach (GameObject menu in mainMenus)
                menu.SetActive(false);
            
            foreach (GameObject menu in settingsMenus)
                menu.SetActive(false);

            foreach (GameObject menu in hardwareTestMenus)
                menu.SetActive(false);
        }

        void ReturnToGame()
        {

        }

        // ==== MAIN MENUS
        void OpenMain()
        {
            CloseAll();
            mainMenus[0].SetActive(true);
            settingsMenuIndex = 0;
            EventSystem.current.SetSelectedGameObject(mainMenuButtons[mainMenuIndex].gameObject);
        }

        void OpenCreditLog()
        {
            CloseAll();
            mainMenus[1].SetActive(true);
            mainMenuIndex = 0;
        }

        void OpenHardwareTest()
        {
            CloseAll();
            mainMenus[2].SetActive(true);
            mainMenuIndex = 1;
            EventSystem.current.SetSelectedGameObject(hardwareTestButtons[hardwareTestIndex].gameObject);
        }

        void OpenHardwareInfo()
        {
            CloseAll();
            mainMenus[3].SetActive(true);
            mainMenuIndex = 2;
        }

        void OpenSystemSettings()
        {
            CloseAll();
            mainMenus[4].SetActive(true);
            mainMenuIndex = 3;
            EventSystem.current.SetSelectedGameObject(systemSettingsButtons[settingsMenuIndex].gameObject);
        }

        // ==== SETTINGS SUBMENUS
        void OpenSettingsSubMenu(int id)
        {
            CloseAll();
            settingsMenus[id].SetActive(true);
            settingsMenuIndex = id;
        }

        void OpenHardwareTestSubMenu(int id)
        {
            CloseAll();
            hardwareTestMenus[id].SetActive(true);
            hardwareTestIndex = id;
        }
    }
}
