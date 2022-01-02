using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Values
    public Spell EquippedSpell;
    public Weapon EquippedWeapon;

    public float moveSpeedMulti = 1.0f;
    public float jumpHeightMulti = 1.0f;
    public float gravityMulti = 1.0f;
    public float airControlMulti = 0.2f;
    public float dashLength = 1f;
    public float dashSpeedMulti = 4f;

    // Camera
    public Camera Cam { private set; get; }

    // Internal values.
    [HideInInspector] public bool shouldDash = false;
    [HideInInspector] public float dashTimer = 0f;

    [SerializeField] Transform gravitationalCenter;

    private float FloorDistance { get {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 20000f, 1 << 6)) 
                return hit.distance;
			else return Mathf.Infinity;
		} }

    private bool IsGrounded { get { return FloorDistance <= 1.2f; } }
    private float GroundedMovementMulti { get { return IsGrounded ? 1.0f : airControlMulti; } }

    // Start is called before the first frame update
    void Start()
    {
        Cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        UpdateCamera();
        UpdateInputs();
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
    Vector3 dashVector = new Vector3();
    void UpdateMovement() {
        // Check if dashing.
        if (dashTimer > 0) {
            // Maintain movement vector.
            transform.position += AdjustVectorByRaycast( dashVector , -0.5f );
            dashTimer -= Time.fixedDeltaTime * dashSpeedMulti;
            if ( dashTimer <= 0 ) movementVector = AdjustVectorByRaycast( dashVector , -0.5f ); ;
            shouldDash = false;
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

        // Check if starting to dash.
        if (dashTimer <= 0f && shouldDash) {
            dashTimer = dashLength;
            dashVector = newTarget * dashSpeedMulti;
            shouldDash = false;
            return;
		}

        // -- Vectical Movement --
        if ( IsGrounded && InputHandler.InputJump ) movementVector += transform.up * jumpHeightMulti * 0.5f ;

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

        transform.position += AdjustVectorByRaycast(movementVector);
    }

    void UpdateInputs () {
        if ( InputHandler.InputDash ) shouldDash = true;
	}

    void UpdateGravity() {
        Vector3 newUp = transform.position - gravitationalCenter.position;
        transform.rotation = Quaternion.LookRotation( Vector3.Cross( transform.right , newUp ) , newUp );
	}

    Vector3 AdjustVectorByRaycast(Vector3 inV, float yOffset = 0f) {
        // Use SphereCastAll to hit multiple walls and adjust the vector appropriately.
        Vector3 orig = transform.TransformPoint(new Vector3(0,yOffset,0));
        RaycastHit[] hits = Physics.SphereCastAll(orig - inV, 0.5f, inV.normalized, inV.magnitude * 2, 1 << 6 );
        if ( hits.Length == 0 ) return inV;

        foreach ( RaycastHit hit in hits ) {
            if ( hit.distance != 0 ) {
                float dotDiff = Vector3.Dot(inV, hit.normal);
                inV += hit.normal * -dotDiff;
            }
		}
        return inV;
	}
}