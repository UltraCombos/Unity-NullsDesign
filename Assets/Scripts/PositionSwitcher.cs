using HomographySharp;
using HomographySharp.Single;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos
{
	[ExecuteInEditMode]
	public class PositionSwitcher : MonoBehaviour
	{
		public enum SoureType
		{
			DEMO,
			SPOUT,
		}

		[Header( "Config" )]
		public SoureType source;
		public Texture demoTexture;
		public Texture spoutTexture;

		[Header( "Camera" )]
		[Range( 0, 1 )]
		public float positioning = 0;
		public FrustumFitter frustumCenter;
		public FrustumFitter frustumLeft;
		public FrustumFitter frustumRight;

		[Header( "Point of View" )]
		public Transform viewCenter;
		public Transform viewSideLeft;
		public Transform viewSideRight;

		[Header( "Renderer" )]
		public Renderer rendererLeft;
		public Renderer rendererRight;
		const string PROP_MAIN_TEXTURE = "_MainTexture";
		const string PROP_HOMOGRAPHY = "_Homography";
		const string PROP_TILING = "_Tiling";
		const string PROP_OFFSET = "_Offset";

		private void Update()
		{
			/*
			frustumLeft.target.transform.position = Vector3.Lerp( viewSideLeft.position, viewCenter.position, positioning );
			frustumRight.target.transform.position = Vector3.Lerp( viewSideRight.position, viewCenter.position, positioning );

			UpdateRenderer( frustumLeft, frustumCenter.target, new Vector2( 0.5f, 1.0f ), new Vector2( 0.0f, 0.0f ), rendererLeft );
			UpdateRenderer( frustumRight, frustumCenter.target, new Vector2( 0.5f, 1.0f ), new Vector2( 0.5f, 0.0f ), rendererRight );
			*/
		}

		private void UpdateRenderer(FrustumFitter frustum, Camera cam, Vector2 tiling, Vector2 offset, Renderer rdr)
		{
			var uvs = new List<Vector2>()
			{
				offset + tiling * new Vector2( 0, 0 ),
				offset + tiling * new Vector2( 1, 0 ),
				offset + tiling * new Vector2( 1, 1 ),
				offset + tiling * new Vector2( 0, 1 ),
			};
			var src = new List<Point2<float>>();
			foreach ( var uv in uvs )
				src.Add( new Point2<float>( uv.x, uv.y ) );

			var dst = new List<Point2<float>>();
			foreach ( var c in frustum.corners )
			{
				var p = cam.WorldToViewportPoint( c, Camera.MonoOrStereoscopicEye.Mono );
				dst.Add( new Point2<float>( p.x, p.y ) );
			}
			var homo = SingleHomographyHelper.FindHomography( src, dst );
			var matrix = Matrix4x4.identity;
			const int DIM = 3;
			for ( int i = 0; i < homo.Elements.Count; ++i )
			{
				int row = i / DIM;
				int col = i % DIM;
				matrix[row, col] = homo.Elements[i];
			}

			var prop = new MaterialPropertyBlock();
			prop.SetMatrix( PROP_HOMOGRAPHY, matrix );
			prop.SetVector( PROP_TILING, tiling );
			prop.SetVector( PROP_OFFSET, offset );
			prop.SetTexture( PROP_MAIN_TEXTURE, (source == SoureType.DEMO) ? demoTexture : spoutTexture );

			rdr.SetPropertyBlock( prop );
		}
	}
}

