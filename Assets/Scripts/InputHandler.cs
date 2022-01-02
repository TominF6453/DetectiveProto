using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Instancing
    public static InputHandler Instance { get; set; }

    // Options
    public float MouseSensitivity = 1.0f;
    public bool InvertMouseX = false;
    public bool InvertMouseY = false;

    // --- Input Vals ---
    // Hardcoded for now lol
    public static bool InputForward { get { return Input.GetKey( KeyCode.W ); } }
    public static bool InputBackward { get { return Input.GetKey( KeyCode.S ); } }
    public static bool InputLeft { get { return Input.GetKey( KeyCode.A ); } }
    public static bool InputRight { get { return Input.GetKey( KeyCode.D ); } }
    public static bool InputJump { get { return Input.GetKey( KeyCode.Space ); } }
    public static bool InputDash { get { return Input.GetKeyDown( KeyCode.LeftShift ); } }
    public static bool InputFire { get { return Input.GetButtonDown( "Fire1" ); } }
    public static bool InputAltFire { get { return Input.GetButtonDown( "Fire2" ); } }
    public static bool InputSpellOne { get { return Input.GetKeyDown( KeyCode.Q ); } }
    public static bool InputSpellTwo { get { return Input.GetKeyDown( KeyCode.E ); } }
    public static bool InputReload { get { return Input.GetKeyDown( KeyCode.R ); } }
    public static Vector2 InputMouseLook { get { return new Vector2(Time.deltaTime * Input.GetAxis( "Mouse X" ), Time.deltaTime * -Input.GetAxis("Mouse Y") ) * 1000 ; } }

    public void Start () {
        if ( Instance == null ) Instance = this;
        else Destroy( this );

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
}
