using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Necro
{
    public class DebugView : MonoBehaviour
    {
        public bool isInitialized = false;

        public bool needsPopulate = false;
        public List<SphereCollider> sphereColliderList;
        public List<CapsuleCollider> capsuleColliderList;
        public List<MeshCollider> meshColliderList;

        public float timer = 0f;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            needsPopulate = false;
            sphereColliderList = new List<SphereCollider>();
            capsuleColliderList = new List<CapsuleCollider>();
            meshColliderList = new List<MeshCollider>();
            isInitialized = true;
            timer = 3f;
        }

        public void Update()
        {
            timer-=Time.deltaTime;
            if (timer < 0f)
            {
                needsPopulate = true;
                timer = 99999999f;
            }
            if (needsPopulate)
            {
                sphereColliderList.Clear();
                sphereColliderList.AddRange( FindObjectsOfType(typeof(SphereCollider)) as SphereCollider[] );
                capsuleColliderList.Clear();
                capsuleColliderList.AddRange( FindObjectsOfType(typeof(CapsuleCollider)) as CapsuleCollider[] );
                meshColliderList.Clear();
                meshColliderList.AddRange( FindObjectsOfType(typeof(MeshCollider)) as MeshCollider[] );
                needsPopulate = false;
            }
            DrawUpdate();
        }

         public void OnDrawGizmos() 
        {
            if (!isInitialized)
            {
                return;
            }
            for(int i=0; i<sphereColliderList.Count; ++i)
            {
                SphereCollider collider = sphereColliderList[i];
                Gizmos.color = collider.enabled ? Color.green : Color.red;
                if (Mathf.Abs(collider.center.x + collider.center.y) > 0.01f)
                {
                    //Gizmos.DrawWireSphere(collider.transform.position+collider.center, collider.radius);
                    Vector3 adjustedCenter = collider.transform.localToWorldMatrix.MultiplyPoint3x4(collider.center);
                    Gizmos.DrawWireSphere(adjustedCenter, collider.radius);
                }
            }
            for(int i=0; i<capsuleColliderList.Count; ++i)
            {
                CapsuleCollider collider = capsuleColliderList[i];
                Gizmos.color = collider.enabled ? Color.green : Color.red;
                Gizmos.DrawWireSphere(collider.transform.position+collider.center-Vector3.up*(collider.height/2-collider.radius), collider.radius);
                Gizmos.DrawWireSphere(collider.transform.position+collider.center+Vector3.up*(collider.height/2-collider.radius), collider.radius);
            }
        }



        public void DrawUpdate() 
        {
            if (!isInitialized)
            {
                return;
            }
            for(int i=0; i<sphereColliderList.Count; ++i)
            {
                SphereCollider collider = sphereColliderList[i];
                Color drawingColor = collider.enabled ? Color.green : Color.red;
				if (Mathf.Abs(collider.center.x + collider.center.y) > 0.01f)
				{
	                Vector3 adjustedCenter = collider.transform.localToWorldMatrix.MultiplyPoint3x4(collider.center);
					DebugRenderer.Instance.AddSphere(DebugRenderMode.COLLISION, adjustedCenter, collider.radius, drawingColor);
				}
            }
            for(int i=0; i<capsuleColliderList.Count; ++i)
            {
                CapsuleCollider collider = capsuleColliderList[i];
                Color drawingColor = collider.enabled ? Color.green : Color.red;
                Vector3 adjustedCenter = collider.transform.localToWorldMatrix.MultiplyPoint3x4(collider.center);
                DebugRenderer.Instance.AddCapsule(DebugRenderMode.COLLISION, adjustedCenter, collider.radius, collider.height, drawingColor);
            }

            for(int i=0; i<meshColliderList.Count; ++i)
            {
                //MeshCollider collider = meshColliderList[i];
                //Color drawingColor = collider.enabled ? Color.green : Color.red;
                //Vector3 adjustedCenter = collider.transform.localToWorldMatrix.MultiplyPoint3x4(collider.center);
                //DebugRenderer.Instance.AddBox(DebugRenderMode.COLLISION, adjustedCenter, collider.radius, collider.height, drawingColor);
            }
        }

        
    }
}