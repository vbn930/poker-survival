using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [SerializeField] private Transform targetCamera; // 메인 카메라 연결

    private float textureUnitSizeX;
    private float textureUnitSizeY;

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;

        // 텍스처 하나의 유닛 단위 크기 계산 (픽셀 / PPU)
        textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = sprite.texture.height / sprite.pixelsPerUnit;

        // Draw Mode가 Tiled일 때, 스케일이 1이어야 정확히 계산됨 (혹은 스케일을 곱해줘야 함)
    }

    void LateUpdate()
    {
        // 카메라와 배경의 위치 차이 계산
        Vector3 deltaMovement = targetCamera.position - transform.position;

        // X축 처리: 카메라가 텍스처 크기만큼 이동했으면 배경을 그만큼 이동시킴
        if (Mathf.Abs(deltaMovement.x) >= textureUnitSizeX)
        {
            float offsetAmountX = (deltaMovement.x > 0) ? textureUnitSizeX : -textureUnitSizeX;
            transform.position += new Vector3(offsetAmountX, 0, 0);
        }

        // Y축 처리: 위와 동일
        if (Mathf.Abs(deltaMovement.y) >= textureUnitSizeY)
        {
            float offsetAmountY = (deltaMovement.y > 0) ? textureUnitSizeY : -textureUnitSizeY;
            transform.position += new Vector3(0, offsetAmountY, 0);
        }
    }
}
