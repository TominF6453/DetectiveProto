using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool fire = Input.GetButton("Fire1");
        bool thumbs = Input.GetKey(KeyCode.Space);
        m_animator.SetBool("Thumbs", thumbs);
        m_animator.SetBool("Fire", fire);

    }
}
