using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MainGameView : MonoBehaviour {

    public Text WhosTurnTitleText;
    public Text WhosTurnText;
    public Text ScoreTitleText;
    public Text ScoreText;
    public Text ScoreSignText;

    public Text BestScoreText;
    public Text SubMultiButtomText;

    public Text MissQuesTitleText;
    public Text MissQuesText;

    public GameObject[] PlayerGameObjectForPauseMenu;
    public Text[] PlayersNamesForPauseMenu;
    public Text[] PlayersScoresForPauseMenu;

    public Image playerCurrentTurnBackground;

    //buttons texts
    public Text ButtonBackToGameText;
    public Text ButtonExitGameText;
    public Text ButtonRestartGameText;

    public Text HowToplayButtonText;
    public Text SettingsButtonText;
    public Text LanguageButtonText;

    //Settings texts for language
    public Text SettingsMainTitleText;
    public Text SettingsTitleText;
    public Text SettingsSubTitleText;
    public Text SettingsCardOnFieldText;
    public Text SettingsAmountOfPlayersText;
    public Text SettingsYourNamesText;

    public Text TitleSoloGameText;
    public Text TitleMultiGameText;
    public Text LanguageTitleText;

    public Text HowToPlayTitleText;
    public GameObject[] HowToPlayLanguageBunddle; //0- english, 1-hebrew
    public GameObject[] GameTitleLanguageBunddle; //0- english, 1-hebrew


    public SpriteRenderer[] CARDS_SPRITES; //CARDS_SPRITES[0] is back of a card, all the rest are cards  // Size: 23
    public GameObject[] GameCards; //The x cards on field, for sprite changing
    public Text[] _gameCardsTitleText; //The x cards on field, for sprite changing

    public GameObject _cardToShowOnMatch;

    [HideInInspector]
    public bool isMenuWindowActive; //Is menuWindow active
    [HideInInspector]
    public bool isSettingsWindowActive; //Is settingsWindow active
    [HideInInspector]
    public bool isPauseWindowActive; //Is settingsWindow active
    [HideInInspector]
    public bool isMissQuesWindowActive; //Is settingsWindow active
    [HideInInspector]
    public bool isHowToPlayWindowActive; //Is settingsWindow active
    [HideInInspector]
    public bool isLanguageWindowActive; //Is settingsWindow active

    public GameObject _menuWindow;
    public GameObject _settingsWindow;
    public GameObject _pauseWindow;
    public GameObject _missQuesWindow;
    public GameObject _howToPlayWindow;
    public GameObject _languageWindow;

    public ParticleSystem _particleSystemLoop;
    public ParticleSystem _particleSystemFlower;

    public GameObject ParticleSystemWIN;

    public SpriteRenderer MissQuesBackground;

    public GameObject BackToGameButton;
    public GameObject GetScoreButton;
    public GameObject RejectScoreButton;

    public GameObject PauseButton;
    public Text PauseTitleText; //Also Winner's name or tie


    public Action EPlayTurnPressed;
    public Action ERestartPressed;

    //private void Update() {
    //    if (Input.GetKeyDown(KeyCode.Space)) {
    //        PlayTurnButton();
    //    }

    //    if (Input.GetKeyDown(KeyCode.Escape)) {
    //        RestartGameButton();
    //    }
    //}

    //public void PlayTurnButton() {
    //    EPlayTurnPressed();
    //}

    public void RestartGameButton() {
        ERestartPressed();
    }

    public void UpdateView(GameData gameData) {
        UpdateCounters(gameData);
        UpdatePlayground(gameData);
        ChangeGameCardsSize(gameData);
    }

    //Changes game cards sizes according to number of cards on field
    private void ChangeGameCardsSize(GameData gameData) {
        if (gameData._gameDeck.Length == 16) {
            for (int i = 0; i < 16; i++) {
                GameCards[i].transform.localPosition = gameData.locations16[i];
                GameCards[i].transform.localScale = new Vector3(0.29f, 0.29f, 0.29f);
            }
        }
        else if (gameData._gameDeck.Length == 20) {
            for (int i = 0; i < 20; i++) {
                GameCards[i].transform.localPosition = gameData.locations20[i];
                GameCards[i].transform.localScale = new Vector3(0.27f, 0.27f, 0.27f);
            }

        }
        else if (gameData._gameDeck.Length == 24) {
            for (int i = 0; i < 24; i++) {
                GameCards[i].transform.localPosition = gameData.locations24[i];
                GameCards[i].transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
        }
    }


    //Sets all cards on filed face down and make sure their rotation is (0,0,0)
    public void FlipAllCards(GameData gameData) {
        for (int i = 0; i < gameData._gameDeck.Length; i++) {
            GameCards[i].GetComponent<SpriteRenderer>().sprite = CARDS_SPRITES[0].sprite;
            GameCards[i].transform.GetChild(0).gameObject.SetActive(false);

            GameCards[i].transform.rotation = Quaternion.identity;

        }
    }

    private void UpdatePlayground(GameData gameData) {
        for (int i = GameCards.Length - 1; i >= 0; i--) {
            if (gameData._gameDeck.Length <= i) {
                GameCards[i].SetActive(false);
            }
            else {
                GameCards[i].SetActive(true);
                _gameCardsTitleText[i].text = gameData._gameDeck[i].GetCardTitleText();
            }
        }
    }




    public void UpdateMissQuesManu(GameData gameData) {

        if (gameData._chosenCard.GetCardType() == Card.CardType.Mission) {//mission
            MissQuesText.fontSize = 61;
            switch (gameData.Language) {
                case 0:
                    MissQuesTitleText.text = "Mission";
                    break;
                case 1:
                    MissQuesTitleText.text = "המישמ";
                    break;
            }

            BackToGameButton.SetActive(false);
            GetScoreButton.SetActive(true);
            RejectScoreButton.SetActive(true);
            MissQuesBackground.color = new Color(1f, 1f, 1f, 0.9882353f);
            MissQuesText.text = gameData._chosenCard.GetCardMissionText();
        }

        else {//Question
            MissQuesText.fontSize = 80;
            if (gameData._chosenCard.GetValue() == 3 || gameData._chosenCard.GetValue() == 5 ||
                gameData._chosenCard.GetValue() == 9 || gameData._chosenCard.GetValue() == 15 ||
                gameData._chosenCard.GetValue() == 17 || gameData._chosenCard.GetValue() == 19) {
                switch (gameData.Language) {
                    case 0:
                        MissQuesTitleText.text = "Tip";
                        break;
                    case 1:
                        MissQuesTitleText.text = "פיט";
                        break;
                }

                MissQuesBackground.color = new Color(1f, 0.6173745f, 0f, 0.9882353f);
            }

            else if (gameData._chosenCard.GetValue() == 1 || gameData._chosenCard.GetValue() == 11 ||
                gameData._chosenCard.GetValue() == 18 || gameData._chosenCard.GetValue() == 20) {
               
                switch (gameData.Language) {
                    case 0:
                        MissQuesTitleText.text = "Bonus";
                        break;
                    case 1:
                        MissQuesTitleText.text = "סונוב";
                        break;
                }

                MissQuesBackground.color = new Color(0.572549f, 0.6871964f, 1f, 0.9882353f);
            }

            else {
                switch (gameData.Language) {
                    case 0:
                        MissQuesTitleText.text = "Question";
                        break;
                    case 1:
                        MissQuesTitleText.text = "הלאש";
                        break;
                }

                MissQuesBackground.color = new Color(0.820439f, 0.6650944f, 1f, 0.9882353f);
            }

            BackToGameButton.SetActive(true);
            GetScoreButton.SetActive(false);
            RejectScoreButton.SetActive(false);
           
            MissQuesText.text = gameData._chosenCard.GetCardQuestionText();
        }


  
    }

    public void UpdatPauseMenu(GameData gameData) {
        if (gameData.isGameRun) {
            switch (gameData.Language) {
                case 0:
                    PauseTitleText.text = "Score Table:";
                    break;
                case 1:
                    PauseTitleText.text = ":דוקינ תלבט";
                    break;
            }
        }
        else {
            if (gameData.SetWonPlayerName()) { // its a tie
                switch (gameData.Language) {
                    case 0:
                        PauseTitleText.text = "It's a Tie!";
                        break;
                    case 1:
                        PauseTitleText.text = "וקיתב רמגנ";
                        break;
                }
               
            }
            else {
                switch (gameData.Language) {
                    case 0:
                        PauseTitleText.text = gameData.WinnerPlayer.GetName() + " Won!";
                        break;
                    case 1:
                        PauseTitleText.text = "!" + gameData.WinnerPlayer.GetName() + " לש ןוחצינה";
                        break;
                }
            }
        }

        switch (gameData.Language) {
            case 0:// english
                for (int i = 0; i < 5; i++) {
                    if (i < gameData.PlayersList.Count) {
                        PlayerGameObjectForPauseMenu[i].SetActive(true);
    
                        if (gameData.PlayersList[i].GetName() == "") {
                            PlayersNamesForPauseMenu[i].text = "Player " + (i + 1);
                        }
                        else {
                            PlayersNamesForPauseMenu[i].text = gameData.PlayersList[i].GetName();
                        }
                        PlayersScoresForPauseMenu[i].text = gameData.PlayersList[i].GetScore().ToString();
                    }

                    else {
                        PlayerGameObjectForPauseMenu[i].SetActive(false);
                    }

                }
                break;
            case 1://hebrew
                for (int i = 0; i < 5; i++) {
                    if (i < gameData.PlayersList.Count) {
                        PlayerGameObjectForPauseMenu[i].SetActive(true);

                        if (gameData.PlayersList[i].GetName() == "") {
                            PlayersScoresForPauseMenu[i].text = (i + 1) + " ןקחש";
                        }
                        else {
                            PlayersScoresForPauseMenu[i].text = gameData.PlayersList[i].GetName();
                        }
                        PlayersNamesForPauseMenu[i].text = gameData.PlayersList[i].GetScore().ToString();
                        
                    }

                    else {

                        PlayerGameObjectForPauseMenu[i].SetActive(false);
                    }

                }
                break;
        }

      
    }
    public void PlayWINParticles() {
        ParticleSystemWIN.SetActive(true);
        ParticleSystemWIN.GetComponent<ParticleSystem>().Play();
    }

    public void StopWINParticles() {
        if (ParticleSystemWIN.GetComponent<ParticleSystem>().isPlaying) ParticleSystemWIN.GetComponent<ParticleSystem>().Stop();
        ParticleSystemWIN.SetActive(false);
    }

    public void PlayMatchParticles() {
        _particleSystemFlower.Play();
        _particleSystemLoop.Play();
    }

    public void PlayGotScorAnim(bool isItPlusSign) {
        if (isItPlusSign) {//Points added
            ScoreSignText.color = new Color(0.3053578f, 0.8867924f, 0.3267312f);
            ScoreSignText.text = "+";
        }
        else {//Points reduced
            ScoreSignText.color = new Color(0.8962264f, 0.2585075f, 0.2071467f);
            ScoreSignText.text = "-";
        }
        ScoreText.GetComponent<Animator>().Play("ScoreAnim");
        ScoreSignText.GetComponent<Animator>().Play("ScoreAnim");
    }



    /// <summary>
    /// Showing wanted window
    /// </summary>
    /// <param name="gameObject"> gameobject of window to show </param>
    public void ShowWindow(GameObject gameObject) {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// Hiding wanted window
    /// </summary>
    /// <param name="gameObject"> gameobject of window to hide </param>
    public void HideWindow(GameObject gameObject) {
        gameObject.SetActive(false);
    }
    //Toggle settings window visabilty
    public void ToggleSettingsWindow() {
        HideWindow(_howToPlayWindow);
        isHowToPlayWindowActive = false;
        if (isSettingsWindowActive) {
            HideWindow(_settingsWindow);
            isSettingsWindowActive = false;
        }
        else {
            ShowWindow(_settingsWindow);
            isSettingsWindowActive = true;
        }
    }
    //Toggle how to play window visabilty
    public void ToggleHowtoPlayWindow() {
        HideWindow(_settingsWindow);
        isSettingsWindowActive = false;
        if (isHowToPlayWindowActive) {
            HideWindow(_howToPlayWindow);
            isHowToPlayWindowActive = false;
        }
        else {
            ShowWindow(_howToPlayWindow);
            isHowToPlayWindowActive = true;
        }
    }
    //Toggle language window visabilty
    public void ToggleLanguageWindow() {
        if (isLanguageWindowActive) {
            HideWindow(_languageWindow);
            isLanguageWindowActive = false;
        }
        else {
            ShowWindow(_languageWindow);
            isLanguageWindowActive = true;
        }
    }


    //Toggle missQues window visabilty
    public void ToggleMissQuesWindow() {
        if (isMissQuesWindowActive) {
            _missQuesWindow.GetComponent<Animator>().Play("BringUp");
             isMissQuesWindowActive = false;
        }
        else {
            ShowWindow(_missQuesWindow);
            _missQuesWindow.GetComponent<Animator>().Play("BringDown");
            isMissQuesWindowActive = true;
        } 
    }

    public void MoveCardToFront(GameData gameData) {
        _cardToShowOnMatch.GetComponent<SpriteRenderer>().sprite = CARDS_SPRITES[gameData._chosenCard.GetValue() + 1].sprite;
        _cardToShowOnMatch.GetComponentInChildren<Text>().text = gameData.CardsTitleTexts[gameData._chosenCard.GetValue() % gameData.CardsTitleTexts.Length];
        _cardToShowOnMatch.transform.position = GameCards[gameData._lastPositionInDeck].transform.position;
        gameData.isBringCardToFrontCoroutineRun = true;
        StartCoroutine(MoveCardOverSeconds(gameData, _cardToShowOnMatch, GameCards[gameData._lastPositionInDeck].transform.position,
            new Vector3(-0f, -1.4f, 0f), GameCards[gameData._lastPositionInDeck].transform.localScale, new Vector3(0.9f, 0.9f, 0.9f), 0.5f, true));

    }

    public void MoveCardToBack(GameData gameData) {
        gameData.isBringCardToFrontCoroutineRun = true;
        StartCoroutine(MoveCardOverSeconds(gameData, _cardToShowOnMatch, new Vector3(-0f, -1.4f, 0f), GameCards[gameData._lastPositionInDeck].transform.position,
            _cardToShowOnMatch.transform.localScale, GameCards[gameData._lastPositionInDeck].transform.localScale, 0.5f, false));
    }

    public void HideCardAndRestorSize() {
        _cardToShowOnMatch.SetActive(false);
    }

    private IEnumerator MoveCardOverSeconds(GameData gameData, GameObject objectToMove, Vector3 startPosition, Vector3 endPosition, Vector3 originalScale, Vector3 wantedScale, float time, bool enlragingSprite) {
        float elapsedTime = 0;
        Vector3 startingPos = startPosition;
        if (enlragingSprite) {
            _cardToShowOnMatch.SetActive(true);
        }

        while (elapsedTime < time) {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endPosition, (elapsedTime / time));
            objectToMove.transform.localScale = Vector3.Lerp(originalScale, wantedScale, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = endPosition;
        if (!enlragingSprite) {
            HideCardAndRestorSize();
        }
        gameData.isBringCardToFrontCoroutineRun = false;
    }

    public void UpdateCountersTitles(GameData gameData, bool isSoloGame) {
        if (gameData.Language == 0) {
            if (isSoloGame) {
                WhosTurnTitleText.text = "Solo Player:";
                ScoreTitleText.text = "Steps:";
            }
            else {
                WhosTurnTitleText.text = "Your Turn:";
                ScoreTitleText.text = "Score:";
            }
        }

        else if (gameData.Language == 1) { //Hebrew
            if (isSoloGame) {
                WhosTurnTitleText.text = "דיחי ןקחש";
                ScoreTitleText.text = "םיכלהמ";
            }
            else {
                WhosTurnTitleText.text = "קחשל ךרות";
                ScoreTitleText.text = "תודוקנ";
            }
        }

    }

    public void UpdateCounters(GameData gameData) {
        if (gameData.PlayersList[gameData.GetWhosTurnIsIt()].GetName() == "") {
            switch (gameData.Language) {
                case 0:
                    WhosTurnText.text = "Player " + (gameData.GetWhosTurnIsIt() + 1);
                    break;
                case 1:
                    WhosTurnText.text = (gameData.GetWhosTurnIsIt() + 1) + " ןקחש";
                    break;    
            }
           
        }
        else {
            WhosTurnText.text = gameData.PlayersList[gameData.GetWhosTurnIsIt()].GetName();
        }



        playerCurrentTurnBackground.color = PlayerGameObjectForPauseMenu[gameData.GetWhosTurnIsIt()].GetComponent<Image>().color;

        ScoreText.text = gameData.PlayersList[gameData.GetWhosTurnIsIt()].GetScore().ToString();
    }

    public void UpdateAllTextsLanguage(int language, int bestScore) {
        //Updating buttons texts (back to game, exit game, restart game)
        if (language == 0) {
            ButtonBackToGameText.text = "Back to Game";
            ButtonExitGameText.text = "Exit to Menu";
            ButtonRestartGameText.text = "Restart Game";

            SettingsMainTitleText.text = "Settings";
            SettingsTitleText.text = "Interactive Game?";
            SettingsSubTitleText.text = "(only for Multiplayer)";
            SettingsCardOnFieldText.text = "Cards On field:";
            SettingsAmountOfPlayersText.text = "Amount of Players:";
            SettingsYourNamesText.text = "Your Names:";

            TitleSoloGameText.text = "Solo Player";
            TitleMultiGameText.text = "2-5 Players";
            SubMultiButtomText.text = "* Can be changed\nin settings";

            LanguageTitleText.text = "Language";


            HowToplayButtonText.text = "How to Play";
            SettingsButtonText.text = "Settings";
            LanguageButtonText.text = "Language";

            //How To Play
            HowToPlayTitleText.text = "Arthur: Social Memory Game";
            HowToPlayLanguageBunddle[0].SetActive(true);
            HowToPlayLanguageBunddle[1].SetActive(false);

            GameTitleLanguageBunddle[0].SetActive(true);
            GameTitleLanguageBunddle[1].SetActive(false);
            

            if (bestScore == int.MaxValue) {
                BestScoreText.text = "Best Score: -";
            }
            else {
                BestScoreText.text = "Best Score: " + bestScore.ToString();
            }
        }

        else if (language == 1) {
            ButtonBackToGameText.text = "קחשמל הרזח";
            ButtonExitGameText.text = "טירפתל האיצי";
            ButtonRestartGameText.text = "שדח קחשמ";

            SettingsMainTitleText.text = "תורדגה";
            SettingsTitleText.text = "?הלעפה קחשמ";
            SettingsSubTitleText.text = "(םיפתתשמ הבורמל קר)";
            SettingsCardOnFieldText.text = ":קחשמב םיפלק רפסמ";
            SettingsAmountOfPlayersText.text = ":םינקחש רפסמ";
            SettingsYourNamesText.text = ":םינקחשה תומש";

            TitleSoloGameText.text = "דיחי ןקחש";
            TitleMultiGameText.text = "םינקחש 2-5";
            SubMultiButtomText.text = "גוס תונשל ןתינ *" + "\n"+ "תורדגהב קחשמ";

            LanguageTitleText.text = "הפש רחב";

            HowToplayButtonText.text = "קחשמה יקוח";
            SettingsButtonText.text = "תורדגה";
            LanguageButtonText.text = "הפש הנש";

            //How To Play
            HowToPlayTitleText.text = "יתרבח ןורכיז קחשמ :רותרא";
            HowToPlayLanguageBunddle[0].SetActive(false);
            HowToPlayLanguageBunddle[1].SetActive(true);

            GameTitleLanguageBunddle[0].SetActive(false);
            GameTitleLanguageBunddle[1].SetActive(true);

            if (bestScore == int.MaxValue) {
                BestScoreText.text = "- :אישה";
            }
            else {
                BestScoreText.text = bestScore.ToString() + " :ישיא איש";
            }
        }
    }

    public void UpdateMenuView(GameData gameData, int bestScore) {
        switch (gameData.Language) {
            case 0:
                if (bestScore == int.MaxValue) {
                    BestScoreText.text = "Best Score: -";
                }
                else {
                    BestScoreText.text = "Best Score: " + bestScore.ToString();
                }
                break;
            case 1:
                if (bestScore == int.MaxValue) {
                    BestScoreText.text = "- :אישה";
                }
                else {
                    BestScoreText.text = bestScore.ToString() + " :אישה";
                }
                break;
        }
      
      
    }

}
