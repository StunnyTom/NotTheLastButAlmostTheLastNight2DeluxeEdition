using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using UnityEditor.Events;
using TMPro;

public class NetworkSetup : EditorWindow
{
    [MenuItem("Tools/Menu Setup (Essential)/8. Setup NetworkManager & Wiring")]
    public static void SetupNetwork()
    {
        Debug.Log("--- Setting up NetworkManager ---");

        // 1. Find or Create NetworkManager
        GameObject netManagerObj = GameObject.Find("NetworkManager");
        NetworkManager netManager = null;

        if (netManagerObj == null)
        {
            netManagerObj = new GameObject("NetworkManager");
            Undo.RegisterCreatedObjectUndo(netManagerObj, "Create NetworkManager");
        }
        else
        {
            netManager = netManagerObj.GetComponent<NetworkManager>();
        }

        if (netManager == null)
        {
            netManager = netManagerObj.AddComponent<NetworkManager>();
            // Add Transport
            var transport = netManagerObj.GetComponent<UnityTransport>();
            if (transport == null) transport = netManagerObj.AddComponent<UnityTransport>();
            
            // Assign Transport to Field (Reflection might be needed if field is private, but usually it's auto-found or public)
            netManager.NetworkConfig = new NetworkConfig();
            // Actually, in Editor, NetworkManager usually auto-detects transport on Awake, 
            // but we can ensure components are there.
        }

        // Configure basic settings so it doesn't complain
        // (NGO 1.0+ handles a lot of defaults)
        
        Debug.Log("NetworkManager is present.");

        // 2. Wire Buttons
        // We need a helper script for the buttons to call because StartHost() is on the Singleton which is static logic,
        // but UnityEvents need a target object. The NetworkManager object IS the target.
        // We will wire Button -> NetworkManager.StartHost()

        // Find buttons (created by Step 7)
        var uiManager = Object.FindFirstObjectByType<SlimUI.ModernMenu.UIMenuManager>();
        if (uiManager == null || uiManager.playMenu == null) return;

        Button hostBtn = null;
        Button joinBtn = null;

        Button[] buttons = uiManager.playMenu.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            if (btn.name.Contains("Btn_Host")) hostBtn = btn;
            if (btn.name.Contains("Btn_Join")) joinBtn = btn;
        }

        if (hostBtn != null)
        {
            Undo.RecordObject(hostBtn, "Wire Host Button");
            hostBtn.onClick.RemoveAllListeners();
            // Add Helper for void return types (UnityEvents require void)
            NetworkUIHelper helper = netManagerObj.GetComponent<NetworkUIHelper>();
            if (helper == null) helper = netManagerObj.AddComponent<NetworkUIHelper>();
            
            UnityEventTools.AddPersistentListener(hostBtn.onClick, helper.StartHostVoid);
            Debug.Log("Wired HOST Button to NetworkUIHelper.");

            // Create Host IP Display
            if (helper.hostIpDisplay == null)
            {
                GameObject ipTextObj = new GameObject("HostIPDisplay");
                ipTextObj.transform.SetParent(hostBtn.transform.parent, false); // Same panel
                // Position it below Host button? Or near it.
                RectTransform rt = ipTextObj.AddComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, -50); // Offset
                
                TextMeshProUGUI tmp = ipTextObj.AddComponent<TextMeshProUGUI>();
                tmp.text = "IP: ...";
                tmp.fontSize = 24;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;
                
                helper.hostIpDisplay = tmp;
                Undo.RegisterCreatedObjectUndo(ipTextObj, "Create Host IP Display");
            }
        }

        if (joinBtn != null)
        {
            Undo.RecordObject(joinBtn, "Wire Join Button");
            joinBtn.onClick.RemoveAllListeners();
            
            // Same for Client
            NetworkUIHelper helper = netManagerObj.GetComponent<NetworkUIHelper>();
            if (helper == null) helper = netManagerObj.AddComponent<NetworkUIHelper>();

            UnityEventTools.AddPersistentListener(joinBtn.onClick, helper.StartClientVoid);
            Debug.Log("Wired JOIN Button to NetworkUIHelper.");

            // Create Join Input Field
            if (helper.joinIpInput == null)
            {
                GameObject inputObj = new GameObject("JoinIPInput");
                inputObj.transform.SetParent(joinBtn.transform.parent, false);
                
                // We need a proper TMP Input Field structure... this is complex via code.
                // Let's create a simplified one: Image + TMP_InputField + Text Area
                
                RectTransform rootRT = inputObj.AddComponent<RectTransform>();
                rootRT.sizeDelta = new Vector2(300, 40);
                rootRT.anchoredPosition = new Vector2(0, -120); // Below buttons

                Image bg = inputObj.AddComponent<Image>();
                bg.color = new Color(1, 1, 1, 0.8f);

                GameObject textArea = new GameObject("Text Area");
                textArea.transform.SetParent(inputObj.transform, false);
                RectTransform areaRT = textArea.AddComponent<RectTransform>();
                areaRT.anchorMin = new Vector2(0, 0);
                areaRT.anchorMax = new Vector2(1, 1);
                areaRT.offsetMin = new Vector2(10, 0);
                areaRT.offsetMax = new Vector2(-10, 0);

                GameObject placeholder = new GameObject("Placeholder");
                placeholder.transform.SetParent(textArea.transform, false);
                TextMeshProUGUI phText = placeholder.AddComponent<TextMeshProUGUI>();
                phText.text = "Enter Host IP...";
                phText.fontSize = 20;
                phText.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                RectTransform phRT = placeholder.GetComponent<RectTransform>();
                phRT.anchorMin = Vector2.zero;
                phRT.anchorMax = Vector2.one;

                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(textArea.transform, false);
                TextMeshProUGUI txtMain = textObj.AddComponent<TextMeshProUGUI>();
                txtMain.fontSize = 20;
                txtMain.color = Color.black;
                RectTransform txRT = textObj.GetComponent<RectTransform>();
                txRT.anchorMin = Vector2.zero;
                txRT.anchorMax = Vector2.one;

                TMP_InputField input = inputObj.AddComponent<TMP_InputField>();
                input.textViewport = areaRT;
                input.textComponent = txtMain;
                input.placeholder = phText;
                
                helper.joinIpInput = input;
                Undo.RegisterCreatedObjectUndo(inputObj, "Create Join Input");
            }
        }

        if (hostBtn == null || joinBtn == null)
        {
            Debug.LogWarning("Could not find Host/Join buttons. Run Step 7 first.");
        }
        else
        {
            // 3. Create and Assign Default Player Prefab
            CreateAndAssignPlayerPrefab(netManager, netManagerObj);

            EditorUtility.DisplayDialog("Success", "NetworkManager created, Buttons wired, and Player Prefab (Cylinder) assigned!", "OK");
        }
    }

    private static void CreateAndAssignPlayerPrefab(NetworkManager netManager, GameObject netManagerObj)
    {
        string prefabPath = "Assets/Resources/DefaultPlayerCylinder.prefab";
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (playerPrefab == null)
        {
            // Create the Cylinder
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = "DefaultPlayerCylinder";
            
            // Add NetworkObject (Essential for Multiplayer)
            cylinder.AddComponent<NetworkObject>(); // NGO component

            // Ensure Resources folder exists
            if (!System.IO.Directory.Exists(Application.dataPath + "/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            // Save as Prefab
            playerPrefab = PrefabUtility.SaveAsPrefabAsset(cylinder, prefabPath);
            GameObject.DestroyImmediate(cylinder);
            Debug.Log("Created DefaultPlayerCylinder prefab.");
        }

        // Assign to NetworkManager
        // In NGO, we need to set the PlayerPrefab field.
        // We use SerializedObject because sometimes properties are hidden or internal setter
        SerializedObject so = new SerializedObject(netManager);
        SerializedProperty networkConfigProp = so.FindProperty("NetworkConfig");
        
        if (networkConfigProp != null)
        {
            SerializedProperty playerPrefabProp = networkConfigProp.FindPropertyRelative("PlayerPrefab");
            if (playerPrefabProp != null)
            {
                playerPrefabProp.objectReferenceValue = playerPrefab;
                so.ApplyModifiedProperties();
                Debug.Log("Assigned Player Prefab to NetworkManager.");
            }
            else
            {
                Debug.LogError("Could not find PlayerPrefab property in NetworkConfig.");
            }
        }
    }
}
