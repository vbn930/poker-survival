using UnityEngine;

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckManager : MonoBehaviour
{
    [Header("Deck Settings")]
    // 유니티 에디터에서 CardData 템플릿(빈 껍데기)이나 리소스를 연결할 곳
    // 실제로는 코드로 생성하겠지만, 스프라이트 관리를 위해 리스트를 둘 수도 있습니다.
    // 지금은 로직 검증용으로 순수 데이터만 생성합니다.

    // 현재 덱에 있는 카드들 (Draw Pile)
    public List<CardData> deck = new List<CardData>();

    // 이미 사용한 카드들 (Discard Pile)
    public List<CardData> discardPile = new List<CardData>();

    // 임시 프로토타입용 Text
    public TextMeshProUGUI remain;
    public TextMeshProUGUI discard;

    void Start()
    {
        InitializeDeck();
    }

    void Update()
    {
        // 덱과 버린 카드 수량 표시
        if (remain != null)
            remain.text = $"Remaining Cards: {deck.Count}";
        if (discard != null)
            discard.text = $"Discarded Cards: {discardPile.Count}";
    }

    // 1. 초기 덱 생성 (52장)
    void InitializeDeck()
    {
        deck.Clear();
        discardPile.Clear();

        // 4개 문양 x 13개 숫자 = 52장 생성
        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardRank rank in System.Enum.GetValues(typeof(CardRank)))
            {
                // ScriptableObject를 런타임에 메모리에 생성 (임시)
                // 나중에는 실제 에셋을 로드하는 방식으로 변경 가능
                CardData newCard = ScriptableObject.CreateInstance<CardData>();
                newCard.suit = suit;
                newCard.rank = rank;
                newCard.name = $"{suit}_{rank}";

                deck.Add(newCard);
            }
        }

        Debug.Log($"[DeckManager] 덱 생성 완료! 총 {deck.Count}장");
        ShuffleDeck();
    }

    // 2. 덱 섞기 (Shuffle)
    public void ShuffleDeck()
    {
        // 피셔-예이츠 셔플 알고리즘
        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        Debug.Log("[DeckManager] 덱을 섞었습니다.");
    }

    private CardData DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogError("[DeckManager] 덱과 버린 카드 모두 비어있습니다! 카드를 뽑을 수 없습니다.");
            return null;
        }
        CardData drawnCard = deck[0];
        deck.RemoveAt(0);
        Debug.Log($"[DeckManager] 카드를 뽑았습니다: {drawnCard.GetCardName()}");
        return drawnCard;
    }

    public List<CardData> DrawCards(int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning("[DeckManager] 뽑을 카드 수는 1장 이상이어야 합니다.");
            return new List<CardData>();
        }

        if (deck.Count < count)
        {
            Debug.LogWarning("[DeckManager] 덱에 충분한 카드가 없습니다. 버린 카드로 덱을 재생성합니다.");
            ReplenishDeckFromDiscard();
        }

        List<CardData> drawnCards = new List<CardData>();
        for (int i = 0; i < count; i++)
        {
            CardData card = DrawCard();
            DiscardCard(card);

            if (card != null)
            {
                drawnCards.Add(card);
            }
        }
        return drawnCards;
    }

    public void DiscardCard(CardData card)
    {
        if (card != null)
        {
            discardPile.Add(card);
            Debug.Log($"[DeckManager] 카드를 버렸습니다: {card.GetCardName()}");
        }
        else
        {
            Debug.LogWarning("[DeckManager] 버릴 카드가 null입니다.");
        }
    }

    private void ReplenishDeckFromDiscard()
    {
        if (discardPile.Count == 0)
        {
            Debug.LogError("[DeckManager] 버린 카드도 없습니다! 덱을 재생성할 수 없습니다.");
            return;
        }
        // 버린 카드를 덱으로 이동
        deck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
        Debug.Log("[DeckManager] 버린 카드로 덱을 재생성하고 섞었습니다.");
    }
}
