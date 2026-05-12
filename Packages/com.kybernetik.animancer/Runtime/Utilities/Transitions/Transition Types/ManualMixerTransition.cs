// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2026 Kybernetik //

using System;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ManualMixerTransition
    [Serializable]
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public class ManualMixerTransition : ManualMixerTransition<ManualMixerState>,
        ICopyable<ManualMixerTransition>
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override ManualMixerState CreateState()
        {
            State = new();
            InitializeState();
            return State;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Transition<ManualMixerState> Clone(CloneContext context)
        {
            var clone = new ManualMixerTransition();
            clone.CopyFrom(this, context);
            return clone;
        }

        /// <inheritdoc/>
        public sealed override void CopyFrom(ManualMixerTransition<ManualMixerState> copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(ManualMixerTransition copyFrom, CloneContext context)
            => base.CopyFrom(copyFrom, context);

        /************************************************************************************************************************/
    }
}

