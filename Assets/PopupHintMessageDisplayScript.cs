using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHintMessageDisplayScript : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyPopup()
    {
        animator.Play("pnlPopupHintMessageHide");
        Destroy(gameObject, 0.5f);
    }

}
