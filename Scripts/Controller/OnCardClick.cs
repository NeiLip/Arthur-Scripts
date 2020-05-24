using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnCardClick : MonoBehaviour {
    public GameObject _cardGameSprite;
    public GameHandler gameHandler;
    //  public GameData gameData;

    private const int FPS = 100;
    private const float RTS = 1440; // rotations degree per second
    private const float LIMIT_DEGREE = 180f;
    private float waitTime;

    private int _cardLocation;

    public void Awake() {
        waitTime = 1f / FPS;

        System.Int32.TryParse(_cardGameSprite.name, out _cardLocation);
        // gameData = gameHandler._gameData;
    }

    private bool GetBothCardsCoroutines() {
        return (gameHandler._waitingForCardsFlipCoroutine[0] && gameHandler._waitingForCardsFlipCoroutine[1]);
    }

    /// <summary>
    /// When clicked on ANY card on the field
    /// </summary>
    public void OnMouseDown() {
        if (!gameHandler._waitingForHaltCoroutine && gameHandler.IsGameRunning() && !gameHandler.GetIsGamePaused() && !GetBothCardsCoroutines()) { //Makes sure game is running and haltCoroutine is not running
        //    for (int i = 0; i < gameHandler.GetAmountOfCards(); i++) {
             //   string tempCard = i.ToString();// getting wanted card
                

            if (!gameHandler.GetCard(_cardLocation).GetIsCardFaceUp()) { //selecting the card and making sure it is not faced up
                gameHandler._waitingForCardsFlipCoroutine[0] = true;
                StartCoroutine(FlipCardAnimationCoroutine(true, _cardGameSprite, 0));//Flip card animation

                _cardGameSprite.GetComponent<SpriteRenderer>().sprite = gameHandler.GetBasicSprite(gameHandler.GetCard(_cardLocation).GetValue() + 1).sprite;//Value + 1 because [0] contains back of a card

                //If it enters it means that there was already one card faced up
                if (gameHandler.GetAmountOfCardsFacedUp() % 2 == 1) {
                    if (gameHandler.GetChosenCard().GetValue() == gameHandler.GetCard(_cardLocation).GetValue()) { //If same value is already faced-up-card or it is the first card to play

                        gameHandler.MainGameViewRef.PlayMatchParticles();
                        gameHandler.soundHandler.PlayMatch();

                        PlaceCard(_cardLocation);

                        //Adding 10 POINTS move
                        if (!gameHandler.isSoloGame) {
                            gameHandler.AddScoreToPlayer(gameHandler.GetCurPlayer(), 10);
                            gameHandler.MainGameViewRef.PlayGotScorAnim(true);
                        }
                        else {//adding 1
                            gameHandler.AddScoreToPlayer(gameHandler.GetCurPlayer(), 1);
                        }
                    
                        gameHandler.UpdateViewRef();

                        if (gameHandler.IsGameWithMissiosQuestions()) { //if using Missions
                            gameHandler.ToggleMissQuesMenu();
                            gameHandler.BringCardToFront();
                        }
                        else {
                            gameHandler.CheckIfWon();
                        }
                      
                    }
                    else {//Flipping back cards
                        gameHandler.soundHandler.PlayFlipCard();
                        gameHandler._waitingForHaltCoroutine = true;
                        StartCoroutine(FlipCardsBackCoroutine()); //Waits befor flipping cards back
                    }
                }
                else { // There are even cards on field. Place the i'th (odd number) card
                    PlaceCard(_cardLocation);
                }
            }
        }
    }

    /// <summary>
    ///  Flipping back two last cards
    /// </summary>
    private IEnumerator FlipCardsBackCoroutine() {
        yield return new WaitForSeconds(gameHandler.HALT_TIME);
        gameHandler._waitingForCardsFlipCoroutine[0] = true;
        StartCoroutine(FlipCardAnimationCoroutine(false, _cardGameSprite, 0));//Flip card animation
        gameHandler._waitingForCardsFlipCoroutine[1] = true;
        StartCoroutine(FlipCardAnimationCoroutine(false, gameHandler.GetSpriteFromGameCardSprite(gameHandler.GetLastPositionInDeck()), 1));//Flip card animation

        gameHandler.soundHandler.PlayFlipCardBack();

        _cardGameSprite.GetComponent<SpriteRenderer>().sprite = gameHandler.GetBasicSprite(0).sprite; // Flipping back last card 
        gameHandler.GetSpriteFromGameCardSprite(gameHandler.GetLastPositionInDeck())
            .GetComponent<SpriteRenderer>().sprite = gameHandler.GetBasicSprite(0).sprite;// Flipping back previos chosen card

        gameHandler.ReduceAmountOfCardsFacedUpByOne();//Reduce just one (eventhough we flipped 2 cards) because we didn't add when drawing card)
        if (!gameHandler.GetIsOneMoreTurn()) {
            Debug.Log("Previous player was " + gameHandler.GetCurPlayer());

            if (gameHandler.GetCurPlayer() == gameHandler.PlayerAmount - 1) {
                gameHandler.SetCurPlayer(0);
            }
            else {
                if (gameHandler.isSoloGame) {//solo game
                    gameHandler.AddScoreToPlayer(gameHandler.GetCurPlayer(), 1);
                }
                else {//multi game
                    gameHandler.SetCurPlayer(gameHandler.GetCurPlayer() + 1);
                }
               
            }
            Debug.Log("Current player is " + gameHandler.GetCurPlayer());
            Debug.Log("------------------------------------------------------------------------");
        }
        else {
            gameHandler.SetIsOneMoreTurn(false);
        }
      
        gameHandler.UpdateViewRef();
        gameHandler.GetCard(gameHandler.GetLastPositionInDeck()).SetIsCardFaceUp(false);
        gameHandler._waitingForHaltCoroutine = false;
    }

    /// <summary>
    /// Placing the i'th card
    /// </summary>
    /// <param name="i"> i'th card in game deck </param>
    private void PlaceCard(int i) {
        gameHandler.soundHandler.PlayFlipCard();
        gameHandler.SetLastPositionInDeck(i);
        gameHandler.SetChosenCard(gameHandler.GetCard(i));
        gameHandler.AddAmountOfCardsFacedUpByOne();
        gameHandler.GetCard(i).SetIsCardFaceUp(true);
        gameHandler.UpdateViewRef();
    }


    /// <summary>
    /// Flip animation coroutine
    /// </summary>
    /// <param name="FlipFaceUp"> True if the card to flip should be faced up </param>
    /// <param name="cardGameObject"> the card GameObject that needs to be flipped </param>
    /// <returns></returns>
    private IEnumerator FlipCardAnimationCoroutine(bool FlipFaceUp, GameObject cardGameObject, int coroutineNumberToChange) {
        bool done = false;

        if (FlipFaceUp) {//If needs to flip card FACE UP
            while (!done) {
                float degree = RTS * Time.deltaTime;
                cardGameObject.transform.Rotate(new Vector3(0, degree, 0));  //Rotate foreward

                if (LIMIT_DEGREE < cardGameObject.transform.eulerAngles.y) { //Till reach limit
                    done = true;
                }
                yield return new WaitForSeconds(waitTime);
            }
    
            //Sometimes card wasn't exactly on LIMIT_DEGREE (exmpl: was on 179.99823 insted of 180), so change it manualy at end of animation
            cardGameObject.transform.rotation = Quaternion.Euler(0, LIMIT_DEGREE, 0);
            cardGameObject.transform.GetChild(0).gameObject.SetActive(true); // Show text on card
            cardGameObject.GetComponentInChildren<Text>().transform.rotation = Quaternion.identity;
        }

        else {//If needs to flip card FACE DOWN
            cardGameObject.transform.GetChild(0).gameObject.SetActive(false);// Hides text on card

            while (!done) {
                float degree = RTS * Time.deltaTime;
                cardGameObject.transform.Rotate(new Vector3(0, -degree, 0)); //Rotate backward

                if (LIMIT_DEGREE < cardGameObject.transform.eulerAngles.y) { //Till reach limit
                    done = true;
                }
                yield return new WaitForSeconds(waitTime);
            }
            //Sometimes card wasn't exactly on LIMIT_DEGREE (exmpl: was on 0.1001204 insted of 0) so change it manualy at end of animation
            cardGameObject.transform.rotation = Quaternion.identity;
        }

        gameHandler._waitingForCardsFlipCoroutine[coroutineNumberToChange] = false;
    }
}