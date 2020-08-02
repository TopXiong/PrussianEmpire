using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorChange : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Death"))
        {
            var playerBehavior = animator.GetComponentInParent<PlayerBehavior>();
            Destroy(animator.transform.parent.gameObject);
            Debug.Log("PlayerAnimatorChange:" + "Death");
        }
    }

}
