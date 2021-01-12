using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UltraCombos
{
	[ExecuteInEditMode]
	public class FrustumFitter : MonoBehaviour
	{
		const float size = 3;
		public BoxCollider frame;
		public Camera target;

		Matrix4x4 m => frame.transform.localToWorldMatrix;
		Vector3 pa => m.MultiplyPoint( Vector3.Scale( frame.size, new Vector3( -0.5f, -0.5f, 0 ) ) );
		Vector3 pb => m.MultiplyPoint( Vector3.Scale( frame.size, new Vector3( +0.5f, -0.5f, 0 ) ) );
		Vector3 pc => m.MultiplyPoint( Vector3.Scale( frame.size, new Vector3( -0.5f, +0.5f, 0 ) ) );
		Vector3 pd => m.MultiplyPoint( Vector3.Scale( frame.size, new Vector3( +0.5f, +0.5f, 0 ) ) );
		Vector3 pe => target.transform.position;

		Vector3[] corners => new Vector3[4] { pa, pb, pd, pc };

		private void LateUpdate()
		{
			if ( target == null || frame == null )
				return;

			float _scale = 1;

			float n = target.nearClipPlane;
			float f = target.farClipPlane;
#if false
			var m = frame.transform.localToWorldMatrix;
			Vector3 pa = m.MultiplyPoint( Vector3.Scale( box.size, new Vector3( -0.5f, -0.5f, 0 ) ) );
			Vector3 pb = m.MultiplyPoint( Vector3.Scale( box.size, new Vector3( +0.5f, -0.5f, 0 ) ) );
			Vector3 pc = m.MultiplyPoint( Vector3.Scale( box.size, new Vector3( -0.5f, +0.5f, 0 ) ) );
			Vector3 pe = target.transform.position;
#endif
			// Compute an orthonormal basis for the screen.
			Vector3 vr = (pb - pa).normalized;
			Vector3 vu = (pc - pa).normalized;
			Vector3 vn = Vector3.Cross( vu, vr ).normalized;

			// Compute the screen corner vectors.
			Vector3 va = pa - pe;
			Vector3 vb = pb - pe;
			Vector3 vc = pc - pe;

			// Find the distance from the eye to screen plane.
			float d = -Vector3.Dot( va, vn );

			// Find the extent of the perpendicular projection.
			float nd = n / d * _scale;

			float l = Vector3.Dot( vr, va ) * nd;
			float r = Vector3.Dot( vr, vb ) * nd;
			float b = Vector3.Dot( vu, va ) * nd;
			float t = Vector3.Dot( vu, vc ) * nd;

			// Load the perpendicular projection.
			Matrix4x4 P = Matrix4x4.Frustum( l, r, b, t, n, f );
			target.projectionMatrix = P;
			target.transform.rotation = Quaternion.LookRotation( -vn, vu );
		}

		private void OnDrawGizmos()
		{
			if ( target == null || frame == null )
				return;
#if UNITY_EDITOR
			using ( new Handles.DrawingScope( Color.cyan ) )
			{
				for ( int i = 0; i < corners.Length; ++i )
				{
					int j = (i + 1) % corners.Length;
					Handles.DrawDottedLine( corners[i], corners[j], size );
					Handles.DrawDottedLine( pe, corners[i], size );
				}
				using ( new Handles.DrawingScope( frame.transform.localToWorldMatrix ) )
				{
					Handles.DrawLine( Vector3.zero, new Vector3( 0, 0, -1 ) );
					Handles.DrawLine( new Vector3( 0, 0, -1 ), new Vector3( 0.0f, 0.3f, -0.7f ) );
				}
			}
#endif
		}
	}

}
