using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevSpell : MonoBehaviour, Spell
{
	public void FireSpell1 ( PlayerController pc ) {
		throw new System.NotImplementedException();
	}

	public void FireSpell2 ( PlayerController pc ) {
		throw new System.NotImplementedException();
	}

	public void SpellDash ( PlayerController pc ) {
		Vector3 dashVector = new Vector3();
        if ( InputHandler.InputForward ) dashVector += pc.Cam.transform.forward;
        else if ( InputHandler.InputBackward ) dashVector += -pc.Cam.transform.forward;

        if ( InputHandler.InputLeft ) dashVector += -pc.Cam.transform.right;
        else if ( InputHandler.InputRight ) dashVector += pc.Cam.transform.right;

        // Try to remove any vertical component from the direction.
        dashVector.Normalize();
        dashVector -= transform.up * Vector3.Dot( dashVector , transform.up );

        // Normalize again, multiply by move speed.
        dashVector.Normalize();
        dashVector *= pc.moveSpeedMulti * 0.5f * pc.dashSpeedMulti;
    }

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
