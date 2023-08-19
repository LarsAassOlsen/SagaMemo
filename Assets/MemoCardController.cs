using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MemoCardController : MonoBehaviour
{
    public MemoGameController GameController;

    public string Name;
    public MeshRenderer CardPictureMeshRenderer;
    public TextMeshPro NameTextMesh;

    public bool IsRevealed;
    public bool Matched = false;

    private Transform myTrans;
    private bool flipping = false;

    private void Start()
    {
        myTrans = transform;
    }

    public void SetupCard(string photoName, Texture2D photoTexture, MemoGameController gameController)
    {
        Name = photoName;
        NameTextMesh.text = photoName;
        CardPictureMeshRenderer.material.mainTexture = photoTexture;
        GameController = gameController;
    }

    public void FlipCard()
    {
        if (!Matched)
        {
            if (!flipping)
            {
                if (!IsRevealed)
                {
                    IsRevealed = true;
                    print("RevealCard");
                    StartCoroutine("RevealCardCoroutine");
                }
                else
                {
                    IsRevealed = false;
                    print("HideCard");
                    StartCoroutine("HideCardCoroutine");
                }
            }
        }
    }

    IEnumerator RevealCardCoroutine()
    {
        flipping = true;
        GameController.AllowInput = false;
        Vector3 initialPos = transform.position;
        Vector3 flipUpPos = transform.position + new Vector3(0, .7f, 0);
        myTrans.DOMove(flipUpPos, .5f).SetEase(Ease.OutExpo);
        myTrans.DORotate(new Vector3(0, 0, 0), .5f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(.4f);
        myTrans.DOMove(initialPos, .5f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(.6f);
        flipping = false;
        GameController.AllowInput = true;
    }

    IEnumerator HideCardCoroutine()
    {
        flipping = true;
        GameController.AllowInput = false;
        Vector3 initialPos = transform.position;
        Vector3 flipUpPos = transform.position + new Vector3(0, .7f, 0);
        myTrans.DOMove(flipUpPos, .4f).SetEase(Ease.OutExpo);
        myTrans.DORotate(new Vector3(0, 0, 180f), .4f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(.3f);
        myTrans.DOMove(initialPos, .4f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(.5f);
        flipping = false;
        GameController.AllowInput = true;
    }

}
