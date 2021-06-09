
using UnityEngine;

public class GameData
{
    public int _score;
    public string _name;
    public float _timePlayed;

    public GameData(int score, string name, float timePlayed)
    {
        _score = score;
        _name = name;
        _timePlayed = timePlayed;
    }

    public GameData() { Debug.Log("inside empty constructor"); }

}
