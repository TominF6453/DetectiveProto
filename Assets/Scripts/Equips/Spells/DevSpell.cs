using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/DevSpell")]
public class DevSpell : Spell
{
	Vector3 dashVector = new Vector3();
	float dashTimer = 0f;

	public override void FireSpell1 () {
		throw new System.NotImplementedException();
	}

	public override void FireSpell2 () {
		throw new System.NotImplementedException();
	}

	public override void SpellDash () {
		if ( player.isDashing ) return; // Already dashing, return.

		dashVector = Vector3.zero;
        if ( InputHandler.InputForward ) dashVector += player.Cam.transform.forward;
        else if ( InputHandler.InputBackward ) dashVector += -player.Cam.transform.forward;

        if ( InputHandler.InputLeft ) dashVector += -player.Cam.transform.right;
        else if ( InputHandler.InputRight ) dashVector += player.Cam.transform.right;

        // Try to remove any vertical component from the direction.
        dashVector.Normalize();
        dashVector -= player.transform.up * Vector3.Dot( dashVector , player.transform.up );

        // Normalize again, multiply by move speed.
        dashVector.Normalize();
        dashVector *= player.moveSpeedMulti * 0.5f * player.dashSpeedMulti;
		dashTimer = player.dashLength;
		player.isDashing = true;
    }

	public override void FixedUpdate () {
		if ( !player.isDashing ) return;

		if ( dashTimer > 0) {
			player.transform.position += Utilities.Inst.AdjustVectorByRaycast( player.transform.position , dashVector , -0.5f );
			dashTimer -= Time.fixedDeltaTime * player.dashSpeedMulti;
		} else { // dashTimer <= 0
			player.SetMovementVector( Utilities.Inst.AdjustVectorByRaycast( player.transform.position , dashVector , -0.5f ) );
			player.isDashing = false;
		}
	}
}
