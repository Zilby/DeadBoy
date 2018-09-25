﻿//
//SpringBone.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpringBone.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/20
//Anima2D adaptation by Mandarina Games 2016/03/18
//
using UnityEngine;
using System.Collections;
using Anima2D;

namespace UnityChan
{
	[RequireComponent(typeof(Bone2D))]
	public class SpringBone : MonoBehaviour
	{
		public float radius = 0.05f;

		//各SpringBoneに設定されているstiffnessForceとdragForceを使用するか？
		public bool isUseEachBoneForceSettings = false; 

		//バネが戻る力
		public float stiffnessForce = 0.01f;

		//力の減衰力
		public float dragForce = 0.4f;
		public Vector3 springForce = new Vector3 (0.0f, -0.0001f, 0.0f);
		public SpringCollider[] colliders;

		//Kobayashi:Thredshold Starting to activate activeRatio
		public float threshold = 0.01f;

		public bool isAnimated = false;

		private float springLength;
		private Quaternion localRotation;
		private Vector3 currTipPos;
		private Vector3 prevTipPos;
		//Kobayashi
		private Transform org;
		//Kobayashi:Reference for "SpringManager" component with unitychan 
		private SpringManager managerRef;

		private Vector3 oldManagerRotation;

		private Bone2D m_Bone;
		
		private void Awake ()
		{
			m_Bone = GetComponent<Bone2D>();

			localRotation = transform.localRotation;
			//Kobayashi:Reference for "SpringManager" component with unitychan
			// GameObject.Find("unitychan_dynamic").GetComponent<SpringManager>();
			managerRef = GetParentSpringManager (transform);
			oldManagerRotation = managerRef.transform.eulerAngles;
		}

		private void Start()
		{
			springLength = Vector3.Distance(transform.position, m_Bone.endPosition);
			currTipPos = m_Bone.endPosition;
			prevTipPos = m_Bone.endPosition;
		}

		private SpringManager GetParentSpringManager (Transform t)
		{
			var springManager = t.GetComponent<SpringManager> ();

			if (springManager != null)
				return springManager;

			if (t.parent != null) {
				return GetParentSpringManager (t.parent);
			}

			return null;
		}

		public void UpdateSpring ()
		{

			if(managerRef.transform.eulerAngles != oldManagerRotation) 
			{
				oldManagerRotation = managerRef.transform.eulerAngles;
				for (int i = 0; i < 10; ++i) {
					UpdateSpring();
				}
			}

			//Kobayashi
			org = transform;
			//回転をリセット
			if(!isAnimated)
			{
				transform.localRotation = Quaternion.identity * localRotation;
			}

			float sqrDt = Time.deltaTime * Time.deltaTime;

			//stiffness
			Vector3 force = transform.rotation * (Vector3.right * stiffnessForce) / sqrDt;

			//drag
			force += (prevTipPos - currTipPos) * dragForce / sqrDt;

			force += springForce / sqrDt;

			//前フレームと値が同じにならないように
			Vector3 temp = currTipPos;

			//verlet
			currTipPos = (currTipPos - prevTipPos) + currTipPos + (force * sqrDt);

			//長さを元に戻す
			currTipPos = ((currTipPos - transform.position).normalized * springLength) + transform.position;

			//衝突判定
			for (int i = 0; i < colliders.Length; i++) {
				if (Vector3.Distance (currTipPos, colliders [i].transform.position) <= (radius + colliders [i].radius)) {
					Vector3 normal = (currTipPos - colliders [i].transform.position).normalized;
					currTipPos = colliders [i].transform.position + (normal * (radius + colliders [i].radius));
					currTipPos = ((currTipPos - transform.position).normalized * springLength) + transform.position;
				}


			}

			prevTipPos = temp;

			//回転を適用；
			Vector3 aimVector = transform.TransformDirection (Vector3.right);
			Quaternion aimRotation = Quaternion.FromToRotation (aimVector, currTipPos - transform.position);
			//original
			//trs.rotation = aimRotation * trs.rotation;
			//Kobayahsi:Lerp with mixWeight
			Quaternion secondaryRotation = aimRotation * transform.rotation;
			transform.rotation = Quaternion.Lerp (org.rotation, secondaryRotation, managerRef.dynamicRatio);
		}
	}
}
