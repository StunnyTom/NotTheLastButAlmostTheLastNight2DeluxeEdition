using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject titlePanel;   // Le panneau "Appuyer sur une touche"
    public GameObject menuPanel;    // Le panneau avec les boutons Host/Join/Settings
    public GameObject settingsPanel; // Le panneau des paramètres
    public GameObject hostPanel;     // Le panneau pour héberger
    public GameObject joinPanel;     // Le panneau pour rejoindre

    private bool isAtTitle = true;

    private string GetPath(GameObject obj)
    {
        if (!obj) return "NULL";
        string path = "/" + obj.name;
        Transform t = obj.transform.parent;
        while (t != null)
        {
            path = "/" + t.name + path;
            t = t.parent;
        }
        return path;
    }

    void Start()
    {
        Debug.Log("--- MainMenuController START ---");
        
        // AUTO-FIND PANELS
        if (!titlePanel || !menuPanel || !settingsPanel) 
        {
             Debug.LogWarning("Some Panels are missing references. Running AutoFindPanels...");
             AutoFindPanels();
        }

        // LOG ASSIGNMENTS WITH PATHS
        Debug.Log($"[Status] TitlePanel: {GetPath(titlePanel)}");
        Debug.Log($"[Status] MenuPanel: {GetPath(menuPanel)}");
        Debug.Log($"[Status] SettingsPanel: {GetPath(settingsPanel)}");
        Debug.Log($"[Status] HostPanel (Play): {GetPath(hostPanel)}");
        Debug.Log($"[Status] JoinPanel: {GetPath(joinPanel)}");

        // Debug Audio Listener Warning
        var listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (listeners.Length > 1)
        {
             for(int i=1; i<listeners.Length; i++) Destroy(listeners[i]);
        }

        // CHECK EVENTSYSTEM
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            var esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.LogError("!!! MISSING EVENTSYSTEM DETECTED !!! Created a backup EventSystem.");
        }
        else
        {
            Debug.Log($"[Check] EventSystem found: {UnityEngine.EventSystems.EventSystem.current.name}");
        }

        // CHECK RAYCASTER
        var raycaster = GetComponentInParent<GraphicRaycaster>();
        if (!raycaster) 
        {
             var canvas = GetComponentInParent<Canvas>();
             if (canvas) 
             {
                 Debug.LogWarning("Missing GraphicRaycaster on Canvas! Adding one.");
                 canvas.gameObject.AddComponent<GraphicRaycaster>();
             }
        }

        // KILL LEGACY CONNECTORS
        var connectors = FindObjectsByType<MainMenuConnector>(FindObjectsSortMode.None);
        foreach(var c in connectors) 
        {
            Debug.LogWarning($"[Cleanup] Destroying legacy 'MainMenuConnector' on {c.name}");
            Destroy(c); 
        }

        // DESTROY SLIMUI MANAGER (Conflict Killer)
        // DESTROY SLIMUI MANAGER (Conflict Killer)
        // var slimUI = FindFirstObjectByType<SlimUI.ModernMenu.UIMenuManager>();
        // if (slimUI)
        // {
        //      Debug.LogWarning($"[Conflict] DESTROYING 'UIMenuManager' on {slimUI.name} to stop persistent listener crashes.");
        //      Destroy(slimUI);
        // }

        // AUTO-WIRE BUTTONS (Runtime Fix)
        AutoWireButtons();

        // Au démarrage, on affiche directement le menu (plus de "Press Any Key")
        ShowMenu();
    }

    private void AutoFindPanels()
    {
        Debug.Log("--- Auto-Finding Panels (Strict Mode) ---");
        
        // 1. Title (Fallback)
        if (!titlePanel) titlePanel = FindInScene("TitlePanel", "Title", "StartScreen");

        // 2. ROOT MENU -> "MAIN"
        if (!menuPanel) menuPanel = FindInSceneStrict("MAIN", "MenuPanel");

        // 3. HOST/PLAY MENU -> "PLAY"
        // This contains the Multi/Solo buttons
        if (!hostPanel) hostPanel = FindInSceneStrict("PLAY", "HostPanel");

        // 4. SETTINGS -> "CustomSettingsMenu" (Priority) or "OPTIONS"
        if (!settingsPanel) settingsPanel = FindInSceneStrict("CustomSettingsMenu", "Options", "Settings", "SettingsPanel");

        if (!joinPanel) joinPanel = FindInScene("JoinPanel", "JoinSession");
    }

    private GameObject FindInScene(params string[] names)
    {
        // 1. Direct Search
        foreach(var n in names)
        {
            var obj = GameObject.Find(n);
            if (IsValidPanel(obj)) return obj;
        }

        // 2. Recursive Search (Contains)
        var canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach(var c in canvases)
        {
            foreach(Transform t in c.GetComponentsInChildren<Transform>(true))
            {
                foreach(var n in names)
                {
                     if (t.name.Contains(n))
                     {
                         if (IsValidPanel(t.gameObject)) return t.gameObject;
                     }
                }
            }
        }
        return null;
    }

    private GameObject FindInSceneStrict(params string[] names)
    {
        // PRIORITIZE NAMES: Search for Name[0] everywhere, then Name[1], etc.
        foreach(var n in names)
        {
            // 1. Direct Search (For Active objects)
            var obj = GameObject.Find(n);
            if (IsValidPanel(obj)) return obj;

            // 2. Recursive Search (For Inactive objects)
            // This is heavy but necessary if the panels are hidden at start
            var canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach(var c in canvases)
            {
                foreach(Transform t in c.GetComponentsInChildren<Transform>(true))
                {
                     if (t.name.Equals(n, System.StringComparison.OrdinalIgnoreCase))
                     {
                         if (IsValidPanel(t.gameObject)) return t.gameObject;
                     }
                }
            }
        }
        return null;
    }

    private bool IsValidPanel(GameObject obj)
    {
        if (obj == null) return false;
        if (obj.name.ToLower().Contains("btn_")) return false; // Ignore Buttons
        if (obj.GetComponent<Button>() != null) return false; // Ignore Objects with Button component (unless they are also panels? unlikely)
        return true;
    }

    private void AutoWireButtons()
    {
        Debug.Log("--- Auto-Wiring Buttons (Deep Debug) ---");
        
        // Harvest buttons from ALL known panels, even if inactive
        System.Collections.Generic.List<Button> allButtons = new System.Collections.Generic.List<Button>();
        
        if (titlePanel) allButtons.AddRange(titlePanel.GetComponentsInChildren<Button>(true));
        if (menuPanel) allButtons.AddRange(menuPanel.GetComponentsInChildren<Button>(true));
        if (settingsPanel) allButtons.AddRange(settingsPanel.GetComponentsInChildren<Button>(true));
        if (hostPanel) allButtons.AddRange(hostPanel.GetComponentsInChildren<Button>(true));
        if (joinPanel) allButtons.AddRange(joinPanel.GetComponentsInChildren<Button>(true));

        // Add self children just in case
        allButtons.AddRange(GetComponentsInChildren<Button>(true));

        // Remove duplicates
        System.Collections.Generic.HashSet<Button> uniqueButtons = new System.Collections.Generic.HashSet<Button>(allButtons);

        Debug.Log($"Found {uniqueButtons.Count} unique buttons to inspect.");

        foreach(var btn in uniqueButtons)
        {
            var txt = btn.GetComponentInChildren<TMPro.TMP_Text>()?.text.ToLower() ?? "";
            var n = btn.name.ToLower();
            string path = GetPath(btn.gameObject);

            // Clear previous runtime listeners
            btn.onClick.RemoveAllListeners();

            string action = "NONE";

            // Multi / Host
            if (txt.Contains("multi") || n.Contains("multi") || n.Contains("lobby"))
            {
                action = "OnClickMulti";
                btn.onClick.AddListener(OnClickMulti);
            }
            // Solo / Join
            else if (txt.Contains("solo") || n.Contains("solo"))
            {
                action = "OnClickSolo";
                btn.onClick.AddListener(OnClickSolo);
            }
            // Custom
            else if (txt.Contains("custom") || n.Contains("custom"))
            {
                action = "OnClickCustom";
                btn.onClick.AddListener(OnClickCustom);
            }
            // Settings / Options
            else if (txt.Contains("option") || n.Contains("option") || txt.Contains("setting") || n.Contains("setting"))
            {
                action = "ShowSettings";
                btn.onClick.AddListener(ShowSettings);
            }
            // Quit / Exit
            else if (txt.Contains("quit") || n.Contains("quit") || txt.Contains("exit") || n.Contains("exit"))
            {
                action = "OnQuitClicked";
                btn.onClick.AddListener(OnQuitClicked);
            }
            // PLAY (Root JOUER -> Opens HostPanel)
            else if (txt.Contains("jouer") || txt.Contains("play") || n.Contains("btn_play"))
            {
                action = "OnHostClicked (Open Play Menu)";
                btn.onClick.AddListener(OnHostClicked);
            }
            // BACK (From Settings/Play -> Menu)
            else if (txt.Contains("retour") || txt.Contains("back") || n.Contains("back") || n.Contains("return"))
            {
                action = "BackToMenu";
                btn.onClick.AddListener(BackToMenu);
            }

            Debug.Log($"[Wiring] Button '{btn.name}' @ {path} (Text: '{txt}') -> {action}");
        }

        // --- NUCLEAR OPTION FOR BACK BUTTON ---
        // Explicitly find the object named "BackButton" if it wasn't wired above
        var specificBack = GameObject.Find("BackButton");
        if (specificBack)
        {
            var btn = specificBack.GetComponent<Button>();
            if (btn)
            {
                Debug.LogWarning($"[NUCLEAR] Found specific 'BackButton' at {GetPath(specificBack)}. FORCING wiring to BackToMenu.");
                
                // Force Interactable
                btn.interactable = true;
                
                // Force Image Raycast
                var img = btn.GetComponent<Image>();
                if (img) 
                {
                    img.raycastTarget = true;
                    Debug.Log($"[Fix] BackButton Image raycastTarget force set to TRUE.");
                }

                // Force Text Raycast (optional, but good for clicks on text)
                var txtComp = btn.GetComponentInChildren<TMPro.TMP_Text>();
                if (txtComp) txtComp.raycastTarget = true;

                // Debug Rect
                var rect = btn.GetComponent<RectTransform>();
                Debug.Log($"[Debug] BackButton Rect: Pos={rect.position}, Size={rect.rect.size}, Scale={rect.localScale}");

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(BackToMenu);
            }
        }
        else
        {
             // Try getting it from the CustomSettingsMenu if we have it
             if (settingsPanel)
             {
                 var btn = settingsPanel.transform.Find("BackButton")?.GetComponent<Button>();
                 if (btn)
                 {
                    Debug.LogWarning($"[NUCLEAR] Found 'BackButton' inside SettingsPanel. FORCING wiring.");
                    
                    // Force Integrity
                    btn.interactable = true;
                    var img = btn.GetComponent<Image>();
                    if (img) img.raycastTarget = true;
                    var txtComp = btn.GetComponentInChildren<TMPro.TMP_Text>();
                    if (txtComp) txtComp.raycastTarget = true;

                    // Debug Rect (Added for diagnostics)
                    var rect = btn.GetComponent<RectTransform>();
                    Debug.Log($"[Debug] BackButton (in Panel) Rect: Pos={rect.position}, Size={rect.rect.size}, Scale={rect.localScale}");

                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(BackToMenu);
                 }
             }
        }
    }

    void Update()
    {
        // Si on est sur l'écran titre et qu'on appuie sur une touche
        if (isAtTitle && Input.anyKeyDown)
        {
            Debug.Log("[Input] Key pressed at Title -> ShowMenu()");
            ShowMenu();
        }

        // DEBUG CLIC SOURIS
        if (Input.GetMouseButtonDown(0))
        {
            var pointerData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerData.position = Input.mousePosition;
            var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                Debug.Log($"[Click Debug] Hit {results.Count} UI Objects:");
                foreach(var result in results)
                {
                    Debug.Log($"   -> {result.gameObject.name} (Depth: {result.depth}, SortingLayer: {result.sortingLayer})");
                }
            }
            else
            {
                Debug.Log("[Click Debug] Clicked on NOTHING. Is there a GraphicRaycaster missing?");
            }
        }
    }

    private void FixCanvasScaler(GameObject panel)
    {
        if (!panel) return;
        var scaler = panel.GetComponentInParent<CanvasScaler>();
        if (scaler)
        {
            if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                Debug.LogWarning($"[Fix] Updating CanvasScaler on {scaler.name} to ScaleWithScreenSize (1920x1080)");
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }
        }
    }

    // Affiche l'écran titre
    public void ShowTitle()
    {
        isAtTitle = true;
        if (titlePanel) titlePanel.SetActive(true);
        if (menuPanel) menuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (hostPanel) hostPanel.SetActive(false);
        if (joinPanel) joinPanel.SetActive(false);
    }



    // Affiche les paramètres
    public void ShowSettings()
    {
        Debug.Log(">>> ShowSettings() CALLED <<<");
        if (menuPanel) menuPanel.SetActive(false);
        if (settingsPanel) 
        {
            settingsPanel.SetActive(true);
            EnsureParentsActive(settingsPanel); // Force parents to be active
            // FixCanvasScaler(settingsPanel);     // DISABLED: Force resolution fix
            
            bool isActive = settingsPanel.activeInHierarchy;
            Debug.Log($"SettingsPanel ActiveSelf: {settingsPanel.activeSelf} | ActiveInHierarchy: {isActive}");
            
            if (!isActive)
            {
                Debug.LogError("!!! SettingsPanel is STILL HIDDEN. Dumping Hierarchy Status: !!!");
                // ... (existing hierarchy dump)
            }
            if (!isActive)
            {
                Debug.LogError("!!! SettingsPanel is STILL HIDDEN. Dumping Hierarchy Status: !!!");
                // ... (existing hierarchy dump)
            }
            else
            {
                // DEBUG: Inspect the panel itself
                var img = settingsPanel.GetComponent<Image>();
                if (img) 
                {
                    // FIX: Disable raycast on the background panel so it doesn't eat clicks intended for buttons
                    // AND disable Maskable as found by user
                    img.raycastTarget = false; 
                    img.maskable = false;
                    Debug.Log("[Fix] CustomSettingsMenu: raycastTarget=FALSE, maskable=FALSE");
                }

                // FIX: FIND AND FIX BACK BUTTON
                var btnBack = settingsPanel.transform.Find("BackButton");
                if (btnBack)
                {
                    btnBack.gameObject.SetActive(true);
                    btnBack.SetAsLastSibling(); // Render ON TOP
                    
                    // Restore Z-Force if needed, but for now just trust hierarchy
                    var rect = btnBack.GetComponent<RectTransform>();
                    var pos = rect.localPosition;
                    rect.localPosition = new Vector3(pos.x, pos.y, 0f); 

                    // FIX: Re-attach listener explicitly as requested
                    var btn = btnBack.GetComponent<Button>();
                    if (btn)
                    {
                        // FORCE MASKABLE OFF (User Request)
                        var bImg = btn.GetComponent<Image>();
                        if (bImg) bImg.maskable = false;
                        
                        var bTxt = btn.GetComponentInChildren<TMPro.TMP_Text>();
                        if (bTxt) bTxt.maskable = false;

                        btn.onClick.RemoveAllListeners();
                        // Use a Lambda to prove it works even if invisible in Inspector
                        btn.onClick.AddListener(() => { 
                            Debug.Log(">>> RUNTIME LISTENER FIRED: BackToMenu <<<");
                            BackToMenu(); 
                        });
                        Debug.Log("[Fix] BackButton Listener re-attached (Runtime Lambda). Maskable=FALSE.");
                    }
                }
            }
        }
        else Debug.LogError("SettingsPanel is NULL!");
        
        if (hostPanel) hostPanel.SetActive(false);
        if (joinPanel) joinPanel.SetActive(false);
    }

    private void EnsureParentsActive(GameObject target)
    {
        Transform t = target.transform.parent;
        while(t != null)
        {
            if (t.GetComponent<Canvas>() != null) break; // Stop at Canvas
            if (!t.gameObject.activeSelf)
            {
                Debug.LogWarning($"[Fix] Enabling parent '{t.name}' of '{target.name}' because it was disabled.");
                t.gameObject.SetActive(true);
            }
            t = t.parent;
        }
    }

    // Retour au menu depuis les paramètres
    public void BackToMenu()
    {
        Debug.Log(">>> BackToMenu() CLICKED <<<");
        ShowMenu();
    } 

    // Affiche le menu principal (Host/Join/Settings)
    public void ShowMenu()
    {
        Debug.Log(">>> ShowMenu() CALLED <<<");
        isAtTitle = false;
        if (titlePanel) titlePanel.SetActive(false);
        if (menuPanel) 
        {
            menuPanel.SetActive(true);
            EnsureParentsActive(menuPanel);
        }
        if (settingsPanel) settingsPanel.SetActive(false);
        if (hostPanel) hostPanel.SetActive(false);
        if (joinPanel) joinPanel.SetActive(false);
    }

    // Méthode pour le bouton "Héberger" (Host) -> DEVENU LE BOUTON PLAY
    public void OnHostClicked()
    {
        // Don't hide the main menu if the user wants it to stay!
        // if(menuPanel != null) menuPanel.SetActive(false); 
        
        if(hostPanel != null) hostPanel.SetActive(true);
    }

    // Méthode pour le bouton "Rejoindre" (Join)
    public void OnJoinClicked()
    {
        if(menuPanel != null) menuPanel.SetActive(false);
        if(joinPanel != null) joinPanel.SetActive(true);
    }

    // Méthode pour le bouton "Quitter"
    public void OnQuitClicked()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();
    }

    // Méthode pour le bouton "Play" (Lance le jeu directement)
    public void OnPlayClicked()
    {
        // Assurez-vous que la scène est ajoutée dans File > Build Settings
        SceneManager.LoadScene("The_Viking_Village");
    }
    // --- NEW METHODS FOR UPDATED MENU ---

    public void OnClickMulti()
    {
        Debug.LogError(">>> BUTTON CLICKED: OnClickMulti() <<<");
        
        // Debug Build Settings
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        Debug.Log($"Scenes in Build: {sceneCount}");
        for(int i=0; i<sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            Debug.Log($"Index {i}: {name} (Path: {path})");
        }

        Debug.Log("Attempting to load scene 'LobbyMenu'...");
        try 
        {
            SceneManager.LoadScene("LobbyMenu");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"FAILED to load 'LobbyMenu': {e.Message}");
            // Panic Fallback: Load index 1 (usually the next scene)
            if (sceneCount > 1) 
            {
                Debug.LogWarning("Fallback: Loading Scene Index 1");
                SceneManager.LoadScene(1);
            }
        }
    }

    public void OnClickSolo()
    {
        Debug.Log("Solo Mode Coming Soon");
        // Optional: Show a "Coming Soon" popup
    }

    public void OnClickCustom()
    {
        Debug.Log("Custom Mode Coming Soon");
    }
}
