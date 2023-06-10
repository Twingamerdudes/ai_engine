using UnityEngine;
using OpenAI;
using TMPro;
using UnityEngine.Serialization;

public class AISplash : MonoBehaviour
{
    [Tooltip("Splash text")]
    public TextMeshProUGUI splash;

    [FormerlySerializedAs("PG")]
    [Tooltip("Should the AI generate PG content?")]
    [SerializeField]
    private bool pg;

    public async void GenerateSplash(string genre)
    {
        var openai = new OpenAIApi();
        var response = await openai.CreateCompletion(new CreateCompletionRequest()
        {
            Prompt = genre + " needs a hilarious in-between text (like in SpongeBob) after a wacky event! Make sure it can fit with any event. " + (!pg ? "Make it offensive. " : "Make it kid-friendly. ") + "Generate a funny splash text:",
            Model = "text-davinci-003",
            MaxTokens = 50
        });
        splash.text = response.Choices[0].Text.Replace("\"", "");
    }
}
