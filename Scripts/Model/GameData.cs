using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class GameData  {

    //Interface
    public int Language = 0;//0 for English, 1 for Hebrew

    //Game Type Handling
    public bool _isMissQuesGame; // is game with missions and questions
    public bool _isSoloGame = false; // is game with missions and questions

    //Coroutines
    public bool isBringCardToFrontCoroutineRun;
    public bool isBringMenuToFrontCoroutineRun;
    public bool isPauseMenuToFrontCoroutineRun;


    //PLAYERS handling
    public List<List<Card>> PlayersCards;
    public List<Player> PlayersList;
    private int _whosTurnIsIt;
    public bool isOneMoreTurnActive;
    [HideInInspector]
    public Player WinnerPlayer;


    //CARDS Handling
    public Card[] _gameDeck; //Game Deck. Is shuffled before game starts
    public Card _chosenCard; //The card that is already faced up (to check if there is a match with next card)
    public int _lastPositionInDeck;// Last position in deck of _chosenCard
    public int _amountOfCardsFacedUp;//How many cards are faced up. It is used to check if game is over or to know how to handle next card (OnCardClick script)
    public bool[] IsCardBeingPlayed = new bool[22];



    public string[] CardsTitleTexts = new string[]{
        "Phone Mate", 
        "Relax Bubbles",
        "Inside Love", 
        "Worth K words",
        "Sweet Home",
        "Calm Count",
        "My Picture",
        "Pen Pal",
        "Cheer Alone",
        "It's Live",
        "Warm Hug",
        "Memories",
        "Chore Helper!",
        "My story",
        "Play&Enjoy",
        "My Feeling",
        "Yum Yum",
        "My Song",
        "I Love...",
        "Relaxing Jumps",
        "Dancing Time",
        "Laughy Laugh!"
    };
     /// <summary>
     /// ////////////////////////////   NOT UPDATED  !!!!
     /// </summary>
    public string[] MissionsTexts = new string[]{
         //"Say the phone number of the friend near you.\n+10 for you if you succeed!",//0
         //"Pantomime: Blow up a bloon and make it blast!\n+10 for you if you accept",//1
         //"Say a name of a person you like. What do like about him?\n+5 for you",//2
         //"Take a selfi with all the participants. Make it your screen for one hour.\n+15 for you if you accept.",//3
         //"Pantomime next player: A place in your home you like the most.\n If he answers correctly, +10 for both of you.",//4
         //"'Draw' (with your finger) all numbers from 1 to 30 in the air.\n+10 for you if you succeed!",//5
         //"'Draw' (with your finger) on the back of next player any kind of emoji.\nIf he answers correctly you both get +5",//6
         //"'Yesterday morning Arthur woke up, when suddenly...' Continue this story with at least 10 words.\n+10 for you if you accept.",//7
         //"Cheer up next player by improvising a song for him!\n You both get +10 if you accept.",//8
         //"Make a funny face and make at least one of the players laugh.\n +5 if you succeed.",//9
         //"Give a hug to next player.\n+5 for both of you if you both accept.",//10
         //"Arthur offers you one more turn. He'll take 5 points from you in return.\nDo you accept?",//11
         //"Say 3 type of dishes you like that start with the latter A.\n +10 if you succeed.",//12
         //"Arthur offers you one more turn. He'll take 5 points from you in return.\nDo you accept?",//13
         //"Arthur offers you one more turn. He'll take 5 points from you in return.\nDo you accept?",//14
         //"Pantomime next player: Any kind of emoji.\nIf he answers correctly, +10 for both of you.",//15
         //"While jumping on one leg, explain how to make an Omlette.\n+10 for you if you succeed!",//16
         //"Sing a song you like but replace all words with 'La La La..'.\n+10 for you if someone recognize the song.",//17
         //"Arthur offers you one more turn. He'll take 5 points from you in return.\nDo you accept?",//18
         //"Jump on each leg 8 times while clappign hands.\n +10 if you succeed.",//19
         //"Mission21",//20
         //"Tell something that made you laugh latly.\n+10 for you if you accept",//21
    };

    public string[] QuestionsTexts = new string[]{
         //"Who's the last person spoke with you on phone?",
         //"Question2",
         //"Tip: Arthur",
         //"Tip: Arthur",
         //"What is your favorite place in the house?",
         //"Tip: Arthur",
         //"What is you favorite color? Why?",
         //"Question8",
         //"What do you like about yourself?",
         //"Tip:",
         //"Question11",
         //"Who do think is missing you?",
         //"What chore do you like the most?",
         //"What was the last time you cried?",
         //"What is your favorite game?",
         //"Arthur is a little bit sad... You get another turn!",
         //"What the three dishes you like the most?",
         //"Tip:",
         //"Say three places outside your house you would like to be at.",
         //"Tip",
         //"You get one more turn! Artuhr is dancing with joy for that!",
         //"What is the most funny thing you know?"
    };

    public readonly Vector3[] locations16 = new Vector3[]{
        new Vector3(-0.08f, -3.27f, 0f), new Vector3(-1.52f, -3.27f, 0f),
        new Vector3(-2.95f, -3.27f, 0f), new Vector3(-4.43f, -3.27f, 0f),

        new Vector3(-1.52f, -0.83f, 0f), new Vector3(-2.95f, -0.83f, 0f),
        new Vector3(-4.43f, -0.83f, 0f), new Vector3(-0.08f, -0.83f, 0f),

        new Vector3(-1.52f, 1.53f, 0f), new Vector3(-2.95f, 1.53f, 0f),
        new Vector3(-4.43f, 1.53f, 0f), new Vector3(-0.08f, 1.53f, 0f),

        new Vector3(-1.52f, 3.81f, 0f), new Vector3(-2.95f, 3.81f, 0f),
        new Vector3(-4.43f, 3.81f, 0f), new Vector3(-0.08f, 3.81f, 0f)
    };
    public readonly Vector3[] locations20 = new Vector3[]{
        new Vector3(-0.08f, -3.44f, 0f), new Vector3(-1.52f, -3.44f, 0f),
        new Vector3(-2.95f, -3.44f, 0f), new Vector3(-4.43f, -3.44f, 0f),

        new Vector3(-1.52f, -1.58f, 0f), new Vector3(-2.95f, -1.58f, 0f),
        new Vector3(-4.43f, -1.58f, 0f), new Vector3(-0.08f, -1.58f, 0f),

        new Vector3(-1.52f, 0.26f, 0f), new Vector3(-2.95f, 0.26f, 0f),
        new Vector3(-4.43f, 0.26f, 0f), new Vector3(-0.08f, 0.26f, 0f),

        new Vector3(-1.52f, 2.13f, 0f), new Vector3(-2.95f, 2.13f, 0f),
        new Vector3(-4.43f, 2.13f, 0f), new Vector3(-0.08f, 2.13f, 0f),

        new Vector3(-1.52f, 3.99f, 0f), new Vector3(-2.95f, 3.99f, 0f),
        new Vector3(-4.43f, 3.99f, 0f), new Vector3(-0.08f, 3.99f, 0f)
    };
    public readonly Vector3[] locations24 = new Vector3[]{
        new Vector3(-0.08f, -3.77999f, 0f), new Vector3(-1.52f, -3.77999f, 0f),
        new Vector3(-2.95f, -3.77999f, 0f), new Vector3(-4.43f, -3.77999f, 0f),

        new Vector3(-1.52f, -2.19f, 0f), new Vector3(-2.95f, -2.19f, 0f),
        new Vector3(-4.43f, -2.19f, 0f), new Vector3(-0.08f, -2.19f, 0f),

        new Vector3(-1.52f, -0.589f, 0f), new Vector3(-2.95f, -0.589f, 0f),
        new Vector3(-4.43f, -0.589f, 0f), new Vector3(-0.08f, -0.589f, 0f),

        new Vector3(-1.52f, 1.01f, 0f), new Vector3(-2.95f, 1.01f, 0f),
        new Vector3(-4.43f, 1.01f, 0f), new Vector3(-0.08f, 1.01f, 0f),

        new Vector3(-1.52f, 2.6f, 0f), new Vector3(-2.95f, 2.6f, 0f),
        new Vector3(-4.43f, 2.6f, 0f), new Vector3(-0.08f, 2.6f, 0f),

        new Vector3(-1.52f, 4.21f, 0f), new Vector3(-2.95f, 4.21f, 0f),
        new Vector3(-4.43f, 4.21f, 0f), new Vector3(-0.08f, 4.21f, 0f)
    };


    //////SETTING///////
    public int _amountOfCardsOnField;
    //private int _startingTime;
    public float _haltTime;
    public DataManagement.StorageOption _chosenStorageOption;

    public bool isGameRun = false;


    public int GetWhosTurnIsIt() {
        return _whosTurnIsIt;
    }

    public void SetWhosTurnIsIt(int NextPlayer) {
        _whosTurnIsIt = NextPlayer;
    }


    public GameData(int numOfPlayers, string[] playersNames, bool isMissquesGame, int language) {
        // DeckCards = new List<Card>();
        // PlayersCards = new List<List<Card>>();
        PlayersList = new List<Player>();

        Language = language;
      

        WinnerPlayer = new Player();

        for (int i = 0; i < numOfPlayers; i++) {
            PlayersList.Add(new Player());
            PlayersList[i].SetName(playersNames[i]);

            if (PlayersList[i].GetName() == "") {
                switch (language) {
                    case 0:
                        PlayersList[i].SetName("Player " + (i+1));
                        break;
                    case 1:
                        PlayersList[i].SetName((i + 1) + " ןקחש");
                        break;
                }
            }
        }
        _isMissQuesGame = isMissquesGame;

        _gameDeck = new Card[24];

        _whosTurnIsIt = Random.Range(0, numOfPlayers);
        isOneMoreTurnActive = false;

        SetAllTextsToWantedLanguage(Language);

        isBringCardToFrontCoroutineRun = false;
        isBringMenuToFrontCoroutineRun = false;
        isPauseMenuToFrontCoroutineRun = false;
    }

    //set winner and returns if there was a tie;
    public bool SetWonPlayerName() {
        bool isTie = false;
        WinnerPlayer = PlayersList[0];
        for (int i = 1; i < PlayersList.Count; i++) {
            if (PlayersList[i].GetScore() > WinnerPlayer.GetScore()) {
                WinnerPlayer = PlayersList[i];
                isTie = false;
            }
            else if (PlayersList[i].GetScore() == WinnerPlayer.GetScore()) {
                isTie = true;
            }
        }
        return isTie;
    }


    private void SetAllTextsToWantedLanguage(int language) {

            switch (language) {
                case 0:
                    CardsTitleTexts = new string[]{
        "Phone Mate", //0
        "Relax Bubbles", //1
        "Inside Love",//2
        "Worth K words",//3
        "Sweet Home",//4
        "Calm Counting",//5
        "My Picture",//6
        "Pen Pal",//7
        "Cheer Alone",//8
        "It's Live",//9
        "Warm Hug",//10
        "Friends",//11
        "Chore Helper!",//12
        "My story",//13
        "Play&Enjoy",//14
        "My Feeling",//15
        "Yum Yum",//16
        "My Song",//17
        "I Love...",//18
        "Relaxing Jumps",//19
        "Dancing Time",//20
        "Laughy Laugh!"//21
    };
                    MissionsTexts = new string[]{
         "Find 5 round objects around you.\n\nDid you get it right?\n+10",//0
         "Pantomime:\nBlow up a bloon and then make it explode!\n\nDid you do it? \n+5",//1
          "What is " + PlayersList[(_whosTurnIsIt + 1) % PlayersList.Count].GetName() + "’s favorite color?\n\nDid you get it right?\n+10",//2
         "Take a selfi with all the players.\n\nDid you do it?\n+10.",//3
         "Pantomime to " + PlayersList[(_whosTurnIsIt + 1) % PlayersList.Count].GetName() + ":\nA place in your house that you like.\n\nDid you get it right?\n+10 for both of you",//4
         "Draw in the air all numbers from 1 to 30.\n\nDid you do it?\n+10",//5
         "Draw on " + PlayersList[(_whosTurnIsIt + 1) % PlayersList.Count].GetName() + "’s back any emoji you like.\n\nDid you get it right?\n+10 for both of you",//6
         "Arthur is bored. Give him 3 ideas to relieve the boredom.\n\nDid you do it?\n+15",//7
         "Cheer up " + PlayersList[(_whosTurnIsIt + 1) % PlayersList.Count].GetName() + " by singing a song for him!\n\nDid you do it?\n+15.",//8
         "Make a funny face that will make everyone laugh!\n\nDid you succeed?\n+10",//9
         "Give a hug to one of the players.\n\nDid you do it?\n+5",//10
         "Talk about someone you miss.\n\nDid you do it?\n+10",//11
         "Say 3 types of dishes you like.\n\nDid you do it?\n+10",//12
         "Say something sad that happaned to you latley.\n\nDid you do it?\n+10",//13
        "Pantomime to " + PlayersList[(_whosTurnIsIt + 1) % PlayersList.Count].GetName() + ":\nThe last game you played.\n\nDid you get it right?\n+10 for both of you",//14
         "Pantomime to " + PlayersList[(_whosTurnIsIt + 1) % PlayersList.Count].GetName() + ":\nAny emoji you like.\n\nDid you get it right?\n+10 for both of you",//15
         "While jumping on one leg, explain how to make an Omlette.\n\nDid you do it?\n+10",//16
         "Sing a song you like but replace all words with 'La La La...'.\n\nDid someone recognize?\n+10",//17
         "Say 3 places outside your house you wish to be at.\n\nDid you do it?\n+10",//18
         "Jump 8 times on each leg while clapping your hands.\n\nDid you do it?\n+10",//19
         "Improvise a dance and all players should imitate your moves.\n\nDid you do it?\n+10",//20
         "Talk about something that made you laugh lately.\n\nDid you do it?\n+10",//21
    };

                    QuestionsTexts = new string[]{
         "Who is the last person you spoke with on the phone?",//0
         "Arthur is relaxed.\n\nYou get 5 points.",//1  //BONUS
         "What do you like about " + PlayersList[(_whosTurnIsIt + 1) % PlayersList.Count].GetName() + "?",//2
         "Arthur likes to take pictures with his lovers to preserve happy moments.",//3 //TIP
         "What is your favorite place in your house?",//4
         "When Arthur gets angry, he counts up to 30 to calm down.",//5  //TIP
         "What is you favorite color?",//6
         "What was your last chat on Whatsapp/Messanger?",//7
         "What do you like about yourself?",//8
         "When Arthur misses someone, he makes a video call with him.",//9   //TIP
         "Who do you like to be hugged by?",//10
         "Arthur takes care of his friends.\n\nYou get 5 points!",//11  //BONUS
         "Which home chore do you like the most?",//12
         "When was the last time you cried?",//13
         "What is your favorite game?",//14
         "When Arthur is feeling sad, he shares his feelings with his lovers and they help",//15  //TIP
         "What is your favorite dish?",//16
         "When Arthur is feeling upset or sad, he listens to music he likes.",//17  //TIP
         "Arthur loves you!\n\nYou get an extra turn!",//18  //BONUS
         "When Arthur is feeling afraid of something, he jumps in the air and becomes relaxed.",//19  //TIP
         "Arthur is dancing out of joy!\n\nYou get an extra turn!",//20  //BONUS
         "What is the most funny thing you saw recently?"//21
    };
                    break;
                case 1:
                    CardsTitleTexts = new string[]{
        "ינופלט רבח",
        "העגרהל תומישנ",
        "בלבש הבהא",
        "םילימ ףלא הווש",
        "קותמה יתיב",
        "תורפסמ תורפס",
        "ילש רויצה",
        "טעל רבח",
        "דודיבב דודיע",
        "יח רודיש",
        "םח קוביח",
        "דימת םירבח",
        "תיבב הרזע",
        "ילש רופיסה",
        "םינהנו םיקחשמ",
        "ילש שגרה",
        "ימאי ימאי",
        "ילש רישה",
        "...בהוא ינא",
        "תוררחשמ תוציפק",
        "דוקרל ןמז",
        "קוחצ יקוחצ"
    };
                MissionsTexts = new string[]{
         ".ךביבסמ םילוגע םירבד 5 אצמ\n\n?תחלצה םאה\n+10",
                          ":המימוטנפב\n.ותוא ץצופו לודג ןולב חפנ\n\n?תעציב םאה\n+5",
        "?"+PlayersList[(_whosTurnIsIt + 1) % (PlayersList.Count)].GetName() + " לע בוהאה עבצה המ" + "\n\n?תקדצ םאה\n+10",
            ".םיפתתשמה לכ ךלש יתצובק יפלס םלצ\n\n?תעציב םאה\n+10",
                                 ":המימוטנפב\n.תיבב בהוא התאש םוקמ "+PlayersList[(_whosTurnIsIt + 1) % (PlayersList.Count)].GetName()+"ל ראת\n\n?ןוכנ שחינ םאה\nםכינשל +10",
          ".30 דע 1 רפסמהמ ריוואב םירפסמ רייצ\n\n?תעציב םאה\n+15",
       ".ךתריחבל י׳גומא "+PlayersList[(_whosTurnIsIt + 1) % (PlayersList.Count)].GetName()+" לש בגה לע רייצ\n\n?ןוכנ שחינ םאה\nםכינשל +10",
          "תונויער 3 ול ןת ,םמעושמ רותרא\n.םומעישה תגפהל\n\n?תעציב םאה\n+15",
       "."+PlayersList[(_whosTurnIsIt + 1) % (PlayersList.Count)].GetName() + " תא דדועו דודיע תוציפק ץופק" + "\n\n?תעציב םאה\n+10",
          ".םיפתתשמה תא קיחציש קיחצמ ףוצרפ השע\n\n?תעציב םאה\n+10",
             ".םיפתתשמה דחאל קוביח ןת\n\n?תעציב םאה\n+5",
        ".וילא עגעגתמ התאש והשימ לע רפס\n\n?תעציב םאה\n+10",
              ".בהוא התאש םילכאמ 3 רומא\n\n?תעציב םאה\n+10",
        ".הנורחאל ךל הרקש בוצע והשמ לע רפס\n\n?תעציב םאה\n+10",
       ":המימוטנפב\n.וב תקחישש ןורחאה קחשמה םש תא "+PlayersList[(_whosTurnIsIt + 1) % (PlayersList.Count)].GetName()+"ל ראת\n\n?ןוכנ שחינ םאה\nםכינשל +10",
        ":המימוטנפב\n.ךתריחבל שגר לש ףוצרפ "+PlayersList[(_whosTurnIsIt + 1) % (PlayersList.Count)].GetName()+"ל השע\n\n?ןוכנ שחינ םאה\nםכינשל +10",
            " הציפק ידכ ךותו תחא לגר לע ץופק\n.התיבח םיניכמ דציכ רפס\n\n?תעציב םאה\n+10",
           ".(הל הל הלב םילימה תא ףלחה וא) בוהא ריש םזמז\n\n?ההיז והשימ\nךל +10",
           ".וישכע םהב תויהל הצור תייהש תיבל ץוחמ תומוקמ 3 רומא\n\n?תעציב םאה\n+10",
           ".ידכ ךות םייפכ אחמו לגר לכ לע םימעפ 8 ץופק\n\n?תעציב םאה\n+10",
          ".ךתוא וקחי םיפתתשמה לכו רצק דוקיר אצמה\n\n?תעציב םאה\n+10",
               ".הנורחאל ךתוא קיחצהש והשמ רפס\n\n?תעציב םאה\n+10",
    };

                QuestionsTexts = new string[]{
         "?ןופלטב הנורחאל תרביד ימ םע",//0
         ":עוגר רותרא\n\n הנתמ תודוקנ 5 לבק",//1  //BONUS
         "?" + PlayersList[(_whosTurnIsIt + 1) % PlayersList.Count].GetName() + "ב בהוא התא המ",//2
                          "וילע םיבוהאה םע םלטצהל בהוא רותרא\n.םיפי םיעגר רמשל",
         "?תיבב בהוא יכה התאש םוקמה המ",//4
         "ומצעל רפוס אוה ,ינבצע רותראשכ\n.עגרנו 30 דע",
         "?ךילע בוהאה עבצה המ",//6
               "הנורחאה החישה יהמ\n?פאסטאווב ךלש",
         "?ךמצעב בהוא התא המ",//8
             "אוה והשימל עגעגתמ רותראשכ\n.ואדיו תחישב וילא רשקתמ",
         "?קוביח לבקל בהוא יכה התא יממ",//10
             ".םירבחל ןגרפמ רותרא\n\n הנתמ תודוקנ 5 לבק",//1  //BONUS
         "?תושעל בהוא התא תיבב המישמ וזיא",//12
         "?הנורחאה םעפב תיכב יתמ",//13
         "?ךילע בוהאה קחשמה המ",//14
            "תא ףתשמ אוה ,בוצע רותראשכ\n.ול םירזוע םהו ולש החפשמה",
         "?בהוא יכה התאש לכאמה המ",//16
                "ןיזאמ אוה סעוכ וא בוצע רותראשכ\n ולש חורה בצמ דימו ,בוהא רישל\n.רפתשמ",
                ".ךתוא בהוא רותרא\n\n.ףסונ רות לבק",//1  //BONUS
                   " בהוא אוה ,סעוכ וא דחופ רותראשכ\n.ררחתשהלו ץופקל",
                 "!החמשמ דקור רותרא\n\n !ףסונ רות תלביק",//1  //BONUS
         "?הנורחאל קוחצל ךל םרג המ"//21
    };
                break;
            }


        }

}


