using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card {

    //additional types can be added later on.
    //our cards are all from Viking type
    public enum CardType {
        Empty,
        Mission,
        Question,
    }

    private CardType _cardType;
    private bool _isSubType; //i.e if it is a tip/offer card
    private int _value; //the value chooses the image later on. So, when we want to know if two cards are identical, just check their values
    private bool _isCardFaceUp;

    private bool _isCardUsed;

    private string _cardTitleText;
    private string _cardMissionText;
    private string _cardQuestionText;


    //Default card builder
    public Card(){
        _cardType = CardType.Empty;
        _value = -1; // -1 means the value field is empty
        _isCardFaceUp = false;
        _isCardUsed = false;
        _isSubType = false;
    }

    //Card with data builder
    public Card(CardType cardType, bool isSubType,int value , bool isCardFaceUp, GameData gameData) {
        _cardType = cardType;
        _value = value;
        _isCardFaceUp = isCardFaceUp;
        _isCardUsed = false;
        _cardMissionText = gameData.MissionsTexts[value % gameData.MissionsTexts.Length];
        _cardQuestionText = gameData.QuestionsTexts[value % gameData.QuestionsTexts.Length];
        _cardTitleText = gameData.CardsTitleTexts[value % gameData.CardsTitleTexts.Length];
        _isSubType = isSubType;
    }


    public string GetCardTitleText() {
        return _cardTitleText;
    }
    public string GetCardMissionText() {
        return _cardMissionText;
    }
    public string GetCardQuestionText() {
        return _cardQuestionText;
    }

    public void SetValue(int value) {
        _value = value;
    }
    public void SetCardType(CardType cardType) {
        _cardType = cardType;
    }
    public int GetValue() {
        return _value;
    }
    public CardType GetCardType() {
        return _cardType;
    }
    public void SetIsCardFaceUp(bool thisBool) {
        _isCardFaceUp = thisBool;
    }
    public bool GetIsCardFaceUp() {
        return _isCardFaceUp;
    }

    public void SetIsCardUsed(bool thisBool) {
        _isCardUsed = thisBool;
    }
    public bool GetIsCardUsed() {
        return _isCardUsed;
    }
}

