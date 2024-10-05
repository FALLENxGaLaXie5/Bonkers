using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.ItemDrops
{
    [CreateAssetMenu(fileName = "Powerup", menuName = "Drops/Make New Powerup", order = 2)]
    public class Powerup : ScriptableObject, IDroppableObject
    {
        #region Data Fields

        [Title("Powerup Data")]


        [SerializeField] [Required] [EnumToggleButtons, HideLabel] PowerupType type;

        [SerializeField][Required][PreviewField(60), HideLabel][HorizontalGroup("Split", 60)] GameObject powerupPrefab = null;
        
        [SerializeField] [VerticalGroup("Split/Right"), LabelWidth(120)] float life = 5f;
        [SerializeField][Range(0.01f, 0.1f)][VerticalGroup("Split/Right"), LabelWidth(120)] float modifier = 0.03f;
        [SerializeField][Required][ColorUsage(true, true)][VerticalGroup("Split/Right"), LabelWidth(120)] Color powerupColor;

        #endregion

        public enum PowerupType { Shield, Stamina};

        #region Class Functions

        public void Spawn(Vector3 position)
        {
            Transform powerupDropsTransform = GameObject.FindWithTag("Drops").transform;
            if (!powerupPrefab) return;
            if (powerupDropsTransform)
            {
                Instantiate(powerupPrefab, position, Quaternion.identity).transform.parent = powerupDropsTransform;
            }                
            else
            {
                Instantiate(powerupPrefab, position, Quaternion.identity);
                Debug.LogWarning("Why no object to put the drop in???");
            }
        }

        #endregion

        #region Data Accessors

        public float Life => life;

        public PowerupType Type => type;

        public Color Color => powerupColor;

        public float Modifier => modifier;

        #endregion
    }
}