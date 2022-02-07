using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject winPanel;

    [SerializeField]
    private GameObject loosePanel;

    [SerializeField]
    private Text scoreTxt;

    [SerializeField]
    private Text timerTxt;

    [SerializeField]
    private List<Vector3> positions;

    [SerializeField]
    private GameObject[] cards;
    
    [SerializeField]
    private List<GameObject> correctCards;

    [SerializeField]
    private AudioClip[] sounds;
    
    private AudioSource audioSrc;
    
    [SerializeField]
    private List<GameObject> picked;
    
    private bool isPicking;
    
    private bool reveal;

    private bool isPlaying;

    private bool won;

    private float timer;

    Coroutine validateMove;

    int discoveredCards;
    
    // Start is called before the first frame update
    void Start()
    {
        winPanel.SetActive(false);
        loosePanel.SetActive(false);

        discoveredCards = 0;

        foreach(GameObject card in cards)
        {
            positions.Add(card.transform.position);
        }

        audioSrc = GetComponent<AudioSource>();

        ShuffleCards();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0.2f)
        {
            isPlaying = true;
        } else
        {
            isPlaying = false;
            loosePanel.SetActive(true);
            winPanel.SetActive(false);
        }

        if (isPlaying)
        {
            if (!won)
            {
                timer -= Time.deltaTime;
            }            
        }

        foreach (GameObject card in cards)
        {
            if (card.transform.rotation.x < 0)
            {
                card.GetComponent<BoxCollider>().enabled = false;
            } else
            {
                card.GetComponent<BoxCollider>().enabled = true;
            }               

        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isPicking && isPlaying)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Select stage    
                    if (hit.transform.name.Contains("carta"))
                    {
                        if (picked.Count == 0)
                        {
                            reveal = true;
                            picked.Add(hit.transform.gameObject);
                            RevealCard(picked[0]);
                        }
                        else
                        {
                            reveal = true;
                            picked.Add(hit.transform.gameObject);
                            RevealCard(picked[1]);
                            validateMove = StartCoroutine(ValidateTurn(picked[0], picked[1]));
                        }                        
                    }
                }
            }            
        }

        scoreTxt.text = discoveredCards.ToString();

        timerTxt.text = TimeSpan.FromSeconds(timer).ToString(@"m\:ss");

        if (discoveredCards == 6)
        {
            won = true;
            winPanel.SetActive(true);
            isPicking = false;
            isPlaying = false;
        }
    }

    public void ShuffleCards()
    {        

        isPlaying = true;
        isPicking = true;

        picked.Clear();
        correctCards.Clear();

        discoveredCards = 0;

        Hidecards();

        List<Vector3> startPos = new List<Vector3>();
        List<Vector3> endPos = new List<Vector3>();
        foreach (GameObject card in cards)
        {
            startPos.Add(card.transform.position);
            endPos.Add(card.transform.position);            
        }


        // shuffle endPos
        for (int i = 0; i < endPos.Count; i++)
        {
            Vector3 temp = endPos[i];
            int swapIndex = UnityEngine.Random.Range(i, endPos.Count);
            endPos[i] = endPos[swapIndex];
            endPos[swapIndex] = temp;
        }

        for (int i = 0; i < startPos.Count; i++)
        {
            cards[i].transform.DOMove(endPos[i], 2);
        }

        timer = 90f;
        won = false;
        winPanel.SetActive(false);
        loosePanel.SetActive(false);
    }

    IEnumerator ValidateTurn(GameObject card1, GameObject card2)
    {
        isPicking = false;
        yield return new WaitForSeconds(1f);
        if (card1.name == card2.name)
        {
            audioSrc.PlayOneShot(sounds[0]);
            correctCards.Add(card1);
            correctCards.Add(card2);
            discoveredCards++;
        }
        else
        {
            audioSrc.PlayOneShot(sounds[1]);
            yield return new WaitForSeconds(5f);
            Hidecards();
        }

        yield return new WaitForSeconds(2f);

        isPicking = true;

        picked.Clear();

        yield return null;
    }

    void Hidecards()
    {
        foreach(GameObject card in cards)
        {
            if (!correctCards.Contains(card))
            {
                if (card.transform.rotation.x < 0)
                {
                    card.transform.DORotate(new Vector3(90, 0, 0), 2);
                }
            }
            
        }
    }

    void RevealCard(GameObject card)
    {
        if (reveal)
        {
            reveal = false;
            if (card.transform.rotation.x > 0)
            {
                card.transform.DORotate(new Vector3(-90, 0, 0), 2);
            }
        }
        
    }

    void HideCard(GameObject card)
    {
        if (reveal)
        {
            reveal = false;
            if (card.transform.rotation.x < 0)
            {
                card.transform.DORotate(new Vector3(90, 0, 0), 2);
            }
        }

    }
}
