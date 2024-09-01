using UnityEngine.SceneManagement;
using UnityEngine;
public class SettingsMenu : MonoBehaviour
{
    public void GoToAudioSettings()
    {
        SceneManager.LoadScene("AudioSettings");
    }
    public void GoToGraphicsSettings()
    {
        SceneManager.LoadScene("GraphicsSettings");
    }
    public void GoToControlerSettings()
    {
        SceneManager.LoadScene("ControlerSettings");
    }
}
