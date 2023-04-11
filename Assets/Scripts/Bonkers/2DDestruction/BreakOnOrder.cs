using RoboRyanTron.Unite2017.Variables;
using UnityEngine;

[RequireComponent(typeof(Breakable))]
public class BreakOnOrder : MonoBehaviour
{
    [SerializeField] FloatReference fadeTime = new (5f);
    private Breakable breakableComponent;
    private BreakForce breakForce;

    #region Properties
    public float FadeTime => fadeTime;
    #endregion
    
    void Awake()
    {
        breakableComponent = GetComponent<Breakable>();
        breakForce = FindObjectOfType<BreakForce>();
    }

    public void BreakBlok()
    {
        Vector3 preExplosionPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        breakableComponent.ActivateFragments();
        breakForce.DoBreaking(preExplosionPosition);
        breakableComponent.FadeAndDestroyFragments(fadeTime);
    }
}