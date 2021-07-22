using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : AnimationManager
{

    private void Start()
    {
        if (Anim == null)
        {
            Anim = GetComponent<Animator>();
        }
    }

    public override void Move(float motion)
    {
        // throw new System.NotImplementedException();

    }

    public void SetBlend(float x, float z)
    {
        // Sets the X component of the blendtree
        Anim.SetFloat("MoveX", x);

        // Sets the Y component of the blendtree
        Anim.SetFloat("MoveY", z);
    }

    public override void Attack()
    {
        // throw new System.NotImplementedException();
    }

    public override void Death()
    {
        // throw new System.NotImplementedException();

        // Play the death anim
    }

    //Player the
    public void SwitchWeapon()
    {

    }

    public void SetIK()
    {

    }
}
