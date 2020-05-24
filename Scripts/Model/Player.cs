using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    private bool _playersTurn;
    private string _playersName;
    private int _playersScore;

    //Default card builder
    public Player(){
        _playersTurn = false;
        _playersName = "Player";
        _playersScore = 0;
    }

    //Setters
    public void SetPlayersTurn(bool value) {
        _playersTurn = value;
    }
    public void SetName(string name) {
        _playersName = name;
    }
    public void SetScore(int score) {
        _playersScore = score;
    }
    public void AddScore(int score) {
        _playersScore += score;
    }
    public void ReduceScore(int score) {
        _playersScore -= score;
    }

    //Getters
    public bool IsPlayersTurn() {
        return _playersTurn;
    }
    public string GetName() {
        return _playersName;
    }
    public int GetScore() {
        return _playersScore;
    }





}

