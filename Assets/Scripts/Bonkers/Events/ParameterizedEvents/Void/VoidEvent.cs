using UnityEngine;

namespace Bonkers.Events
{
    [CreateAssetMenu(fileName = "New Void Event", menuName = "Game Events/Void Event", order = 0)]
    public class VoidEvent : BaseGameEvent<Void>
    {
        public void Raise() => Raise(new Void());
    }
}