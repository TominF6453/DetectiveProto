using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController Player { get; private set; }

    // Player Values
    public Spell EquippedSpell;
    public Weapon EquippedWeapon;

    public float moveSpeedMulti = 1.0f;
    public float jumpHeightMulti = 1.0f;
    public float gravityMulti = 1.0f;
    public float airControlMulti = 0.2f;
    public float dashLength = 1f;
    public float dashSpeedMulti = 4f;
    public Transform respawnPoint;
    public float coyoteTimeDuration = 0.1f;

    // Camera
    public Camera Cam { private set; get; }

    // Internal values.
    [HideInInspector] public bool shouldDash = false;
    [HideInInspector] public bool isDashing = false;

    [SerializeField] Transform gravitationalCenter;

    private float FloorDistance { get {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 20000f, 1 << 6)) 
                return hit.distance;
			else return Mathf.Infinity;
		} }

    /// when IsGrounded is false start CoyoteTimer counting down from coyoteTimeDuration to 0
    /// if CoyoteTimer is > 0 CanJump is true
    private bool IsGrounded { get { return FloorDistance <= 1.2f; } }
    private float CoyoteTimer;
    private bool CanJump;
    private float GroundedMovementMulti { get { return IsGrounded ? 1.0f : airControlMulti; } }

    // Start is called before the first frame update
    void Start()
    {
        Cam = Camera.main;
        Player = this;
        EquippedSpell?.Init();
        EquippedWeapon?.Init();
    }

    // Update is called once per frame
    void Update() {
        UpdateCamera();
        UpdateInputs();

        if (transform.position.y < -10) {
            transform.position = respawnPoint == null ? new Vector3(0, 10, 0) : respawnPoint.position;
		}
    }

	private void FixedUpdate () {
        UpdateMovement();
	}

    private float curX = 0f;
    private float curY = 0f;
    void UpdateCamera() {
        if ( Cursor.lockState != CursorLockMode.Locked ) return;
        // Get raw mouse delta, multiply by sensitivity and inversion.
        Vector2 mouse = InputHandler.InputMouseLook * InputHandler.Instance.MouseSensitivity;
        if ( InputHandler.Instance.InvertMouseX ) mouse.x *= -1;
        if ( InputHandler.Instance.InvertMouseY ) mouse.y *= -1;

        curX += mouse.x;
        curY += mouse.y;
        curY = Mathf.Clamp( curY , -75f , 75f );

        Cam.transform.localRotation = Quaternion.Euler( curY , curX , 0 );
	}

    Vector3 movementVector = new Vector3();
    void UpdateMovement() {
        if ( IsGrounded ){
            CanJump = true;
            CoyoteTimer = coyoteTimeDuration;
        }
        else  CoyoteTimer -= 1 * Time.deltaTime;

        CanJump = CoyoteTimer > 0;
        print(CanJump);

        // Check if dashing.
        // Was coded here, but needs to be moved to spell FixedUpdate();
        if ( isDashing ) {
            EquippedSpell.FixedUpdate();
            return;
        }
        
        // -- Horizontal Movement --
        Vector3 newTarget = new Vector3();
        if ( InputHandler.InputForward ) newTarget += Cam.transform.forward;
        else if ( InputHandler.InputBackward ) newTarget += -Cam.transform.forward;

        if ( InputHandler.InputLeft ) newTarget += -Cam.transform.right;
        else if ( InputHandler.InputRight ) newTarget += Cam.transform.right;

        // Try to remove any vertical component from the direction.
        newTarget.Normalize();
        newTarget -= transform.up * Vector3.Dot( newTarget , transform.up );

        // Normalize again, multiply by move speed.
        newTarget.Normalize();
        newTarget *= moveSpeedMulti * 0.5f;

        // -- Vectical Movement --
        if (CanJump && InputHandler.InputJump){
            CoyoteTimer = 0;
            movementVector += transform.up * jumpHeightMulti * 0.5f;
        }

        // Get clamped dot value.
        float clampDot = Mathf.Clamp(Vector3.Dot(movementVector, transform.up) - (gravityMulti * .05f), -FloorDistance + 1, 100f);
        // Set newTarget value by adding transform down multiplied by clamp dot.
        Vector3 downVector = transform.up * clampDot;

        if ( gravitationalCenter != null ) UpdateGravity();

        // Remove vertical component of movement vector, lerp horizontal component.
        movementVector -= transform.up * Vector3.Dot( movementVector , transform.up );
        movementVector = Vector3.Lerp( movementVector , newTarget , 0.2f * GroundedMovementMulti );
        // Add back in the new vertical component.
        movementVector += downVector;

        transform.position += Utilities.Inst.AdjustVectorByRaycast(transform.position, movementVector);
    }

    void UpdateInputs () {
        if ( InputHandler.InputDash ) EquippedSpell.SpellDash();
	}

    void UpdateGravity() {
        Vector3 newUp = transform.position - gravitationalCenter.position;
        transform.rotation = Quaternion.LookRotation( Vector3.Cross( transform.right , newUp ) , newUp );
	}

    /// <summary>
    /// Used for an outside script to adjust the player movement.
    /// </summary>
    /// <param name="newVec">The new vector to use for the movement vector.</param>
    /// <param name="additive">Optional. If false, sets the internal movement vector to newVec. Otherwise, adds the newVec to the current movement vector.</param>
    public void SetMovementVector(Vector3 newVec, bool additive = false) {
        if ( additive ) movementVector += newVec;
        else movementVector = newVec;
	}
}
