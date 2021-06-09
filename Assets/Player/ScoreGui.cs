using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ScoreGui : MonoBehaviour
{
    private Player _player;
    private TMP_Text textMesh;
    // Start is called before the first frame update
    void Start()
    {
        if (Creatures.AllCreatures.ContainsKey(transform.parent.parent.parent))
        {
            _player = (Player)Creatures.AllCreatures[transform.parent.parent.parent];
        }
        textMesh = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_player == null)
        {
            if (Creatures.AllCreatures.ContainsKey(transform.parent.parent.parent))
            {
                _player = (Player)Creatures.AllCreatures[transform.parent.parent.parent];
            }
        }
        else
        {
            textMesh.text = "§" + _player.SaveData.CurrentScore.ToString();
        }
    }
}
