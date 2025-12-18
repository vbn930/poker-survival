using UnityEngine;

// 문양 (Suit) 정의
public enum CardSuit
{
    Spade,   // ♠ 물리/크리티컬
    Heart,   // ♥ 흡혈/회복
    Diamond, // ♦ 골드/방어
    Clover   // ♣ 독/유틸
}

// 등급 (Rank) 정의 (2~10, J, Q, K, A)
// 계산 편의를 위해 숫자로 매핑 (J=11, Q=12, K=13, A=14)
public enum CardRank
{
    Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10,
    Jack = 11, Queen = 12, King = 13, Ace = 14
}

public enum HandRank { 
    HighCard, OnePair, TwoPair, ThreeOfAKind, 
    Straight, Flush, FullHouse, FourOfAKind, 
    StraightFlush, RoyalFlush 
}

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [Header("Card Identity")]
    public CardSuit suit;
    public CardRank rank;

    [Header("Visuals")]
    public Sprite icon;       // 인게임 UI용 아이콘
    public Color themeColor;  // 문양별 테마 색상 (Spade=검정/파랑, Heart=빨강 등)

    // 카드의 이름 반환 (예: "Spade Ace")
    public string GetCardName()
    {
        return $"{suit} {rank}";
    }

    // 카드의 실제 데미지 계수 (나중에 밸런싱할 때 사용)
    public int GetPowerValue()
    {
        return (int)rank;
    }

    public int CompareTo(CardData other)
    {
        return this.rank.CompareTo(other.rank);
    }
}
