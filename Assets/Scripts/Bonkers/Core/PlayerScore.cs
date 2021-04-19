using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Control;
using TMPro;

namespace Bonkers.Core
{
    public class PlayerScore : MonoBehaviour
    {
        [SerializeField] GameObject scoreTextObject;
        public int score = 0;

        int playerNum = 1;
        TextMeshProUGUI textField;

        void Start()
        {
            playerNum = GetComponent<PlayerControl>().GetPlayerNum();
            textField = scoreTextObject.GetComponent<TextMeshProUGUI>();

        }

        void Update()
        {
            textField.text = score.ToString();  
        }

        public void AddToScore(int num)
        {
            this.score += num;
        }
    }
}