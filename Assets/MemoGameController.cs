using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MemoGameController : MonoBehaviour
{
    public GameObject CardPrefab;
    public List<MemoCard> MemoCards;
    private List<MemoCard> memoCardsInPlay = new List<MemoCard>();

    public MemoCardController FirstCardFlipped;
    public MemoCardController SecondCardFlipped;

    public float gridX = 4f;
    public float gridY = 4f;
    public float spacing = 2f;

    public bool AllowInput = true;

    void Start()
    {
        InitiateGame();
    }

    private void InitiateGame()
    {
        MemoCards.Clear();
        // Loads card textures from Resource folder
        var textures = Resources.LoadAll("Textures", typeof(Texture2D)).Cast<Texture2D>().ToArray();
        foreach (var t in textures)
        {
            MemoCard cardToAdd = new MemoCard();
            cardToAdd.Texture = t;
            cardToAdd.Name = t.name;
            MemoCards.Add(cardToAdd);
        }

        //Shuffles cards before duplication
        Shuffle(MemoCards);

        //Reduces list of cards to 6
        var cardsToSpawn = MemoCards.GetRange(0, 6);

        //Duplicates cards
        for (int i = 0; i < cardsToSpawn.Count; i++)
        {
            memoCardsInPlay.Add(MemoCards[i]);
            memoCardsInPlay.Add(MemoCards[i]);
        }
        Shuffle(memoCardsInPlay);
        //Spawns cards
        int spawnedCardNumber = 0;
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                Vector3 pos = new Vector3(x, 0, y) * spacing;
                GameObject cardPrefab = Instantiate(CardPrefab, pos, new Quaternion(0, 0, 180f, 0));
                cardPrefab.GetComponent<MemoCardController>().SetupCard(memoCardsInPlay[spawnedCardNumber].Name, memoCardsInPlay[spawnedCardNumber].Texture, this);
                spawnedCardNumber++;
            }
        }
        for (int i = 0; i < memoCardsInPlay.Count; i++)
        {
            /*
            float posX = (float)i;
            float adjustedPosX = (posX * 1.2f);
            GameObject cardPrefab = Instantiate(CardPrefab, new Vector3(adjustedPosX, 0.05f, 0), new Quaternion(0,0,180f,0));
            cardPrefab.GetComponent<MemoCardController>().SetupCard(memoCardsInPlay[i].Name, memoCardsInPlay[i].Texture);
            */
        }
    }
    
    //Shuffles order of cards
    public static void Shuffle(List<MemoCard> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
    
    void Update()
    {
        if (AllowInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f) && hit.transform.tag == "MemoCard")
                {
                    AllowInput = false;
                    MemoCardController hitCard = hit.transform.GetComponent<MemoCardController>();
                    if (!hitCard.Matched)
                    {
                        if (FirstCardFlipped == null)
                        {
                            FirstCardFlipped = hitCard;
                            hitCard.FlipCard();
                        }
                        else
                        {
                            //Check if card hit is the same at the first card hit
                            if (hitCard.gameObject == FirstCardFlipped.gameObject)
                            {
                                hitCard.FlipCard();
                                FirstCardFlipped = null;
                            }
                            else
                            {
                                SecondCardFlipped = hitCard;
                                hitCard.FlipCard();
                                CompareCards();
                            }
                        }
                    }
                }
            }
        }
    }
    
    void CompareCards()
    {
        print("Comparing Cards");
        if(FirstCardFlipped.Name == SecondCardFlipped.Name)
        {
            print("Cards match");
            FirstCardFlipped.Matched = SecondCardFlipped.Matched = true;
            FirstCardFlipped = null;
            SecondCardFlipped = null;
        }
        else
        {
            print("Cards do not match");
            StartCoroutine("FlipCardsBackAfterFailedMatch");
        }
    }

    IEnumerator FlipCardsBackAfterFailedMatch()
    {
        print("Flip back coroutine started");
        AllowInput = false;
        yield return new WaitForSeconds(1.5f);
        FirstCardFlipped.FlipCard();
        SecondCardFlipped.FlipCard();
        //yield return new WaitForSeconds(1f);
        FirstCardFlipped = null;
        SecondCardFlipped = null; 
        //AllowInput = true;
        print("Flip back coroutine finished");
    }

    [Serializable]
    public class MemoCard
    {
        public string Name;
        public Texture2D Texture;
    }
}
