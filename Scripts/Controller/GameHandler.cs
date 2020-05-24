using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour {

    //MANAGERS
    public DataManagement dataManagement;
    public SoundHandler soundHandler;

    public MainGameView MainGameViewRef;
 
    private GameData _gameData;

    public int PlayerAmount;

    public bool isSoloGame;

    private int _bestScore = int.MaxValue;

    //SETTING
    private int _amountOfCardsOnField;
    [HideInInspector]
    public string[] _playersNames = new string[5];
    private int _language = 0;

    //private int _startingTime;
    public readonly float HALT_TIME = 0.7f;

    //DropDown buttons on Settings
    public Dropdown LanguageropDown;
    public Dropdown AmountOfPlayersDropDown;
    public Dropdown AmountOfCardsOnFieldDropDown;
    public GameObject[] InputFieldsForPlayersNames;
    public Toggle WithMissionsToggle;
  

    //Coroutine
    public bool _waitingForHaltCoroutine;
    public bool[] _waitingForCardsFlipCoroutine;


    //Initializing for the first time.
    private void Awake() {
        
        if (GetIsFirstGameEver()) {
            
            SetFirstGameEver();
          
            SaveName("", 0);
            SaveName("", 1);
            SaveName("", 2);
            SaveName("", 3);
            SaveName("", 4);
           
        }

        if (PlayerPrefs.HasKey("Language")) {
            LoadLanguage();
        }
        if (PlayerPrefs.HasKey("BestScore")) {
            LoadBestScore();
        }
        LoadNames();

        isSoloGame = false;

        _gameData = new GameData(5, _playersNames, true, _language);

        
       // MainGameViewRef.ERestartPressed += RestartGame;
        //  MainGameViewRef.EPlayTurnPressed += OnMouseDown;
        DontDestroyOnLoad(gameObject);

        _amountOfCardsOnField = 8;
        MainGameViewRef.isSettingsWindowActive = false;
        MainGameViewRef.isHowToPlayWindowActive = false;
        MainGameViewRef.isLanguageWindowActive = false;
        MainGameViewRef.isPauseWindowActive = false;
        MainGameViewRef.isMissQuesWindowActive = false;
        MainGameViewRef.isMenuWindowActive = true;

   


        //Creates and shuffles game deck
        _gameData._gameDeck = new Card[_amountOfCardsOnField * 2];
        CreateDeck();
        _gameData._gameDeck = ShuffleDeck(_gameData._gameDeck);
        PlayerAmount = 2;
        _gameData._chosenCard = new Card(); //Setting empty card for chosenCard
        _gameData._lastPositionInDeck = -1;//Setting chosenCard location to -1 (no card)

        _waitingForHaltCoroutine = false;
        _waitingForCardsFlipCoroutine = new bool[2];

        _gameData._amountOfCardsFacedUp = 0;

        


        UpdateTextsLanguageView();
        //   _timeToEnd = _startingTime;
        MainGameViewRef.HideWindow(MainGameViewRef._pauseWindow);
        MainGameViewRef.HideWindow(MainGameViewRef._missQuesWindow);
        MainGameViewRef.HideWindow(MainGameViewRef._settingsWindow);
        MainGameViewRef.HideWindow(MainGameViewRef._howToPlayWindow);
        MainGameViewRef.HideWindow(MainGameViewRef._languageWindow);
        MainGameViewRef.ShowWindow(MainGameViewRef._menuWindow);
    }


    public void SetSoloGameAndRestart() {
        isSoloGame = true;
        MainGameViewRef.UpdateCountersTitles(_gameData ,isSoloGame);
        RestartGame(true);
    }
    public void SetMultiGameAndRestart() {
        isSoloGame = false;
        MainGameViewRef.UpdateCountersTitles(_gameData, isSoloGame);
        RestartGame(true);
    }

    // When clicking Restart game
    public void RestartGame(bool isRestartedFromMenu) {
        
        GetSettingValues();// gets values from settings and sets correspondence values

        if (!isSoloGame) {//regular
            _gameData = new GameData(PlayerAmount, _playersNames, IsGameWithMissiosQuestions(), _language);
        }
        else {//solo
            _gameData = new GameData(1, _playersNames, false, _language);
        }
       

        InitializeBeforeGameStart(isRestartedFromMenu); //Initializes data before game

        // StartCoroutine(TimeToEndCoroutine()); //Starts counting _startingTime seconds, than game is over
      //  FlipAllCards(); //Flip all cards faced down
        MainGameViewRef.FlipAllCards(_gameData);
        for (int i = 0; i < _gameData._gameDeck.Length; i++) {
            _gameData._gameDeck[i].SetIsCardFaceUp(false);
        }
        if (!isRestartedFromMenu) {
            MainGameViewRef.HideWindow(MainGameViewRef._menuWindow); //Hides menu window
        }

        MainGameViewRef.UpdateCountersTitles(_gameData, isSoloGame);
        UpdateViewRef();
        //MainGameViewRef.UpdateAllTextsLanguage(_gameData);


    }

    private void InitializeBeforeGameStart(bool isRestartedFromMenu) {
        

        //Creates and shuffles game deck
        _gameData._gameDeck = new Card[_amountOfCardsOnField * 2];
        CreateDeck();
        _gameData._gameDeck = ShuffleDeck(_gameData._gameDeck);
        _gameData._chosenCard = new Card();
        _gameData._lastPositionInDeck = -1;
        _gameData._amountOfCardsFacedUp = 0;
        _waitingForCardsFlipCoroutine[0] = false;
        _waitingForCardsFlipCoroutine[1] = false;
        _waitingForHaltCoroutine = false;

        //Windows handler
        MainGameViewRef.HideWindow(MainGameViewRef._settingsWindow);//Closing setting window
        MainGameViewRef.HideWindow(MainGameViewRef._languageWindow);//Closing language window
        MainGameViewRef.HideWindow(MainGameViewRef._howToPlayWindow);//Closing how to play window
        if (isRestartedFromMenu) {
            MainGameViewRef.HideWindow(MainGameViewRef._pauseWindow);//Closing pause window
        }
        MainGameViewRef.HideWindow(MainGameViewRef._missQuesWindow);//Closing missions window
        MainGameViewRef.isSettingsWindowActive = false;
        MainGameViewRef.isLanguageWindowActive = false;
        MainGameViewRef.isHowToPlayWindowActive = false;
        MainGameViewRef.isMenuWindowActive = false;
        MainGameViewRef.isPauseWindowActive = false;
        MainGameViewRef.isMissQuesWindowActive = false;
        MainGameViewRef.PauseButton.GetComponent<Button>().interactable = true;

        MainGameViewRef._cardToShowOnMatch.SetActive(false);
        MainGameViewRef.StopWINParticles();

        _gameData.isGameRun = true;
        for (int i = 0; i < _gameData.IsCardBeingPlayed.Length; i++) {
            _gameData.IsCardBeingPlayed[i] = false;
        }

        UpdateViewRef();
       //  MainGameViewRef.UpdateView(_gameData);
    }


    /// <summary>
    /// Setting game deck with two copies of each card
    /// </summary>
    // TODO: when creating cards, change the subtype boolean accordingly (i.e if it is a Tip/Offer card)
    private void CreateDeck() {
        for (int i = 0; i < (_gameData._gameDeck.Length / 2); i++) {

            int rnd = 1;
            do {
                rnd = UnityEngine.Random.Range(1, MainGameViewRef.CARDS_SPRITES.Length);
            } while (_gameData.IsCardBeingPlayed[rnd - 1]);

            _gameData.IsCardBeingPlayed[rnd - 1] = true;

            Card tempCard = new Card(Card.CardType.Mission, false, rnd - 1, false, _gameData);
            _gameData._gameDeck[i] = tempCard;//first copy

            Card tempCard2 = new Card(Card.CardType.Question, false, rnd - 1, false, _gameData);
            _gameData._gameDeck[i + _gameData._gameDeck.Length / 2] = tempCard2;//second copy
        }
    }



    public int GetCurPlayer() {
        return _gameData.GetWhosTurnIsIt();
    }

    public void SetCurPlayer(int nextPlayer) {
        _gameData.SetWhosTurnIsIt(nextPlayer);
    }

    public int GetNextPlayer() {
        if (_gameData.GetWhosTurnIsIt() + 1 == _gameData.PlayersList.Count) {
            return 0;
        }
        else {
            return _gameData.GetWhosTurnIsIt() + 1;
        }
     
    }

    public void UpdateViewRef() {
        MainGameViewRef.UpdateView(_gameData);
    }

    public void BringCardToFront() {
        MainGameViewRef.MoveCardToFront(_gameData);
    }
    public void BringCardToBack() {
        MainGameViewRef.MoveCardToBack(_gameData);
    }



    /// <summary>
    /// Get a deck, shuffle it, and return a differet shuffled deck
    /// </summary>
    /// <param name="deck"></param>
    /// <returns></returns>
    private Card[] ShuffleDeck(Card[] deck) {
        for (int i = deck.Length - 1; i > 1; i--) {
            int rnd = UnityEngine.Random.Range(0, i + 1);
            Card tempCard = deck[rnd];
            deck[rnd] = deck[i];
            deck[i] = tempCard;
        }
        return deck;
    }

    //Getting the i'th card
    public Card GetCard(int i) {
        return _gameData._gameDeck[i];
    }


    /// <summary>
    ///  Returns wanted card sprite from basic sprites.
    /// </summary>
    /// <param name="i"> Sprite in CARDS_SPRITES. at [0] there is back of a card </param>
    /// <returns> a card sprite </returns>
    public SpriteRenderer GetBasicSprite(int i) {
        return MainGameViewRef.CARDS_SPRITES[i];
    }

    /// <summary>
    /// Returns wanted card sprite FROM CARDS ON FIELD
    /// </summary>
    /// <param name="i"> Sprite in _gameCardsSprites.[0] to [15] </param>
    /// <returns> a card sprite </returns>
    public GameObject GetSpriteFromGameCardSprite(int i) {
        return MainGameViewRef.GameCards[i];
    }

    public int GetAmountOfCards() {
        return _amountOfCardsOnField * 2;
    }

    public Card GetChosenCard() {
        return _gameData._chosenCard;
    }
    public void SetChosenCard(Card card) {
        _gameData._chosenCard = card;
    }

    public void ExitGameToMenu() {
        GameOver(true); // game over
    }

    public bool GetIsGamePaused() {
        return (MainGameViewRef.isPauseWindowActive || MainGameViewRef.isMissQuesWindowActive);
    }

    //Checks if player on. IF SO, game over and won
    public void CheckIfWon() {
        if (_gameData._amountOfCardsFacedUp == _amountOfCardsOnField * 2) {
            //FOR SOLO GAMES
            if (isSoloGame) {
                if (_gameData.PlayersList[0].GetScore() < _bestScore) {
                    _bestScore = _gameData.PlayersList[0].GetScore();
                    SaveBestScore(_bestScore);
                }
            }
            _gameData.isGameRun = false;
            MainGameViewRef.UpdatPauseMenu(_gameData);
            GameOver(false); // game over
        }
    }

 
    public void AddScoreToPlayer(int player, int scoreToAdd) {
        _gameData.PlayersList[player].AddScore(scoreToAdd);
    }


    public void TogglePauseMenu() {
        if (!_gameData.isPauseMenuToFrontCoroutineRun) {
            
            MainGameViewRef.UpdatPauseMenu(_gameData);
            if (!MainGameViewRef.isPauseWindowActive) {
                MovePauseMenuDown();
            }
            else {
                MovePauseMenuUp();
            //    MainGameViewRef.isPauseWindowActive = true;
            }
        }
     
    }

    public void ToggleMissQuesMenu() {
        // call funtion from MainGameViewRef that puts wanted text on field

        MainGameViewRef.UpdateMissQuesManu(_gameData);
        MainGameViewRef.ToggleMissQuesWindow();
        if (!MainGameViewRef.isMissQuesWindowActive) {
            CheckIfWon();
            
        }
      
    }

    private void OneMoreTurn() {
        _gameData.isOneMoreTurnActive = true;
    }

    public bool GetIsOneMoreTurn() {
        return _gameData.isOneMoreTurnActive;
    }
    public void SetIsOneMoreTurn(bool toSet) {
        _gameData.isOneMoreTurnActive = toSet;
    }

    //POINTS HANDLER
    private void AddPointsToPlayer(int pointsToAdd) {
        _gameData.PlayersList[GetCurPlayer()].AddScore(pointsToAdd);
        MainGameViewRef.PlayGotScorAnim(true);
    }

    private void AddPointsToBoth(int pointsToAdd) {
        _gameData.PlayersList[GetCurPlayer()].AddScore(pointsToAdd);
        _gameData.PlayersList[GetNextPlayer()].AddScore(pointsToAdd);
        MainGameViewRef.PlayGotScorAnim(true);
    }

    private void AddPointsToEveryone(int pointsToAdd) {
        for (int i = 0; i < _gameData.PlayersList.Count; i++) {
            _gameData.PlayersList[i].AddScore(pointsToAdd);
        }
        MainGameViewRef.PlayGotScorAnim(true);
    }

    private void ReducePointsToPlayer(int pointsToAdd) {
        _gameData.PlayersList[GetCurPlayer()].ReduceScore(pointsToAdd);
        MainGameViewRef.PlayGotScorAnim(false);
    }


    //Controls all missions! One more turn, add score to playes etc...
    public void GetScoreAndCloseMissQuesWindow() {
        if (!MainGameViewRef.isPauseWindowActive) {


            if (!_gameData.isBringCardToFrontCoroutineRun) {

                //TODO: different missions that adding different amount of score to different players

                switch (_gameData._chosenCard.GetValue()) {
                    default:
                        break;
                    case 0:
                        AddPointsToPlayer(10);
                        break;
                    case 1:
                        AddPointsToPlayer(5);
                        break;
                    case 2:
                        AddPointsToPlayer(10);
                        break;
                    case 3:
                        AddPointsToPlayer(10);
                        break;
                    case 4:
                        AddPointsToBoth(10);
                        break;
                    case 5:
                        AddPointsToPlayer(10);
                        break;
                    case 6:
                        AddPointsToBoth(10);
                        break;
                    case 7:
                        AddPointsToPlayer(15);
                        break;
                    case 8:
                        AddPointsToPlayer(15);
                        break;
                    case 9:
                        AddPointsToPlayer(10);
                        break;
                    case 10:
                        AddPointsToPlayer(5);
                        break;
                    case 11:
                        AddPointsToPlayer(10);
                        break;
                    case 12:
                        AddPointsToPlayer(10);
                        break;
                    case 13:
                        AddPointsToPlayer(10);
                        break;
                    case 14:
                        AddPointsToBoth(10);
                        break;
                    case 15:
                        AddPointsToBoth(10);
                        break;
                    case 16:
                        AddPointsToPlayer(10);
                        break;
                    case 17:
                        AddPointsToPlayer(10);
                        break;
                    case 18:
                        AddPointsToPlayer(10);
                        break;
                    case 19:
                        AddPointsToPlayer(10);
                        break;
                    case 20:
                        AddPointsToPlayer(10);
                        break;
                    case 21:
                        AddPointsToPlayer(10);
                        break;
                }

                MainGameViewRef.PlayMatchParticles();
                MainGameViewRef.UpdatPauseMenu(_gameData);
                MainGameViewRef.UpdateView(_gameData);

                BringCardToBack();
                ToggleMissQuesMenu();
            }
        }
    }

    public void CloseMissQuesWindow() {
        if (!MainGameViewRef.isPauseWindowActive) {

            if (!_gameData.isBringCardToFrontCoroutineRun) {
                if (_gameData._chosenCard.GetValue() == 18 || _gameData._chosenCard.GetValue() == 20) {
                    OneMoreTurn();
                    soundHandler.PlayGotScore();
                }
                else if (_gameData._chosenCard.GetValue() == 1 || _gameData._chosenCard.GetValue() == 11) {
                    AddPointsToPlayer(5);
                    MainGameViewRef.UpdateCounters(_gameData);
                    soundHandler.PlayGotScore();
                    MainGameViewRef.PlayMatchParticles();
                }

                BringCardToBack();
                ToggleMissQuesMenu();
            }
        }
    }

    public void MoveMenuUp() {
        soundHandler.PlaySwash();
        _gameData.isBringMenuToFrontCoroutineRun = true;
        StartCoroutine(MoveMenuOverSeconds(_gameData, true));
    }
    public void MoveMenuDown() {
        
        UpdateTextsLanguageView();
        soundHandler.PlaySwash();
        _gameData.isBringMenuToFrontCoroutineRun = true;
        StartCoroutine(MoveMenuOverSeconds(_gameData, false));
    }


    private IEnumerator MoveMenuOverSeconds(GameData gameData,bool moveUp) {
        float time = .3f;
        Vector3 startPosition = new Vector3(-7.7248e-05f, -0.00047111f, 0f);
        Vector3 endPosition = new Vector3(-7.7248e-05f, 148.3f, 0f);
        float elapsedTime = 0;

        if (moveUp) {
            while (elapsedTime < time) {
                MainGameViewRef._menuWindow.transform.localPosition = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            MainGameViewRef._menuWindow.transform.localPosition = startPosition;
            MainGameViewRef.HideWindow(MainGameViewRef._menuWindow); //Hides menu window
        }
        else {
            ExitGameToMenu();
            while (elapsedTime < time) {
                MainGameViewRef._menuWindow.transform.localPosition = Vector3.Lerp(endPosition, startPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            MainGameViewRef._menuWindow.transform.localPosition = startPosition;
        }

          gameData.isBringMenuToFrontCoroutineRun = false;
    }


    public void MovePauseMenuUp() {
        soundHandler.PlaySwash();
        _gameData.isPauseMenuToFrontCoroutineRun = true;
        StartCoroutine(MovePauseWindowOverSeconds(_gameData, true));
    }
    public void MovePauseMenuDown() {
        soundHandler.PlaySwash();
        _gameData.isPauseMenuToFrontCoroutineRun = true;
        StartCoroutine(MovePauseWindowOverSeconds(_gameData, false));
    }

    private IEnumerator MovePauseWindowOverSeconds(GameData gameData, bool moveUp) {
        float time = .2f;
        Vector3 startPosition = new Vector3(9.9301e-05f, 3.82f, 0f);
        Vector3 endPosition = new Vector3(9.9301e-05f, 120f, 0f);
        float elapsedTime = 0;

        if (moveUp) {
            while (elapsedTime < time) {
                MainGameViewRef._pauseWindow.transform.localPosition = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            MainGameViewRef._pauseWindow.transform.localPosition = startPosition;
            MainGameViewRef.HideWindow(MainGameViewRef._pauseWindow); //Hides menu window
            MainGameViewRef.isPauseWindowActive = false;
        }
        else {//Game restarted
            MainGameViewRef.ShowWindow(MainGameViewRef._pauseWindow); //show pause window
            MainGameViewRef.isPauseWindowActive = true;
            while (elapsedTime < time) {
                MainGameViewRef._pauseWindow.transform.localPosition = Vector3.Lerp(endPosition, startPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            MainGameViewRef._pauseWindow.transform.localPosition = startPosition;
        }

        gameData.isPauseMenuToFrontCoroutineRun = false;
    }

    private void GameOver(bool exitFromPause) {
        _gameData.isGameRun = false;
        MainGameViewRef.PauseButton.GetComponent<Button>().interactable = false;
      //  MainGameViewRef.UpdatPauseMenu(_gameData);

        if (exitFromPause) {
            MainGameViewRef.isMenuWindowActive = true;
            MainGameViewRef.HideWindow(MainGameViewRef._missQuesWindow);
    
            MainGameViewRef.ShowWindow(MainGameViewRef._menuWindow);
    

        }
            else {
            MainGameViewRef.isMenuWindowActive = false;
            MainGameViewRef.PlayWINParticles();
            soundHandler.PlayWin();
            MainGameViewRef.HideWindow(MainGameViewRef._missQuesWindow);
            MainGameViewRef.HideWindow(MainGameViewRef._menuWindow);
            MovePauseMenuDown();
        }

       
    }

  

    //Gets all values from settings window, the set the correspondence values
    public void GetSettingValues() {
        PlayerAmount = AmountOfPlayersDropDown.value + 2; //gets amount of player from settings

        _gameData._isMissQuesGame = WithMissionsToggle.isOn;

        int tempIndexChosen = AmountOfCardsOnFieldDropDown.value;
        if (tempIndexChosen == 0) {
            _amountOfCardsOnField = 8;
        }
        else if (tempIndexChosen == 1) {
            _amountOfCardsOnField = 10;
        }
        else if (tempIndexChosen == 2) {
            _amountOfCardsOnField = 12;
        }

        SavePlayersNamesFromInputFields();

        //for (int i = 0; i < PlayerAmount; i++) {
        //    _playersNames[i] = InputFieldsForPlayersNames[i].GetComponent<InputField>().text;
        //    SaveName(_playersNames[i], i);
        //}
    }

    public void HideWantedInputFields() {
        for (int i = 0; i < _playersNames.Length; i++) {
            if (i < PlayerAmount) {
                InputFieldsForPlayersNames[i].SetActive(true);
            }
            else {
                InputFieldsForPlayersNames[i].SetActive(false);
            }
        }
    }

  


    //Amount of cards face up
    public int GetAmountOfCardsFacedUp() {
        return _gameData._amountOfCardsFacedUp;
    }
    public void AddAmountOfCardsFacedUpByOne() {
        _gameData._amountOfCardsFacedUp++;
    }
    public void ReduceAmountOfCardsFacedUpByOne() {
        _gameData._amountOfCardsFacedUp--;
    }
    public void SetAmountOfCardsFacedUp(int amount) {
        _gameData._amountOfCardsFacedUp = amount;
    }

    public int GetLastPositionInDeck() {
        return _gameData._lastPositionInDeck;
    }
    public void SetLastPositionInDeck(int lastPos) {
        _gameData._lastPositionInDeck = lastPos;
    }


    public void SetLanguage() {
        _language = LanguageropDown.value;
     
        
        //TODO: Change language number
        //TODO: Change all screens in main menu with language
    }


    public bool IsGameWithMissiosQuestions() {
        return _gameData._isMissQuesGame;
    }


    public void UpdateTextsLanguageView() {
        MainGameViewRef.UpdateAllTextsLanguage(_language, _bestScore);
    }



    public void IsAnyCharacterRightToLeft(int playerNumber) {
        string pattern = @"[\u0591-\u07FF]";
        string tempText = InputFieldsForPlayersNames[playerNumber].GetComponent<InputField>().text;
        if (Regex.IsMatch(tempText, pattern)) {
            InputFieldsForPlayersNames[playerNumber].GetComponent<InputField>().text = Reverse(tempText);
        }
        _gameData.PlayersList[playerNumber].SetName(InputFieldsForPlayersNames[playerNumber].GetComponent<InputField>().text);
        _playersNames[playerNumber] = _gameData.PlayersList[playerNumber].GetName();
    }

    private string Reverse(string s) {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    ///  Returns if game is running
    /// </summary>
    /// <returns> if game is running </returns>
    public bool IsGameRunning() {
        return _gameData.isGameRun;
    }

    private void SetFirstGameEver() {
        PlayerPrefs.SetInt("First Game", 0);
    }

    private bool GetIsFirstGameEver() {
        return PlayerPrefs.HasKey("First Game");
    }

    public void SaveBestScore(int bestScore) {
        PlayerPrefs.SetInt("BestScore", bestScore);
    }
    private void LoadBestScore() {
        _bestScore = PlayerPrefs.GetInt("BestScore");
    }

    public void SaveLanguage() {
        PlayerPrefs.SetInt("Language", _language);
    }
    private void LoadLanguage() {
        _language = PlayerPrefs.GetInt("Language");
        LanguageropDown.value = _language;


    }

    public void SavePlayersNamesFromInputFields() {
   
        for (int i = 0; i < _playersNames.Length; i++) {
            _playersNames[i] = InputFieldsForPlayersNames[i].GetComponent<InputField>().text;
            SaveName(_playersNames[i], i);
        }
        
    }

    private void SaveName(string nameToSave, int pos) {
        //   _playersNames[pos] = InputFieldsForPlayersNames[pos].GetComponent<InputField>().text;
        _playersNames[pos] = nameToSave;
        PlayerPrefs.SetString("name" + pos, nameToSave);
    }

    private void LoadNames() {
        MainGameViewRef.ShowWindow(MainGameViewRef._settingsWindow);
        MainGameViewRef.ShowWindow(MainGameViewRef._menuWindow);
        for (int i = 0; i < _playersNames.Length; i++) {
          
            _playersNames[i] = PlayerPrefs.GetString("name" + i);
            InputFieldsForPlayersNames[i].GetComponent<InputField>().text = _playersNames[i];
     
        }
       
        InputFieldsForPlayersNames[3].SetActive(false);
        InputFieldsForPlayersNames[4].SetActive(false);
        MainGameViewRef.HideWindow(MainGameViewRef._settingsWindow);
     
    }
}