//
// Copyright (c) Brian Hernandez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;

public class EngineOld : MonoBehaviour
{
	[Range(0, 1)]
	public float throttle = 1.0f;

	[Tooltip("How much power the engine puts out.")]
	public float thrust;

	[Tooltip("Engine location to apply force from")]
	public Transform engineTransform;

	private Rigidbody rigid;

	public Transform engineAnimationMesh;

	private void Awake()
	{
		rigid = GetComponentInParent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (rigid != null)
		{
			rigid.AddForceAtPosition(engineTransform.forward * thrust * throttle, engineTransform.position, ForceMode.Force);
		}
		if (engineAnimationMesh)
		{
			engineAnimationMesh.Rotate(0, 0, thrust * throttle * Time.fixedDeltaTime);
		}
	}

}
