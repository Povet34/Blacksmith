using DG.Tweening;
using System;
using Unity.Physics;
using UnityEngine;

public class Hammering : MonoBehaviour
{
    [Serializable]
    public struct ActionTransform
    {
        public Vector3 pos;
        public Vector3 rot;
    }

    [SerializeField] ParticleSystem hitEffect;

    [SerializeField] Transform rightArmTarget;
    public ActionTransform rightReadyTr;
    public ActionTransform rightHitTr;

    [SerializeField] Transform leftArmTarget;
    public ActionTransform leftReadyTr;
    public ActionTransform leftHitTr;

    public float time;
    bool isDone = true;

    public float pressureForce;
    public float pressureOffset;

    public void Init()
    {
        rightArmTarget.localPosition = rightReadyTr.pos;
        rightArmTarget.localRotation = Quaternion.Euler(rightReadyTr.rot);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(isDone)
                Do();
        }
    }

    void ApplyHit()
    {
        var raycastHit = MouseInputHelper.GetRaycastHit();

        VertexMover vertexMover = raycastHit.collider.GetComponent<VertexMover>();
        if (vertexMover != null)
        {
            Vector3 inputPoint = raycastHit.point + (raycastHit.normal * pressureOffset);
            vertexMover.ApplyPressureToPoint(inputPoint, pressureForce);
            var particle = Instantiate(hitEffect, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
            Destroy(particle.gameObject, particle.main.duration);
        }
    }

    public void Do()
    {
        Sequence sequence = DOTween.Sequence();

        isDone = false;
        sequence.Append(rightArmTarget.DOLocalMove(rightHitTr.pos, time).SetEase(Ease.InExpo));
        sequence.Join(rightArmTarget.DOLocalRotate(rightHitTr.rot, time).SetEase(Ease.InExpo));

        sequence.AppendCallback(() => ApplyHit());
        sequence.AppendInterval(0.1f);

        sequence.Append(rightArmTarget.DOLocalMove(rightReadyTr.pos, 0.2f));
        sequence.Join(rightArmTarget.DOLocalRotate(rightReadyTr.rot, 0.2f));

        sequence.AppendCallback(() =>
        {
            isDone = true;
        });

        sequence.Play();
    }
}
