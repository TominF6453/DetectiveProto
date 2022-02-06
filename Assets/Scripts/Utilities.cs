using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static Utilities Inst { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if ( Inst == null ) Inst = this;
        else Destroy( gameObject );
    }

    /// <summary>
    /// Adjusts a vector by sphere casting out to detect any objects, the vector is returned modified such that no such collisions would occur.
    /// </summary>
    /// <param name="orig">The originating position of the vector in world space.</param>
    /// <param name="inV">The originating direction vector.</param>
    /// <param name="yOffset">Optional. A flat offset to apply to the y of orig.</param>
    /// <param name="radius">Optional. The radius of the sphere cast.</param>
    /// <returns></returns>
    public Vector3 AdjustVectorByRaycast ( Vector3 orig, Vector3 inV , float yOffset = 0f, float radius = 0.5f ) {
        // Use SphereCastAll to hit multiple walls and adjust the vector appropriately.
        orig += new Vector3( 0 , yOffset , 0 );
        RaycastHit[] hits = Physics.SphereCastAll(orig - inV, radius, inV.normalized, inV.magnitude * 2, 1 << 6 );
        if ( hits.Length == 0 ) return inV;

        float dotDiff;
        int n = 0;
        // Infinitely loop through this until no more objects get hit.
        while ( hits.Length != 0 && n < 5 ) {
            foreach ( RaycastHit hit in hits ) {
                if ( hit.distance != 0 ) {
                    dotDiff = Vector3.Dot(inV, hit.normal);
                    inV += hit.normal * -dotDiff;
                }
            }

            // Refresh hits with adjusted inV vector.
            hits = Physics.SphereCastAll( orig - inV , radius , inV.normalized , inV.magnitude * 2 , 1 << 6 );
            n++;
        }
        return inV;
    }
}
