using System.Collections.Generic;
using System.Linq;

public struct HandResult
{
    public HandRank Rank;
    public List<CardData> WinningCards; // 족보를 구성하는 핵심 카드들 (키커 제외)
}

public static class PokerEvaluator
{
    public static HandResult EvaluateHand(List<CardData> cards)
    {
        // 입력 카드가 없으면 예외 처리 혹은 HighCard 반환
        if (cards == null || cards.Count == 0)
            return new HandResult { Rank = HandRank.HighCard, WinningCards = new List<CardData>() };

        // 1. 정렬 (내림차순: A -> K -> ... -> 2)
        // 내림차순이 높은 족보 찾기에 훨씬 유리합니다.
        cards = cards.OrderByDescending(c => c.rank).ToList();

        // 2. 플러시 & 스트레이트 플러시 체크 (5장 이상일 때만)
        if (cards.Count >= 5)
        {
            var flushGroup = cards.GroupBy(c => c.suit).FirstOrDefault(g => g.Count() >= 5);
            if (flushGroup != null)
            {
                // 플러시가 가능한 수트의 카드들만 모음
                List<CardData> flushCards = flushGroup.ToList();

                // 그 안에서 스트레이트가 되는지 확인 (스트레이트 플러시)
                List<CardData> straightFlushCards = GetStraightCards(flushCards);
                if (straightFlushCards != null)
                {
                    // 로얄 플러시 체크
                    if (straightFlushCards[0].rank == CardRank.Ace && straightFlushCards[1].rank == CardRank.King)
                        return new HandResult { Rank = HandRank.RoyalFlush, WinningCards = straightFlushCards };

                    return new HandResult { Rank = HandRank.StraightFlush, WinningCards = straightFlushCards };
                }

                // 그냥 플러시 (상위 5장만)
                return new HandResult { Rank = HandRank.Flush, WinningCards = flushCards.Take(5).ToList() };
            }

            // 일반 스트레이트 체크
            List<CardData> straightCards = GetStraightCards(cards);
            if (straightCards != null)
            {
                return new HandResult { Rank = HandRank.Straight, WinningCards = straightCards };
            }
        }

        // 3. 페어/트리플 계열 체크 (카드 개수 상관 없음)
        var rankGroups = cards.GroupBy(c => c.rank).ToList();

        // 4카드 확인
        var fourOfAKind = rankGroups.FirstOrDefault(g => g.Count() == 4);
        if (fourOfAKind != null)
        {
            return new HandResult { Rank = HandRank.FourOfAKind, WinningCards = fourOfAKind.ToList() };
        }

        // 3장(Triple) 그룹과 2장(Pair) 그룹 찾기
        var trips = rankGroups.Where(g => g.Count() == 3).ToList();
        var pairs = rankGroups.Where(g => g.Count() == 2).ToList();

        // 풀하우스 (트리플이 2개이거나, 트리플1+페어1 이상)
        if (trips.Count > 0)
        {
            // 트리플이 2개 이상이면 더 높은 트리플이 Main, 낮은 트리플이 Pair 역할
            if (trips.Count >= 2)
            {
                // 제일 높은 트리플 + 그 다음 높은 트리플(에서 2장만 가져옴)
                var mainTrips = trips[0].ToList();
                var subPair = trips[1].Take(2).ToList();
                return new HandResult { Rank = HandRank.FullHouse, WinningCards = mainTrips.Concat(subPair).ToList() };
            }
            // 트리플 1개 + 페어 1개 이상
            else if (pairs.Count >= 1)
            {
                var mainTrips = trips[0].ToList();
                var subPair = pairs[0].ToList(); // 이미 정렬되어 있으므로 가장 높은 페어
                return new HandResult { Rank = HandRank.FullHouse, WinningCards = mainTrips.Concat(subPair).ToList() };
            }

            // 그냥 트리플 (One Three of a Kind)
            return new HandResult { Rank = HandRank.ThreeOfAKind, WinningCards = trips[0].ToList() };
        }

        // 투페어
        if (pairs.Count >= 2)
        {
            var pair1 = pairs[0].ToList();
            var pair2 = pairs[1].ToList();
            return new HandResult { Rank = HandRank.TwoPair, WinningCards = pair1.Concat(pair2).ToList() };
        }

        // 원페어
        if (pairs.Count == 1)
        {
            return new HandResult { Rank = HandRank.OnePair, WinningCards = pairs[0].ToList() };
        }

        // 하이카드 (가장 높은 1장만)
        return new HandResult { Rank = HandRank.HighCard, WinningCards = new List<CardData> { cards[0] } };
    }

    // --- 헬퍼 함수: N장의 카드 중 스트레이트 구성 5장 찾기 ---
    private static List<CardData> GetStraightCards(List<CardData> cards)
    {
        // 중복 숫자 제거하고 내림차순 정렬 (K, Q, J...)
        var distinctCards = cards.GroupBy(c => c.rank).Select(g => g.First()).OrderByDescending(c => c.rank).ToList();

        // 5장 미만이면 스트레이트 불가
        if (distinctCards.Count < 5) return null;

        // 일반 스트레이트 검사
        for (int i = 0; i <= distinctCards.Count - 5; i++)
        {
            // 현재 카드와 4번째 뒤 카드의 랭크 차이가 4라면 연속된 것임 (예: 10 - 6 = 4)
            if ((int)distinctCards[i].rank - (int)distinctCards[i + 4].rank == 4)
            {
                return distinctCards.GetRange(i, 5);
            }
        }

        // 백스트레이트 (Wheel) 검사: A, 5, 4, 3, 2
        // 조건: A가 있고, 5,4,3,2가 연속으로 있어야 함
        if (distinctCards[0].rank == CardRank.Ace)
        {
            // 5부터 시작하는 스트레이트 구간 찾기
            var fiveIndex = distinctCards.FindIndex(c => c.rank == CardRank.Five);
            if (fiveIndex != -1 && fiveIndex <= distinctCards.Count - 4)
            {
                // 5, 4, 3, 2 가 존재하는지 확인
                // 5가 발견된 곳부터 4장이 연속인지 확인 (5-2 = 3 차이)
                if ((int)distinctCards[fiveIndex].rank - (int)distinctCards[fiveIndex + 3].rank == 3 &&
                    distinctCards[fiveIndex + 3].rank == CardRank.Two)
                {
                    var wheel = new List<CardData>();
                    wheel.AddRange(distinctCards.GetRange(fiveIndex, 4)); // 5,4,3,2
                    wheel.Add(distinctCards[0]); // Ace 추가
                    return wheel; // 5,4,3,2,A 순서 반환
                }
            }
        }

        return null;
    }
}