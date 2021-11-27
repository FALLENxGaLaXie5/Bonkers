using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Bonkers.Score
{
    public class PlayerScore : MonoBehaviour
    {
        [SerializeField] GameObject scoreTextObject;
        public int score = 0;

        public int playerNum = 1;
        TextMeshProUGUI textField;

        void Start()
        {
            //textField = scoreTextObject.GetComponent<TextMeshProUGUI>();

        }

        void Update()
        {
            if (!textField) return;
            textField.text = score.ToString();
        }

        public void AddToScore(int num)
        {
            score += num;
        }

        public void SetupScoreUI(GameObject scoreObject)
        {
            scoreTextObject = scoreObject;
            textField = scoreTextObject.GetComponent<TextMeshProUGUI>();
        }
    }
}