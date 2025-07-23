using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// JSON Helper Class - Simplified tool for LLM to generate UI layouts
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// Create UILayoutData object from simplified data
    /// This method makes it easier for LLM to generate UI layouts
    /// </summary>
    public static UILayoutData CreateLayoutFromSimpleData(
        string layoutName,
        string description = "",
        Vector2? canvasSize = null,
        string theme = "Default",
        Color? primaryColor = null)
    {
        UILayoutData layout = new UILayoutData
        {
            layoutName = layoutName,
            description = description,
            canvasSize = canvasSize ?? new Vector2(1920, 1080),
            theme = theme,
            primaryColor = primaryColor ?? Color.blue,
            rootElements = new List<UIElementData>()
        };

        return layout;
    }

    /// <summary>
    /// Simplified method to create UI elements
    /// </summary>
    public static UIElementData CreateElement(
        string id,
        UIElementType elementType,
        string name = "",
        string text = "",
        Vector2? position = null,
        Vector2? size = null,
        Vector2? anchorMin = null,
        Vector2? anchorMax = null,
        Color? backgroundColor = null,
        Color? textColor = null,
        float fontSize = 14f,
        string onClick = "",
        string placeholder = "",
        string layoutType = "None",
        float spacing = 5f)
    {
        UIElementData element = new UIElementData
        {
            id = id,
            elementType = elementType,
            name = string.IsNullOrEmpty(name) ? id : name,
            text = text,
            position = position ?? Vector2.zero,
            size = size ?? new Vector2(100, 30),
            anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f),
            anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f),
            pivot = new Vector2(0.5f, 0.5f),
            backgroundColor = backgroundColor ?? Color.white,
            textColor = textColor ?? Color.black,
            fontSize = fontSize,
            onClick = onClick,
            placeholder = placeholder,
            layoutType = layoutType,
            spacing = spacing,
            children = new List<UIElementData>()
        };

        return element;
    }

    /// <summary>
    /// Quick create button
    /// </summary>
    public static UIElementData CreateButton(string id, string text, string onClick = "", Vector2? position = null, Vector2? size = null)
    {
        return CreateElement(
            id: id,
            elementType: UIElementType.Button,
            text: text,
            position: position,
            size: size ?? new Vector2(120, 40),
            anchorMin: new Vector2(0.5f, 0.5f),
            anchorMax: new Vector2(0.5f, 0.5f),
            backgroundColor: new Color(0.2f, 0.6f, 1.0f, 1.0f),
            textColor: Color.white,
            onClick: onClick
        );
    }

    /// <summary>
    /// Quick create text
    /// </summary>
    public static UIElementData CreateText(string id, string text, float fontSize = 14f, Vector2? position = null)
    {
        return CreateElement(
            id: id,
            elementType: UIElementType.Text,
            text: text,
            position: position,
            fontSize: fontSize,
            anchorMin: new Vector2(0.5f, 0.5f),
            anchorMax: new Vector2(0.5f, 0.5f),
            backgroundColor: Color.clear
        );
    }

    /// <summary>
    /// Quick create input field
    /// </summary>
    public static UIElementData CreateInputField(string id, string placeholder = "", Vector2? position = null, Vector2? size = null)
    {
        return CreateElement(
            id: id,
            elementType: UIElementType.InputField,
            placeholder: placeholder,
            position: position,
            size: size ?? new Vector2(200, 30),
            anchorMin: new Vector2(0.5f, 0.5f),
            anchorMax: new Vector2(0.5f, 0.5f),
            backgroundColor: Color.white
        );
    }

    /// <summary>
    /// Quick create panel
    /// </summary>
    public static UIElementData CreatePanel(string id, string layoutType = "None", Vector2? position = null, Vector2? size = null, Color? backgroundColor = null)
    {
        return CreateElement(
            id: id,
            elementType: UIElementType.Panel,
            position: position,
            size: size ?? new Vector2(300, 200),
            anchorMin: new Vector2(0.5f, 0.5f),
            anchorMax: new Vector2(0.5f, 0.5f),
            backgroundColor: backgroundColor ?? new Color(0.9f, 0.9f, 0.9f, 1.0f),
            layoutType: layoutType
        );
    }

    /// <summary>
    /// Generate example login interface JSON string
    /// Shows how LLM should build UI layouts
    /// </summary>
    public static string GenerateLoginExampleJson()
    {
        // Create layout
        UILayoutData layout = CreateLayoutFromSimpleData("Login Interface", "Simple login interface example");

        // Create main panel
        UIElementData mainPanel = CreatePanel("main_panel", "Vertical", Vector2.zero, new Vector2(400, 300));
        mainPanel.anchorMin = new Vector2(0.5f, 0.5f);
        mainPanel.anchorMax = new Vector2(0.5f, 0.5f);
        mainPanel.pivot = new Vector2(0.5f, 0.5f);
        mainPanel.spacing = 20f;
        mainPanel.padding = new Vector2(20, 20);

        // Add child elements
        mainPanel.children.Add(CreateText("title", "User Login", 24f));
        mainPanel.children.Add(CreateInputField("username", "Enter username"));
        mainPanel.children.Add(CreateInputField("password", "Enter password"));
        mainPanel.children.Add(CreateButton("login_btn", "Login", "OnLoginClick"));

        // Add to layout
        layout.rootElements.Add(mainPanel);

        // Convert to JSON
        return JsonUtility.ToJson(layout, true);
    }

    /// <summary>
    /// Validate JSON format
    /// </summary>
    public static bool ValidateLayoutJson(string jsonString, out string errorMessage)
    {
        errorMessage = "";
        
        try
        {
            UILayoutData layout = JsonUtility.FromJson<UILayoutData>(jsonString);
            
            if (layout == null)
            {
                errorMessage = "JSON parse result is null";
                return false;
            }
            
            if (string.IsNullOrEmpty(layout.layoutName))
            {
                errorMessage = "Layout name cannot be empty";
                return false;
            }
            
            if (layout.rootElements == null || layout.rootElements.Count == 0)
            {
                errorMessage = "Must contain at least one root element";
                return false;
            }
            
            return true;
        }
        catch (System.Exception e)
        {
            errorMessage = $"JSON format error: {e.Message}";
            return false;
        }
    }
} 