using UnityEngine;

namespace Bonkers.Events
{
    [CreateAssetMenu(fileName = "New Int Event", menuName = "Game Events/Int Event", order = 0)]
    public class IntEvent : BaseGameEvent<int> {}
}