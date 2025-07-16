// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using UnityEngine.SceneManagement;
// using System.Collections.Generic;

// public class ProfileManager : MonoBehaviour
// {
//     [Header("Panels")]
//     public GameObject panelAuth;
//     public GameObject panelWelcome;

//     [Header("Auth UI")]
//     public TMP_InputField inputUsername;
// public TMP_Dropdown dropdownUsers;

//     public Button createButton;
//     public Button connectButton;

//     [Header("Welcome UI")]
//     public TextMeshProUGUI welcomeText;
//     public Button startButton;
//     public Button logoutButton;

//     private string currentUser = "";
//     private List<string> allUsers = new List<string>();

//   void Start()
// {
//     LoadAllUsers();

//     createButton.onClick.AddListener(CreateProfile);
//     connectButton.onClick.AddListener(ConnectProfile);
//     startButton.onClick.AddListener(LaunchApp);
//     logoutButton.onClick.AddListener(Logout);

//     UpdateDropdown();

//     currentUser = PlayerPrefs.GetString("current_user", "");
//     if (!string.IsNullOrEmpty(currentUser))
//         ShowWelcomePanel(currentUser);
//     else
//         ShowAuthPanel();
// }

// void UpdateDropdown()
// {
//     dropdownUsers.ClearOptions();

//     List<string> options = new List<string>();
//     if (allUsers.Count > 0)
//     {
//         options.Add("– Choisir un profil –");
//         options.AddRange(allUsers);
//         dropdownUsers.AddOptions(options);
//         dropdownUsers.value = 0;
//     }
// }



//     void CreateProfile()
//     {
//         string username = inputUsername.text.Trim();
//         if (!string.IsNullOrEmpty(username))
//         {
//             if (!allUsers.Contains(username))
//             {
//                 allUsers.Add(username);
//                 SaveAllUsers();
//             }

//             PlayerPrefs.SetString("current_user", username);
//             PlayerPrefs.Save();
//             ShowWelcomePanel(username);
//         }
//     }

// void ConnectProfile()
// {
//     if (dropdownUsers.options.Count == 0 || dropdownUsers.value < 0)
//     {
//         Debug.LogWarning("Aucun profil sélectionné !");
//         return;
//     }

//     string username = dropdownUsers.options[dropdownUsers.value].text;
//     if (allUsers.Contains(username))
//     {
//         PlayerPrefs.SetString("current_user", username);
//         PlayerPrefs.Save();
//         ShowWelcomePanel(username);
//     }
// }


//     void ShowWelcomePanel(string username)
//     {
//         currentUser = username;
//         welcomeText.text = $"Bienvenue, {username} 👋";
//         panelAuth.SetActive(false);
//         panelWelcome.SetActive(true);
//     }

//     void ShowAuthPanel()
//     {
//         panelAuth.SetActive(true);
//         panelWelcome.SetActive(false);
//     }

//     void LaunchApp()
//     {
//         if (!string.IsNullOrEmpty(currentUser))
//         {
//             SceneManager.LoadScene("Commencer");
//         }
//     }

//     void Logout()
//     {
//         PlayerPrefs.DeleteKey("current_user");
//         currentUser = "";
//         ShowAuthPanel();
//     }

//  void LoadAllUsers()
// {
//     if (System.IO.File.Exists(userFilePath))
//     {
//         string json = System.IO.File.ReadAllText(userFilePath);
//         userList = JsonUtility.FromJson<UserList>(json);
//         allUsers = userList.users;
//     }
//     else
//     {
//         userList = new UserList();
//         allUsers = new List<string>();
//     }
// }

// void SaveAllUsers()
// {
//     userList.users = allUsers;
//     string json = JsonUtility.ToJson(userList, true);
//     System.IO.File.WriteAllText(userFilePath, json);
// }
//    [System.Serializable]
//     public class UserList
//     {
//         public List<string> users = new List<string>();
//     }

//     private string userFilePath => $"{Application.persistentDataPath}/users.json";

//     private UserList userList = new UserList();
// }


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class ProfileManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelAuth;
    public GameObject panelWelcome;

    [Header("Auth UI")]
    public TMP_InputField inputUsername;
    public TMP_Dropdown dropdownUsers;
    public Button createButton;
    public Button connectButton;

    [Header("Welcome UI")]
    public TextMeshProUGUI welcomeText;
    public Button startButton;
    public Button logoutButton;

    private string currentUser = "";
    private List<string> allUsers = new List<string>();
    private UserList userList = new UserList();

    private string userFilePath => Path.Combine(Application.persistentDataPath, "users.json");

    void Start()
    {
        LoadAllUsers();

        createButton.onClick.AddListener(CreateProfile);
        connectButton.onClick.AddListener(ConnectProfile);
        startButton.onClick.AddListener(LaunchApp);
        logoutButton.onClick.AddListener(Logout);

        UpdateDropdown();

        currentUser = PlayerPrefs.GetString("current_user", "");
        if (!string.IsNullOrEmpty(currentUser))
            ShowWelcomePanel(currentUser);
        else
            ShowAuthPanel();
    }

    void UpdateDropdown()
    {
        dropdownUsers.ClearOptions();

        if (allUsers.Count > 0)
        {
            List<string> options = new List<string> { "-- Choisir un profil --" };
            options.AddRange(allUsers);
            dropdownUsers.AddOptions(options);
            dropdownUsers.value = 0;
        }
    }

    void CreateProfile()
    {
        string username = inputUsername.text.Trim();
        if (!string.IsNullOrEmpty(username))
        {
            if (!allUsers.Contains(username))
            {
                allUsers.Add(username);
                SaveAllUsers();
            }

            PlayerPrefs.SetString("current_user", username);
            PlayerPrefs.Save();
            ShowWelcomePanel(username);
        }
    }

    void ConnectProfile()
    {
        if (dropdownUsers.options.Count <= 1 || dropdownUsers.value == 0)
        {
            Debug.LogWarning("Veuillez sélectionner un profil valide.");
            return;
        }

        string username = dropdownUsers.options[dropdownUsers.value].text;
        if (allUsers.Contains(username))
        {
            PlayerPrefs.SetString("current_user", username);
            PlayerPrefs.Save();
            ShowWelcomePanel(username);
        }
    }

    void ShowWelcomePanel(string username)
    {
        currentUser = username;
        welcomeText.text = $"Bienvenue, {username} 👋";
        panelAuth.SetActive(false);
        panelWelcome.SetActive(true);
    }

    void ShowAuthPanel()
    {
        panelAuth.SetActive(true);
        panelWelcome.SetActive(false);
    }

    void LaunchApp()
    {
        if (!string.IsNullOrEmpty(currentUser))
        {
            SceneManager.LoadScene("Commencer");
        }
    }

    void Logout()
    {
        PlayerPrefs.DeleteKey("current_user");
        currentUser = "";
        ShowAuthPanel();
    }

    void LoadAllUsers()
    {
        if (File.Exists(userFilePath))
        {
            string json = File.ReadAllText(userFilePath);
            userList = JsonUtility.FromJson<UserList>(json);
            allUsers = userList.users;
        }
        else
        {
            userList = new UserList();
            allUsers = new List<string>();
        }
    }

    void SaveAllUsers()
    {
        userList.users = allUsers;
        string json = JsonUtility.ToJson(userList, true);
        File.WriteAllText(userFilePath, json);
    }

    [System.Serializable]
    public class UserList
    {
        public List<string> users = new List<string>();
    }
}
