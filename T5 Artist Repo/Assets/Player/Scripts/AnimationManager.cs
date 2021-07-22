using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationManager : MonoBehaviour
{
    public Animator Anim;

    public abstract void Attack();

    public abstract void Move(float motion);

    public abstract void Death();

    public void PlayAnimation(Animation animation, int layer = 0)
    {
        Anim.Play(animation.ToString(), layer);
    }

    public void SetAnimBool(string boolName, bool setBool)
    {
        Anim.SetBool(boolName, setBool);
    }

    public void SetAnimTrigger(string boolName)
    {
        Anim.SetTrigger(boolName);
    }

    public void SetAnimFloat(string boolName, float num)
    {
        Anim.SetFloat(boolName, num);
    }

}
