using UnityEngine;
using UnityEngine.InputSystem; // 네임스페이스 필수

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 5f;

    [Header("Components")]
    private Rigidbody2D rb;
    private GameControls controls; // 자동 생성된 C# 클래스
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new GameControls(); // 인스턴스 생성
    }

    void OnEnable()
    {
        // 입력 활성화
        controls.Enable();
    }

    void OnDisable()
    {
        // 입력 비활성화 (플레이어가 죽거나, 메뉴 창 떴을 때)
        controls.Disable();
    }

    void Update()
    {
        // 1. 입력 값 읽어오기 (매 프레임)
        // Player 맵의 Move 액션 값을 Vector2로 가져옴
        moveInput = controls.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // 2. 물리 이동 적용 (물리 연산은 FixedUpdate에서)
        // 뱀서류는 관성 없이 즉각적으로 멈추는 게 조작감이 좋음 (Velocity 직접 제어)
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
