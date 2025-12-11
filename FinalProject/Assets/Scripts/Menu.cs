using UnityEngine;

public class Menu : MonoBehaviour
{
    public void GoToTavern()
    {
        SceneLoader.LoadScene("Tavern");
    }

    public void GoToCredits()
    {
        SceneLoader.LoadScene("Credits");
    }
    
    public void GoToOpen()
    {
        SceneLoader.LoadScene("OpenScene");
    }
}