using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Cinemachine;
using Engine.Scripts;
using ScriptableObjects;

// ReSharper disable All

public class Manager : MonoBehaviour
{
    [Header("Unity Settings")]
    [Tooltip("The TMPro text that handles subtitles.")]
    [SerializeField]
    private TextMeshProUGUI subtitles;
    
    [Tooltip("The Cinemachine virtual camera that will follow the characters.")]
    [SerializeField]
    private CinemachineVirtualCamera vcam;

    [Tooltip("The screen that will be displayed right before the next conversation starts.")]
    [SerializeField]
    private GameObject momentsLaterScreen;

    [Tooltip("The location that the characters will be in.")]
    [SerializeField]
    private GameObject locationObj;

    [Header("Customizable Settings")]
    [Tooltip("The topics that the AI will generate dialogue about.")]
    [SerializeField]
    private List<string> topics = new List<string>();
    
    [Tooltip("The file that the AI can also generate dialogue from.")]
    [SerializeField]
    private string topcisFile;

    [Tooltip("The locations that the characters can be in.")] 
    [SerializeField]
    private Location[] locations;
    
    [Tooltip("The characters that will be in the scene.")]
    [SerializeField]
    private Character[] characters;
    
    [Tooltip("Items that the characters can equip.")]
    [SerializeField]
    private Item[] items;
    
    [Tooltip("The media that the AI-generated dialogue will be based on.")]
    [SerializeField]
    private string genre = "Rick and Morty";
    
    [Tooltip("The maximum number of times a character can speak in a row.")]
    [SerializeField]
    private int maxConsecutiveSpeech = 2;

    [Header("General AI Settings")]
    [Tooltip("The maximum number of tokens (words and punctuation combined) that the AI will generate.")]
    [SerializeField]
    private int maxTokens = 500;

    [Tooltip("The minimum number of sentences of a conversation that the AI will generate.")]
    [SerializeField]
    private int minimumDialogueLength = 5;

    [Tooltip("The maximum number of sentences of a conversation that the AI will generate.")]
    [SerializeField]
    private int maximumDialogueLength = 7;

    [Tooltip("Should the AI should generate PG content?")]
    [SerializeField]
    private bool PG;
    
    [Tooltip("Should the AI generate topics on it's own?")]
    [SerializeField]
    private bool infiniteTopics = false;
    

    [Serializable]
    [HideInInspector]
    public struct Character
    {
        public string name;
        public Transform character;

        public override string ToString()
        {
            return name; // Return the name of the character for display purposes
        }
    }

    [Serializable]
    private struct Item
    {
        public string name;
        public string keyword;
        public GameObject itemObj;
    }

    private bool _isDoingDialogue = false;

    [HideInInspector]
    public static List<Vector3> originalPositions = new List<Vector3>();

    private void Start()
    {
        if (topcisFile != "")
        {
            if (File.Exists(topcisFile))
            {
                string[] topicsFromFile = File.ReadAllLines(topcisFile);
                topics.AddRange(topicsFromFile);
                File.WriteAllText(topcisFile, "");
            }
        }
        
        if (momentsLaterScreen.GetComponent<AISplash>())
        {
            momentsLaterScreen.GetComponent<AISplash>().GenerateSplash(genre);
        }

        foreach (Character character in characters)
        {
            originalPositions.Add(character.character.position);
        }

        Generate();
    }

    private async void Generate()
    {
        if(characters.Length == 0)
        {
            Debug.LogError("You must have characters, add them in the unity inspector");
            return;
        }

        string[] characterNames = new string[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            characterNames[i] = characters[i].ToString();
        }

        string charactersString = string.Join(", ", characterNames);
        if (topics.Count != 0 || infiniteTopics)
        {
            if (infiniteTopics)
            {
                var openai = new OpenAIApi();
                var response = await openai.CreateCompletion(new CreateCompletionRequest()
                {
                    Prompt = "Generate a random topic someone would talk about " + (!PG ? " (you can be offensive):\n" : ":\n"), // Prompt the AI to generate a topic
                    Model = "text-davinci-003",
                    MaxTokens = maxTokens
                });
                topics.Add(response.Choices[0].Text);
            }
            Location location = locations[UnityEngine.Random.Range(0, locations.Length)];
            GenerateDialogue(
                $"You are an AI that generates {genre.Trim()} content. Your current topic is {topics[0].Trim()}\nGenerate a dialogue between the listed characters here. Characters: {charactersString.Trim()}\nFormat the conversation as (character): (thing);(character they are talking to)\n(another character): (thing);(character they are talking to)\nYou should generate a minimum of " +
                minimumDialogueLength.ToString() + " pieces of dialogue and a maximum of " +
                maximumDialogueLength.ToString() + " pieces of dialogue.\nYou may also make a character speak " + 
                maxConsecutiveSpeech.ToString() + " times in a row, but not " + (maxConsecutiveSpeech - 1).ToString() + " times in a row. The characters location is " + location.name + ". " + (!PG ? "You can be offensive." : "\n"), location);
            topics.RemoveAt(0);
        }
        else
        {
            Debug.LogError("Topics list ran out, add more topics in the unity inspector, or enable the infinite topics bool");
        }
    }

    private async void GenerateDialogue(string prompt, Location location)
    {
        var openai = new OpenAIApi();
        var response = await openai.CreateCompletion(new CreateCompletionRequest()
        {
            Prompt = prompt,
            Model = "text-davinci-003",
            MaxTokens = maxTokens
        });
        StartCoroutine(GoThroughDialogue(response.Choices[0].Text, location));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    
    private IEnumerator GoThroughDialogue(string dialogueText, Location location)
    {
        //wait until the last dialogue is done using waituntil
        yield return new WaitUntil(() => !_isDoingDialogue);
        momentsLaterScreen.SetActive(true);
        LocationManager.GetBackgroundInfo(location, locationObj, characters, vcam.transform);
        if(momentsLaterScreen.GetComponent<AISplash>())
        {
            yield return new WaitForSeconds(momentsLaterScreen.GetComponent<AISplash>().splash.text.Length / 10f);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }
        //read topic file and add new topics to topics list, then clear the file
        if (topcisFile != "")
        {
            if (File.Exists(topcisFile))
            {
                string[] topicsFromFile = File.ReadAllLines(topcisFile);
                topics.AddRange(topicsFromFile);
                File.WriteAllText(topcisFile, "");
            }
        }
        momentsLaterScreen.SetActive(false);
        if (momentsLaterScreen.GetComponent<AISplash>())
        {
            momentsLaterScreen.GetComponent<AISplash>().GenerateSplash(genre);
        }
        _isDoingDialogue = true;
        Generate();
        Debug.Log("Topic change.");
        List<string> dialogue = dialogueText.Split('\n').ToList();
        foreach (string text in dialogue)
        {
            string formattedText = "";
            bool error = false;
            try
            {
                string characterName = text.Split(":")[0].Trim();
                Character character = characters.FirstOrDefault(c => c.name == characterName);

                string subjectName = text.Split(";").Last();
                formattedText = text.Replace(";" + subjectName, "").Replace("\"", "").Trim();
                Debug.Log(subjectName);
                if (character.character != null)
                {
                    vcam.m_Follow = character.character;
                    vcam.m_LookAt = character.character;
                    Character subject = characters.FirstOrDefault(c => c.name == subjectName);
                    if (subject.character != null)
                    {
                        character.character.gameObject.GetComponent<global::Engine.Scripts.Character>().target =
                            subject.character;
                    }

                    //loop through items
                    foreach (Item item in items)
                    {
                        if (formattedText.ToLower().Contains(item.keyword.ToLower()))
                        {
                            character.character.GetComponent<global::Engine.Scripts.Character>()
                                .EquipItem(item.itemObj);
                            break;
                        }
                    }

                    subtitles.text = formattedText;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing dialogue: " + e);
                error = true;
            }

            if (!error)
            {
                yield return new WaitForSeconds(formattedText.Length / 10f);
            }
        }
        _isDoingDialogue = false;
    }
    
    //on unity stopped, stop the coroutine
    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}
