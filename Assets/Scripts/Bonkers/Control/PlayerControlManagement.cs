using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Control
{
    [CreateAssetMenu(fileName = "Player Control Management System", menuName = "Player Control/Player Control Management System", order = 0)]
    public class PlayerControlManagement : ScriptableObject
    {
        private List<PlayerControl> players = new List<PlayerControl>();

        public void AddPlayer(PlayerControl playerControl) => players.Add(playerControl);
        public void RemovePlayer(PlayerControl playerControl) => players.Remove(playerControl);
        public int NumberPlayersRemaining => players.Count;
        public void ClearPlayers() => players.Clear();
    }
}