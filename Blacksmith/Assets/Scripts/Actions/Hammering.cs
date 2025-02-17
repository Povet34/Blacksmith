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

    [SerializeField] Transform rightArmTarget;
    public ActionTransform readyTr;
    public ActionTransform hitTr;

    bool isDone = true;

    private void Start()
    {
        rightArmTarget.localPosition = readyTr.pos;
        rightArmTarget.localRotation = Quaternion.Euler(readyTr.rot);
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
        sequence.AppendCallback(() =>
        {
            rightArmTarget.DOLocalMove(hitTr.pos, 0.5f);
            rightArmTarget.DOLocalRotate(hitTr.rot, 0.5f);
        });

        sequence.AppendInterval(0.2f);

        sequence.AppendCallback(() => 
        {
            rightArmTarget.DOLocalMove(readyTr.pos, 0.5f);
            rightArmTarget.DOLocalRotate(readyTr.rot, 0.5f);
        });

        sequence.AppendCallback(() =>
        {
            isDone = true;
        });

        sequence.Play();
    }
}
