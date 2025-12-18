using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;

public class HandManager : MonoBehaviour
{
    public HandSlot handSlot;
    public GameObject defaultProjectile;
    public LayerMask targetLayer;

    // 임시
    public TextMeshProUGUI handTextUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handSlot = gameObject.AddComponent<HandSlot>();
        handSlot.Setup(defaultProjectile, targetLayer);
    }

    // Update is called once per frame
    void Update()
    {
        handTextUI.text = handSlot.handText;
    }
}
