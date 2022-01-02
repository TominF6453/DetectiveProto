using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : ScriptableObject {
	protected PlayerController player;
	public abstract void FireSpell1 ();
	public abstract void FireSpell2 ();
	public abstract void SpellDash ();
	public abstract void FixedUpdate ();

	public void Init () {
		if ( player == null ) player = PlayerController.Player;
	}
}
