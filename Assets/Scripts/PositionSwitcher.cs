using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos
{
	[ExecuteInEditMode]
	public class PositionSwitcher : MonoBehaviour
	{
		[Range(0, 1)]
		public float progress = 0;
		public Transform leftCamera;
		public Transform rightCamera;

		public Transform centerPoV;
		public Transform leftPoV;
		public Transform rightPoV;

		private void Update()
		{
			leftCamera.position = Vector3.Lerp( centerPoV.position, leftPoV.position, progress );
			rightCamera.position = Vector3.Lerp( centerPoV.position, rightPoV.position, progress );
		}
	}
}

