using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine;



public class DataManagement : MonoBehaviour {

    // Keys and values for each data type
    private Dictionary<string, string> _StringsDictionary; 
    private Dictionary<string, bool> _BoolsDictionary;
    private Dictionary<string, int> _IntsDictionary;
    private Dictionary<string, float> _FloatsDictionary;

    private StorageOption _storageOption;

    public enum StorageOption {
        Persisted,
        NotPersisted,
    }

    //Default DataManagement
    public DataManagement() {
        _StringsDictionary = new Dictionary<string, string>();
        _BoolsDictionary = new Dictionary<string, bool>();
        _IntsDictionary = new Dictionary<string, int>();
        _FloatsDictionary = new Dictionary<string, float>();

        _storageOption = StorageOption.Persisted;
    }

    public void SetStorageOption(StorageOption storageOption) {
        _storageOption = storageOption;
    }


    /// <summary>
    /// Saving the object with the provided key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"> Key to save by </param> 
    /// <param name="obj"> Object to save. Can be string/bool/int/float </param>
    public void SaveObject<T>(string key, T obj){
        if (_storageOption == StorageOption.NotPersisted) {
            if (obj.GetType() == typeof(string)) { /// I need to make sure it dosn't say that everything is a string
                if (_StringsDictionary.ContainsKey("string: " + key)){ // if the key (value) is already existed, than overwrite
                    _StringsDictionary["string: " + key] = (string)(object)obj;
                }
                else { // If key is not in dictionary, add a new one
                    _StringsDictionary.Add("string: " + key, (string)(object)obj);
                }
            }
            else if (obj.GetType() == typeof(bool)) {
                if (_BoolsDictionary.ContainsKey("bool: " + key)) { // if the key (value) is already existed, than overwrite
                    _BoolsDictionary["bool: " + key] = (bool)(object)obj;
                }
                else { // If key is not in dictionary, add a new one
                    _BoolsDictionary.Add("bool: " + key, (bool)(object)obj);
                }
            }
            else if (obj.GetType() == typeof(int)) {
                if (_IntsDictionary.ContainsKey("int: " + key)) { // if the key (value) is already existed, than overwrite
                    _IntsDictionary["int: " + key] = (int)(object)obj;
                }
                else { // If key is not in dictionary, add a new one
                    _IntsDictionary.Add("int: " + key, (int)(object)obj);
                }
            }
            else if (obj.GetType() == typeof(float)) {
                if (_FloatsDictionary.ContainsKey("float: " + key)) { // if the key (value) is already existed, than overwrite
                    _FloatsDictionary["float: " + key] = (float)(object)obj;
                }
                else { // If key is not in dictionary, add a new one
                    _FloatsDictionary.Add("float: " + key, (float)(object)obj);
                }
            }
            else {
                throw new System.Exception("Type unkown");
            }
        }

        //If option is Persisted use PlayerPrefs to save key and object
        else if (_storageOption == StorageOption.Persisted) {
            if (obj.GetType() == typeof(string)) { /// I need to make sure it dosn't say that everything is a string
                PlayerPrefs.SetString("string: " + key, (string)(object)obj);
            }
            else if (obj.GetType() == typeof(bool)) {//Saves as int
                int tempAsBool = 0; // false
                if ((bool)(object)obj) {
                    tempAsBool = 1; //true
                }
                PlayerPrefs.SetInt("bool: " + key, tempAsBool);

            }
            else if (obj.GetType() == typeof(int)) {
                PlayerPrefs.SetInt("int: " + key, (int)(object)obj);
            }
            else if (obj.GetType() == typeof(float)) {
                PlayerPrefs.SetFloat("float: " + key, (float)(object)obj);
            }
        }

        else {
            throw new System.Exception("Storage Optioin Unkown");
        }
    }

    //Loading string by key
    public string LoadString(string key) {
        if (_storageOption == StorageOption.NotPersisted) {
            if (_StringsDictionary.ContainsKey("string: " + key)) {
                return _StringsDictionary["string: " + key];
            }
        }
        else if (_storageOption == StorageOption.Persisted) {
            if (PlayerPrefs.HasKey("string: " + key)) {
                return PlayerPrefs.GetString("string: " + key);
            }
        }
        throw new System.Exception("String by Key not found. Cannot Load");
    }
    //Loading bool by key
    public bool LoadBool(string key) {
        if (_storageOption == StorageOption.NotPersisted) {
            if (_BoolsDictionary.ContainsKey("bool: " + key)) {
                return _BoolsDictionary["bool: " + key];
            }
        }
        else if (_storageOption == StorageOption.Persisted) {
            if (PlayerPrefs.HasKey("bool: " + key)) {
                if (PlayerPrefs.GetInt("bool: " + key) == 0) {
                    return false;
                }
                else { return true; }
            }
        }
        throw new System.Exception("Bool by Key not found. Cannot Load");
    }

    //Loading int by key
    public int LoadInt(string key) {
        if (_storageOption == StorageOption.NotPersisted) {
            if (_IntsDictionary.ContainsKey("int: " + key)) {
                return _IntsDictionary["int: " + key];
            }
        }
        else if (_storageOption == StorageOption.Persisted) {
            if (PlayerPrefs.HasKey("int: " + key)) {
                return PlayerPrefs.GetInt("int: " + key);
            }
        }
        throw new System.Exception("Int by Key not found. Cannot Load");
    }
    //Loading float by key
    public float LoadFloat(string key) {
        if (_storageOption == StorageOption.NotPersisted) {
            if (_FloatsDictionary.ContainsKey("float: " + key)) {
                return _FloatsDictionary["float: " + key];
            }
        }
        else if (_storageOption == StorageOption.Persisted) {
            if (PlayerPrefs.HasKey("float: " + key)) {
                return PlayerPrefs.GetFloat("float: " + key);
            }
        }
        throw new System.Exception("Float by Key not found. Cannot Load");
    }


    /// <summary>
    /// Deleting the object with the provided key.
    ///
    /// JUST TO MENTION: I don't use RemoveObject in part 2 (i.e in the memory game)..
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"> Key to delete by </param> 
    /// <param name="obj"> Use it just to know the type (can be anything as long it is the wanted object type </param>
    public void DeleteObject<T>(string key, T obj) {
        if (_storageOption == StorageOption.NotPersisted) {
            if (obj.GetType() == typeof(string)) { /// I need to make sure it dosn't say that everything is a string
                if (_StringsDictionary.ContainsKey("string: " + key)) { // if the key is existing, than remove it
                    _StringsDictionary.Remove("string: " + key);
                }
            }
            else if (obj.GetType() == typeof(bool)) {
                if (_BoolsDictionary.ContainsKey("bool: " + key)) { // if the key is existing, than remove it
                    _BoolsDictionary.Remove("bool: " + key);
                }
            }
            else if (obj.GetType() == typeof(int)) {
                if (_IntsDictionary.ContainsKey("int: " + key)) { // if the key is existing, than remove it
                    _IntsDictionary.Remove("int: " + key);
                }
            }
            else if (obj.GetType() == typeof(float)) {
                if (_FloatsDictionary.ContainsKey("float: " + key)) { // if the key is existing, than remove it
                    _FloatsDictionary.Remove("float: " + key);
                }
            }
            else {
                throw new System.Exception("Type unkown");
            }
        }

        //If option is Persisted use PlayerPrefs to remove key and object
        else if (_storageOption == StorageOption.Persisted) {
            if (obj.GetType() == typeof(string)) {
                PlayerPrefs.DeleteKey("string: " + key);
            }
            else if (obj.GetType() == typeof(bool)) {
                PlayerPrefs.DeleteKey("bool: " + key);
            }
            else if (obj.GetType() == typeof(int)) {
                PlayerPrefs.DeleteKey("int: " + key);
            }
            else if (obj.GetType() == typeof(float)) {
                PlayerPrefs.DeleteKey("float: " + key);
            }
        }
    }

}
