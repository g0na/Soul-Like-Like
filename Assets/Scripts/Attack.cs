using UnityEngine;

public class Attack : MonoBehaviour
{
    Animator anim;
    int hashAttackCount = Animator.StringToHash("AttackCount");
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public int AttackCount
    {
        get => anim.GetInteger(hashAttackCount);
        set => anim.SetInteger(hashAttackCount, value);
    }
}
