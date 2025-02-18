using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MouseInput : MonoBehaviour {

    [SerializeField] ParticleSystem hitEffect;

    public float pressureForce;
	//We need to apply an offset when adding force to our 
	//nesh to ensure that the vertices arent being pushed apart but rather 
	//pushed inwards - therefore we are combining this offset with the input points normal
    public float pressureOffset;

    private Ray mouseRay;
    private RaycastHit raycastHit;

    Hammering hammering;

    private void Start()
    {
        Hammering.InitData hammerInitData = new Hammering.InitData();
        hammerInitData.hitCallback = ProcessHitEffect;
        hammering?.Init(hammerInitData);
    }

    void ProcessHitEffect()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //We need to check if we are clicking on a mesh we can deform
        if (Physics.Raycast(mouseRay, out raycastHit))
        {
            VertexMover vertexMover = raycastHit.collider.GetComponent<VertexMover>();
            if (vertexMover != null)
            {
                Vector3 inputPoint = raycastHit.point + (raycastHit.normal * pressureOffset);
                vertexMover.ApplyPressureToPoint(inputPoint, pressureForce);

                var particle = Instantiate(hitEffect, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
                Destroy(particle.gameObject, particle.main.duration);
            }
        }
    }
}
