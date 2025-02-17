using DG.Tweening;
using System;
using UnityEngine;

public class Hammering : MonoBehaviour
{
    [Serializable]
    public struct ActionTransform
    {
        public Vector3 pos;
        public Vector3 rot;
    }

    public struct InitData
    {
        public Action hitCallback;
    }

    [SerializeField] Transform rightArmTarget;
    public ActionTransform rightReadyTr;
    public ActionTransform rightHitTr;

    [SerializeField] Transform leftArmTarget;
    public ActionTransform leftReadyTr;
    public ActionTransform leftHitTr;

    public float time;
    bool isDone = true;

    Action hitCallback;

    public void Init(InitData data)
    {
        hitCallback = data.hitCallback;

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

    public void Do()
    {
        Sequence sequence = DOTween.Sequence();

        isDone = false;
        sequence.Append(rightArmTarget.DOLocalMove(rightHitTr.pos, time).SetEase(Ease.InExpo));
        sequence.Join(rightArmTarget.DOLocalRotate(rightHitTr.rot, time).SetEase(Ease.InExpo));

        sequence.AppendCallback(() => hitCallback());
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
