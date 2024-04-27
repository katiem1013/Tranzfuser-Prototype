using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCharacterSelection : MonoBehaviour
{
    public string firstSelectedCharacter, secondSelectedCharacter;

    [Header("References")]
    public GameObject playerHolder;
    Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerHolder.transform.childCount; i++)
        {
            if (playerHolder.transform.GetChild(i).name == firstSelectedCharacter && playerHolder.transform.GetChild(i).gameObject.activeSelf == false)
            {
                playerHolder.transform.GetChild(i).gameObject.SetActive(true);
            }

            else if (playerHolder.transform.GetChild(i).name == firstSelectedCharacter && playerHolder.transform.GetChild(i).gameObject.activeSelf == true)
            {
                playerHolder.transform.GetChild(i).gameObject.SetActive(true);
            }

                else
                playerHolder.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // sets the selected characters to active
            for (int i = 0; i < playerHolder.transform.childCount; i++)
            {
                if ((playerHolder.transform.GetChild(i).name == firstSelectedCharacter && playerHolder.transform.GetChild(i).gameObject.activeSelf == false) || (playerHolder.transform.GetChild(i).name == secondSelectedCharacter && playerHolder.transform.GetChild(i).gameObject.activeSelf == false))
                {
                    playerHolder.transform.GetChild(i).gameObject.SetActive(true);
                }

                else
                    playerHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        // keeps all the characters in the same position at all times
        for (int i = 0; i < playerHolder.transform.childCount; i++)
        {
            if ((playerHolder.transform.GetChild(i).name == firstSelectedCharacter && playerHolder.transform.GetChild(i).gameObject.activeSelf == true) || (playerHolder.transform.GetChild(i).name == secondSelectedCharacter && playerHolder.transform.GetChild(i).gameObject.activeSelf == true))
            {
                playerPos = playerHolder.transform.GetChild(i).transform.position;
            }

            else
                playerHolder.transform.GetChild (i).gameObject.transform.position = playerPos;
        }
    }
}
