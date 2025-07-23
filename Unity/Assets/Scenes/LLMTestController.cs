using UnityEngine;

/// <summary>
/// LLM Test Controller - Demonstrates how LLM system calls UI generation
/// </summary>
public class LLMTestController : MonoBehaviour
{
    private UILayoutManager layoutManager;

    void Start()
    {
        layoutManager = FindObjectOfType<UILayoutManager>();
        if (layoutManager == null)
        {
            Debug.LogError("UILayoutManager not found! Please ensure UILayoutManager component exists in scene.");
            return;
        }

        // Register event listeners
        layoutManager.OnElementClicked += OnElementClicked;

        Debug.Log("LLM Test Controller started, press number keys to test different UI layouts:");
        Debug.Log("1 - Create Simple Button Layout");
        Debug.Log("2 - Create Form Layout");
        Debug.Log("3 - Create Menu Layout");
        Debug.Log("4 - Test Specific JSON Data (User Provided JSON)");
    }

    void Update()
    {
        // Test different UI layouts
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreateSimpleButtonLayout();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreateFormLayout();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CreateMenuLayout();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestSpecificJsonData();
        }
    }

    /// <summary>
    /// Test 1: Create Simple Button Layout
    /// Simulates LLM generating simple interface
    /// </summary>
    void CreateSimpleButtonLayout()
    {
        Debug.Log("=== Test 1: Create Simple Button Layout ===");

        // Use JsonHelper for quick creation (recommended for LLM)
        UILayoutData layout = JsonHelper.CreateLayoutFromSimpleData("Simple Button Interface", "Interface with three buttons");

        //// Add three buttons with proper sizing  
        ///必须要有一个root element
        layout.rootElements.Add(JsonHelper.CreateButton("btn_start", "Start Game", "OnStartGame", new Vector2(0, 50), new Vector2(200, 50)));
        layout.rootElements.Add(JsonHelper.CreateButton("btn_settings", "Settings", "OnSettings", new Vector2(0, 0), new Vector2(200, 50)));
        layout.rootElements.Add(JsonHelper.CreateButton("btn_exit", "Exit", "OnExit", new Vector2(0, -50), new Vector2(200, 50)));

        // Convert to JSON and create UI
        string json = JsonUtility.ToJson(layout, true);
        Debug.Log("Generated JSON:\n" + json);

        bool success = layoutManager.CreateLayoutFromJson(json);
        Debug.Log(success ? "✅ Interface created successfully!" : "❌ Interface creation failed!");
    }

    /// <summary>
    /// Test 2: Create Form Layout
    /// Simulates LLM generating complex form
    /// </summary>
    void CreateFormLayout()
    {
        Debug.Log("=== Test 2: Create Form Layout ===");

        UILayoutData layout = JsonHelper.CreateLayoutFromSimpleData("User Registration Form", "User registration information form");
        
        // Create main panel
        UIElementData mainPanel = JsonHelper.CreatePanel("form_panel", "Vertical", Vector2.zero, new Vector2(400, 500));
        mainPanel.anchorMin = new Vector2(0.5f, 0.5f);
        mainPanel.anchorMax = new Vector2(0.5f, 0.5f);
        mainPanel.pivot = new Vector2(0.5f, 0.5f);
        mainPanel.spacing = 15f;
        mainPanel.padding = new Vector2(30, 30);

        // Add form elements with proper sizing
        mainPanel.children.Add(JsonHelper.CreateText("form_title", "User Registration", 28f));
        
        var usernameField = JsonHelper.CreateInputField("username_field", "Enter username");
        usernameField.size = new Vector2(300, 35);
        mainPanel.children.Add(usernameField);
        
        var emailField = JsonHelper.CreateInputField("email_field", "Enter email");
        emailField.size = new Vector2(300, 35);
        mainPanel.children.Add(emailField);
        
        var passwordField = JsonHelper.CreateInputField("password_field", "Enter password");
        passwordField.size = new Vector2(300, 35);
        mainPanel.children.Add(passwordField);
        
        var registerBtn = JsonHelper.CreateButton("register_btn", "Register", "OnRegister");
        registerBtn.size = new Vector2(150, 40);
        mainPanel.children.Add(registerBtn);
        
        var cancelBtn = JsonHelper.CreateButton("cancel_btn", "Cancel", "OnCancel");
        cancelBtn.size = new Vector2(150, 40);
        mainPanel.children.Add(cancelBtn);

        layout.rootElements.Add(mainPanel);

        string json = JsonUtility.ToJson(layout, true);
        bool success = layoutManager.CreateLayoutFromJson(json);
        Debug.Log(success ? "✅ Form interface created successfully!" : "❌ Form interface creation failed!");
    }

    /// <summary>
    /// Test 3: Create Menu Layout
    /// Simulates LLM generating game menu
    /// </summary>
    void CreateMenuLayout()
    {
        Debug.Log("=== Test 3: Create Menu Layout ===");

        UILayoutData layout = JsonHelper.CreateLayoutFromSimpleData("Game Main Menu", "Game main menu interface");
        
        // Create left menu panel
        UIElementData leftPanel = JsonHelper.CreatePanel("left_menu", "Vertical", new Vector2(-300, 0), new Vector2(250, 400));
        leftPanel.anchorMin = new Vector2(0.5f, 0.5f);
        leftPanel.anchorMax = new Vector2(0.5f, 0.5f);
        leftPanel.pivot = new Vector2(0.5f, 0.5f);
        leftPanel.spacing = 15f;
        leftPanel.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);

        leftPanel.children.Add(JsonHelper.CreateText("menu_title", "Game Menu", 20f));
        
        var newGameBtn = JsonHelper.CreateButton("new_game", "New Game", "OnNewGame");
        newGameBtn.size = new Vector2(200, 45);
        leftPanel.children.Add(newGameBtn);
        
        var loadGameBtn = JsonHelper.CreateButton("load_game", "Load Game", "OnLoadGame");
        loadGameBtn.size = new Vector2(200, 45);
        leftPanel.children.Add(loadGameBtn);
        
        var optionsBtn = JsonHelper.CreateButton("options", "Options", "OnOptions");
        optionsBtn.size = new Vector2(200, 45);
        leftPanel.children.Add(optionsBtn);
        
        var creditsBtn = JsonHelper.CreateButton("credits", "Credits", "OnCredits");
        creditsBtn.size = new Vector2(200, 45);
        leftPanel.children.Add(creditsBtn);

        // Create right info panel
        UIElementData rightPanel = JsonHelper.CreatePanel("info_panel", "Vertical", new Vector2(300, 0), new Vector2(250, 400));
        rightPanel.anchorMin = new Vector2(0.5f, 0.5f);
        rightPanel.anchorMax = new Vector2(0.5f, 0.5f);
        rightPanel.pivot = new Vector2(0.5f, 0.5f);
        rightPanel.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.8f);

        rightPanel.children.Add(JsonHelper.CreateText("info_title", "Game Info", 18f));
        rightPanel.children.Add(JsonHelper.CreateText("version", "Version: v1.0.0", 14f));
        rightPanel.children.Add(JsonHelper.CreateText("player_name", "Player: Test User", 14f));

        layout.rootElements.Add(leftPanel);
        layout.rootElements.Add(rightPanel);

        string json = JsonUtility.ToJson(layout, true);
        bool success = layoutManager.CreateLayoutFromJson(json);
        Debug.Log(success ? "✅ Menu interface created successfully!" : "❌ Menu interface creation failed!");
    }

    /// <summary>
    /// Test 4: Test User Provided Specific JSON Data
    /// This tests the specific JSON format requested by user
    /// </summary>
    void TestSpecificJsonData()
    {
        Debug.Log("=== Test 4: Test User Specified JSON Data ===");

        // User provided JSON data with fixed sizing
        string userJsonData = @"{
    ""layoutName"": ""My Interface"",
    ""description"": ""Interface Description"",
    ""canvasSize"": {""x"": 1920, ""y"": 1080},
    ""theme"": ""Modern"",
    ""primaryColor"": {""r"": 0.2, ""g"": 0.6, ""b"": 1.0, ""a"": 1.0},
    ""rootElements"": [
        {
            ""id"": ""main_panel"",
            ""elementType"": 0,
            ""name"": ""Main Panel"",
            ""position"": {""x"": 0, ""y"": 0},
            ""size"": {""x"": 400, ""y"": 300},
            ""anchorMin"": {""x"": 0.5, ""y"": 0.5},
            ""anchorMax"": {""x"": 0.5, ""y"": 0.5},
            ""pivot"": {""x"": 0.5, ""y"": 0.5},
            ""layoutType"": ""Vertical"",
            ""spacing"": 20,
            ""children"": [
                {
                    ""id"": ""title"",
                    ""elementType"": 2,
                    ""name"": ""Title"",
                    ""text"": ""Title Text"",
                    ""fontSize"": 24,
                    ""size"": {""x"": 350, ""y"": 60}
                },
                {
                    ""id"": ""button1"",
                    ""elementType"": 1,
                    ""name"": ""Button"",
                    ""text"": ""Click Me"",
                    ""onClick"": ""OnButtonClick"",
                    ""size"": {""x"": 200, ""y"": 50}
                }
            ]
        }
    ]
}";

        Debug.Log("Using JSON data:\n" + userJsonData);

        // Validate JSON format
        string errorMessage;
        bool isValid = JsonHelper.ValidateLayoutJson(userJsonData, out errorMessage);
        
        if (!isValid)
        {
            Debug.LogError($"❌ JSON format validation failed: {errorMessage}");
            return;
        }
        
        Debug.Log("✅ JSON format validation passed");

        // Clear existing UI
        layoutManager.ClearCurrentLayout();

        // Create UI
        bool success = layoutManager.CreateLayoutFromJson(userJsonData);

        if (success)
        {
            Debug.Log("✅ User specified JSON UI created successfully!");
            
            // Check created elements
            CheckUserJsonElements();
        }
        else
        {
            Debug.LogError("❌ User specified JSON UI creation failed!");
        }
    }

    /// <summary>
    /// Check UI elements created from user JSON
    /// </summary>
    private void CheckUserJsonElements()
    {
        Debug.Log("=== Checking User JSON Created UI Elements ===");
        
        // Check main panel
        GameObject mainPanel = layoutManager.GetElement("main_panel");
        Debug.Log($"Main Panel (main_panel): {(mainPanel != null ? "✅ Created successfully" : "❌ Creation failed")}");
        
        if (mainPanel != null)
        {
            Debug.Log($"Main panel position: {mainPanel.transform.position}");
            Debug.Log($"Main panel size: {mainPanel.GetComponent<RectTransform>().sizeDelta}");
        }
        
        // Check title
        GameObject title = layoutManager.GetElement("title");
        Debug.Log($"Title (title): {(title != null ? "✅ Created successfully" : "❌ Creation failed")}");
        
        if (title != null)
        {
            var textComponent = title.GetComponent<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                Debug.Log($"Title text: '{textComponent.text}'");
                Debug.Log($"Title font size: {textComponent.fontSize}");
            }
        }
        
        // Check button
        GameObject button = layoutManager.GetElement("button1");
        Debug.Log($"Button (button1): {(button != null ? "✅ Created successfully" : "❌ Creation failed")}");
        
        if (button != null)
        {
            var buttonComponent = button.GetComponent<UnityEngine.UI.Button>();
            Debug.Log($"Button component: {(buttonComponent != null ? "✅ Normal" : "❌ Abnormal")}");
            
            // Find button text
            var buttonText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (buttonText != null)
            {
                Debug.Log($"Button text: '{buttonText.text}'");
            }
            
            // Check button size
            var rectTransform = button.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Debug.Log($"Button size: {rectTransform.sizeDelta}");
            }
        }
    }

    /// <summary>
    /// Handle UI element click events
    /// </summary>
    private void OnElementClicked(string elementId, string clickMethod)
    {
        Debug.Log($"[LLMTestController] Element clicked: {elementId} -> {clickMethod}");
        
        switch (clickMethod)
        {
            case "OnButtonClick":
                Debug.Log("🔘 User specified test button was clicked!");
                ShowButtonClickedFeedback();
                break;
            case "OnStartGame":
                Debug.Log("🎮 Start Game button clicked!");
                break;
            case "OnSettings":
                Debug.Log("⚙️ Settings button clicked!");
                break;
            case "OnExit":
                Debug.Log("🚪 Exit button clicked!");
                break;
            case "OnRegister":
                Debug.Log("📝 Register button clicked!");
                break;
            case "OnCancel":
                Debug.Log("❌ Cancel button clicked!");
                break;
            default:
                Debug.Log($"🔘 Unknown click method: {clickMethod}");
                break;
        }
    }

    /// <summary>
    /// Show button clicked feedback
    /// </summary>
    private void ShowButtonClickedFeedback()
    {
        Debug.Log("💡 This is the button click effect defined in user JSON!");
        Debug.Log("📋 You can add specific business logic here");
    }

    void OnDestroy()
    {
        if (layoutManager != null)
        {
            layoutManager.OnElementClicked -= OnElementClicked;
        }
    }
} 