using DG.Tweening;
using System;
using Unity.Physics;
using UnityEngine;

public class Hammering : MonoBehaviour, IForgeAction
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

    public float time = 0.1f;
    bool isHammeringDone = true;

    public float pressureForce = 3;
    public float pressureOffset;

    FPSArm arm;
    public FPSArm FPSArm { get => arm; set => arm = value; }

    public void Init()
    {
        arm = GetComponent<FPSArm>();

        rightArmTarget.localPosition = rightReadyTr.pos;
        rightArmTarget.localRotation = Quaternion.Euler(rightReadyTr.rot);
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

    public void UpdateAction()
    {
        DoHammering();
    }

    public void WheelAction(float value)
    {
        MoveIngot(value);
    }

    void MoveIngot(float value)
    {
        arm.arm_l.Translate(Vector3.right * value * 0.3F, Space.World);
    }

    void DoHammering()
    {
        if (!isHammeringDone)
            return;

        Sequence sequence = DOTween.Sequence();

        isHammeringDone = false;
        sequence.Append(rightArmTarget.DOLocalMove(rightHitTr.pos, time).SetEase(Ease.InExpo));
        sequence.Join(rightArmTarget.DOLocalRotate(rightHitTr.rot, time).SetEase(Ease.InExpo));

        sequence.AppendCallback(() => ApplyHit());
        sequence.AppendInterval(0.1f);

        sequence.Append(rightArmTarget.DOLocalMove(rightReadyTr.pos, 0.2f));
        sequence.Join(rightArmTarget.DOLocalRotate(rightReadyTr.rot, 0.2f));

        sequence.AppendCallback(() =>
        {
            isHammeringDone = true;
        });

        sequence.Play();
    }
}
