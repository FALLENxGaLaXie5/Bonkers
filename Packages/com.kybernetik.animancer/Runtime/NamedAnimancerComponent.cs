// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2026 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <summary>
    /// An <see cref="AnimancerComponent"/> which uses the <see cref="Object.name"/>s of <see cref="AnimationClip"/>s
    /// so they can be referenced using strings as well as the clips themselves.
    /// </summary>
    /// 
    /// <remarks>
    /// It also has fields to automatically register animations on startup and play the first one automatically without
    /// needing another script to control it, much like Unity's Legacy <see cref="Animation"/> component.
    /// <para></para>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/component-types">
    /// Component Types</see>
    /// <para></para>
    /// <strong>Sample:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/samples/fine-control/named">
    /// Named Character</see>
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/NamedAnimancerComponent
    /// 
    [AddComponentMenu(Strings.MenuPrefix + "Named Animancer Component")]
    [AnimancerHelpUrl(typeof(NamedAnimancerComponent))]
    public class NamedAnimancerComponent : AnimancerComponent
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        /// <summary>[Internal] Field names for the custom Inspector.</summary>
        public const string
            PlayAutomaticallyField = nameof(_PlayAutomatically),
            NamesField = nameof(_Names),
            AnimationsField = nameof(_Animations);

        /************************************************************************************************************************/

        [SerializeField, Tooltip("If true, the 'Default Animation' will be automatically played by " + nameof(OnEnable))]
        private bool _PlayAutomatically = true;

        /// <summary>[<see cref="SerializeField"/>]
        /// If true, the first clip in the <see cref="Animations"/> array will be automatically played by
        /// <see cref="OnEnable"/>.
        /// </summary>
        public ref bool PlayAutomatically => ref _PlayAutomatically;

        /************************************************************************************************************************/

        [SerializeField, Tooltip(
            "Optional names for the Animations." +
            " If not set, they will use their Animation Clip names.")]
        private StringAsset[] _Names;

        /// <summary>[<see cref="SerializeField"/>]
        /// Optional names for the <see cref="Animations"/>.
        /// If not set, they will use their <see cref="Object.name"/>.
        /// </summary>
        public StringAsset[] Names
        {
            get => _Names;
            set
            {
                _Names = value;

                Debug.Assert(
                    !IsGraphInitialized,
                    $"{nameof(NamedAnimancerComponent)}.{nameof(Names)}" +
                    $" doesn't support being changed after it has already initialized." +
                    $"\nIf any names aren't specified, they will use their Animation Clip name.",
                    this);

                // This could potentially be supported by trying to look up the states based on their old names
                // and changing their keys, but that doesn't seem like a common use case
                // so it's probably not worth the effort.
            }
        }

        /************************************************************************************************************************/

        [SerializeField, Tooltip("Animations in this array will be automatically registered by " + nameof(Awake) +
            " as states that can be retrieved using their name")]
        private AnimationClip[] _Animations;

        /// <summary>[<see cref="SerializeField"/>]
        /// Animations in this array will be automatically registered by <see cref="Awake"/> as states that can be
        /// retrieved using their name and the first element will be played by <see cref="OnEnable"/> if
        /// <see cref="PlayAutomatically"/> is true.
        /// </summary>
        public AnimationClip[] Animations
        {
            get => _Animations;
            set
            {
                _Animations = value;
                States.CreateIfNew(value);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The first element in the <see cref="Animations"/> array. It will be automatically played by
        /// <see cref="OnEnable"/> if <see cref="PlayAutomatically"/> is true.
        /// </summary>
        public AnimationClip DefaultAnimation
        {
            get => _Animations.IsNullOrEmpty() ? null : _Animations[0];
            set
            {
                if (_Animations.IsNullOrEmpty())
                    _Animations = new AnimationClip[] { value };
                else
                    _Animations[0] = value;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Methods
        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only]
        /// Uses <see cref="ClipState.ValidateClip"/> to ensure that all of the clips in the <see cref="Animations"/>
        /// array are supported by the <see cref="Animancer"/> system and removes any others.
        /// </summary>
        /// <remarks>Called in Edit Mode whenever this script is loaded or a value is changed in the Inspector.</remarks>
        protected virtual void OnValidate()
        {
            if (_Animations == null)
                return;

            for (int i = 0; i < _Animations.Length; i++)
            {
                var clip = _Animations[i];
                if (clip == null)
                    continue;

                try
                {
                    Validate.AssertAnimationClip(clip, true, $"add animation to {nameof(NamedAnimancerComponent)}");
                    continue;
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, clip);
                }

                Array.Copy(_Animations, i + 1, _Animations, i, _Animations.Length - (i + 1));
                Array.Resize(ref _Animations, _Animations.Length - 1);
                i--;
            }
        }
#endif

        /************************************************************************************************************************/

        /// <summary>Creates a state for each clip in the <see cref="Animations"/> array.</summary>
        protected virtual void Awake()
        {
            if (!TryGetAnimator())
                return;

            if (_Names == null || _Names.Length == 0)
            {
                States.CreateIfNew(_Animations);
            }
            else
            {
                var nameCount = _Names.Length;
                var clipCount = _Animations.Length;
                for (int i = 0; i < clipCount; i++)
                {
                    var clip = _Animations[i];
                    if (clip != null)
                    {
                        var key = i < nameCount ? (object)(StringReference)_Names[i] : null;
                        key ??= GetKey(clip);
                        States.GetOrCreate(key, clip);
                    }
                }
            }

        }

        /************************************************************************************************************************/

        /// <summary>
        /// Plays the first clip in the <see cref="Animations"/> array if <see cref="PlayAutomatically"/> is true.
        /// </summary>
        /// <remarks>This method also ensures that the <see cref="PlayableGraph"/> is playing.</remarks>
        protected override void OnEnable()
        {
            if (!TryGetAnimator())
                return;

            base.OnEnable();

            if (_PlayAutomatically && !_Animations.IsNullOrEmpty())
            {
                var clip = _Animations[0];
                if (clip != null)
                    Play(clip);
            }
        }

        /************************************************************************************************************************/

        /// <summary>Returns the clip's name.</summary>
        /// <remarks>
        /// This method is used to determine the dictionary key to use for an animation when none is specified by the
        /// caller, such as in <see cref="AnimancerComponent.Play(AnimationClip)"/>.
        /// </remarks>
        public override object GetKey(AnimationClip clip) => clip.name;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void GatherAnimationClips(ICollection<AnimationClip> clips)
        {
            base.GatherAnimationClips(clips);
            clips.Gather(_Animations);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

