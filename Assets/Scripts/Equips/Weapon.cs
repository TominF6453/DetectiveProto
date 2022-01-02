using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject {
	protected PlayerController player;
	public abstract void PrimaryFire ();
	public abstract void AltFire ();
	public abstract void Reload ();

	public void Init () {
		if ( player == null ) player = PlayerController.Player;
	}
}
