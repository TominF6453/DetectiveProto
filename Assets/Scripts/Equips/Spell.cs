using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spell {
	public void FireSpell1 ( PlayerController pc );
	public void FireSpell2 ( PlayerController pc );
	public void SpellDash ( PlayerController pc );
}
