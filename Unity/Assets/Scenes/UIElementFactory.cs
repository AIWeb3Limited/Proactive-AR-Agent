using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// UI Element Factory - Creates UI elements based on parameters
/// </summary>
public class UIElementFactory
{
    private Transform canvasTransform;
    private Dictionary<string, GameObject> createdElements;

    public UIElementFactory(Transform canvas)
    {
        canvasTransform = canvas;
        createdElements = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// Create UI element based on data
    /// </summary>
    public GameObject CreateUIElement(UIElementData data, Transform parent = null)
    {
        GameObject element = null;
        Transform parentTransform = parent ?? canvasTransform;

        switch (data.elementType)
        {
            case UIElementType.Panel:
                element = CreatePanel(data, parentTransform);
                break;
            case UIElementType.Button:
                element = CreateButton(data, parentTransform);
                break;
            case UIElementType.Text:
                element = CreateText(data, parentTransform);
                break;
            case UIElementType.Image:
                element = CreateImage(data, parentTransform);
                break;
            case UIElementType.InputField:
                element = CreateInputField(data, parentTransform);
                break;
        }

        if (element != null)
        {
            SetupCommonProperties(element, data);
            if (!string.IsNullOrEmpty(data.id))
            {
                createdElements[data.id] = element;
            }
            CreateChildElements(data, element.transform);
        }

        return element;
    }

    private GameObject CreatePanel(UIElementData data, Transform parent)
    {
        GameObject panel = new GameObject(data.name ?? "Panel");
        panel.transform.SetParent(parent, false);
        panel.AddComponent<RectTransform>();
        Image image = panel.AddComponent<Image>();
        image.color = data.backgroundColor;
        return panel;
    }

    private GameObject CreateButton(UIElementData data, Transform parent)
    {
        GameObject button = new GameObject(data.name ?? "Button");
        button.transform.SetParent(parent, false);
        button.AddComponent<RectTransform>();
        Image image = button.AddComponent<Image>();
        Button buttonComponent = button.AddComponent<Button>();
        
        image.color = data.backgroundColor;
        buttonComponent.interactable = data.interactable;

        if (!string.IsNullOrEmpty(data.text))
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(button.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = data.text;
            text.color = data.textColor;
            text.fontSize = data.fontSize;
            text.alignment = TextAlignmentOptions.Center;
        }
        return button;
    }

    private GameObject CreateText(UIElementData data, Transform parent)
    {
        GameObject textObj = new GameObject(data.name ?? "Text");
        textObj.transform.SetParent(parent, false);
        textObj.AddComponent<RectTransform>();
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        
        text.text = data.text ?? "Text";
        text.color = data.textColor;
        text.fontSize = data.fontSize;
        return textObj;
    }

    private GameObject CreateImage(UIElementData data, Transform parent)
    {
        GameObject imageObj = new GameObject(data.name ?? "Image");
        imageObj.transform.SetParent(parent, false);
        imageObj.AddComponent<RectTransform>();
        Image image = imageObj.AddComponent<Image>();
        image.color = data.backgroundColor;
        return imageObj;
    }

    private GameObject CreateInputField(UIElementData data, Transform parent)
    {
        GameObject inputField = new GameObject(data.name ?? "InputField");
        inputField.transform.SetParent(parent, false);
        inputField.AddComponent<RectTransform>();
        Image image = inputField.AddComponent<Image>();
        TMP_InputField input = inputField.AddComponent<TMP_InputField>();
        
        image.color = data.backgroundColor;
        input.interactable = data.interactable;
        
        // Create placeholder text
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputField.transform, false);
        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.sizeDelta = Vector2.zero;
        placeholderRect.offsetMin = new Vector2(10, 5);
        placeholderRect.offsetMax = new Vector2(-10, -5);
        
        TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholderText.text = data.placeholder;
        placeholderText.color = new Color(data.textColor.r, data.textColor.g, data.textColor.b, 0.5f);
        placeholderText.fontSize = data.fontSize;
        placeholderText.alignment = TextAlignmentOptions.MidlineLeft;
        
        // Create input text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputField.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);
        
        TextMeshProUGUI inputText = textObj.AddComponent<TextMeshProUGUI>();
        inputText.text = "";
        inputText.color = data.textColor;
        inputText.fontSize = data.fontSize;
        inputText.alignment = TextAlignmentOptions.MidlineLeft;
        
        // Setup input field references
        input.textComponent = inputText;
        input.placeholder = placeholderText;
        
        return inputField;
    }

    private void SetupCommonProperties(GameObject element, UIElementData data)
    {
        RectTransform rectTransform = element.GetComponent<RectTransform>();
        
        // Set anchors and pivot
        rectTransform.anchorMin = data.anchorMin;
        rectTransform.anchorMax = data.anchorMax;
        rectTransform.pivot = data.pivot;
        
        // Set position and size
        rectTransform.anchoredPosition = data.position;
        rectTransform.sizeDelta = data.size;
        
        // Set name
        element.name = data.name ?? element.name;
        
        // Add layout components if specified
        SetupLayoutComponents(element, data);
    }

    private void SetupLayoutComponents(GameObject element, UIElementData data)
    {
        if (string.IsNullOrEmpty(data.layoutType) || data.layoutType == "None")
            return;

        switch (data.layoutType)
        {
            case "Vertical":
                VerticalLayoutGroup verticalLayout = element.GetComponent<VerticalLayoutGroup>();
                if (verticalLayout == null)
                {
                    verticalLayout = element.AddComponent<VerticalLayoutGroup>();
                }
                verticalLayout.spacing = data.spacing;
                verticalLayout.childControlWidth = true;
                verticalLayout.childControlHeight = true;
                verticalLayout.childForceExpandWidth = false;
                verticalLayout.childForceExpandHeight = false;
                verticalLayout.childAlignment = TextAnchor.UpperCenter;
                
                // Set padding if specified
                if (data.padding != Vector2.zero)
                {
                    verticalLayout.padding = new RectOffset(
                        (int)data.padding.x, (int)data.padding.x,
                        (int)data.padding.y, (int)data.padding.y);
                }
                break;
                
            case "Horizontal":
                HorizontalLayoutGroup horizontalLayout = element.GetComponent<HorizontalLayoutGroup>();
                if (horizontalLayout == null)
                {
                    horizontalLayout = element.AddComponent<HorizontalLayoutGroup>();
                }
                horizontalLayout.spacing = data.spacing;
                horizontalLayout.childControlWidth = true;
                horizontalLayout.childControlHeight = true;
                horizontalLayout.childForceExpandWidth = false;
                horizontalLayout.childForceExpandHeight = false;
                horizontalLayout.childAlignment = TextAnchor.MiddleCenter;
                
                // Set padding if specified
                if (data.padding != Vector2.zero)
                {
                    horizontalLayout.padding = new RectOffset(
                        (int)data.padding.x, (int)data.padding.x,
                        (int)data.padding.y, (int)data.padding.y);
                }
                break;
                
            case "Grid":
                GridLayoutGroup gridLayout = element.GetComponent<GridLayoutGroup>();
                if (gridLayout == null)
                {
                    gridLayout = element.AddComponent<GridLayoutGroup>();
                }
                gridLayout.spacing = new Vector2(data.spacing, data.spacing);
                gridLayout.cellSize = new Vector2(100, 100); // Default cell size
                gridLayout.childAlignment = TextAnchor.UpperLeft;
                
                // Set padding if specified
                if (data.padding != Vector2.zero)
                {
                    gridLayout.padding = new RectOffset(
                        (int)data.padding.x, (int)data.padding.x,
                        (int)data.padding.y, (int)data.padding.y);
                }
                break;
        }
    }

    private void CreateChildElements(UIElementData data, Transform parent)
    {
        if (data.children != null && data.children.Count > 0)
        {
            foreach (UIElementData child in data.children)
            {
                GameObject childElement = CreateUIElement(child, parent);
                
                // For layout group children, set proper anchors and positioning
                if (IsLayoutGroupChild(parent))
                {
                    SetupLayoutGroupChild(childElement, child);
                }
            }
        }
    }

    private bool IsLayoutGroupChild(Transform parent)
    {
        if (parent == null) return false;
        
        return parent.GetComponent<VerticalLayoutGroup>() != null ||
               parent.GetComponent<HorizontalLayoutGroup>() != null ||
               parent.GetComponent<GridLayoutGroup>() != null;
    }

    private void SetupLayoutGroupChild(GameObject element, UIElementData data)
    {
        if (element == null) return;
        
        RectTransform rectTransform = element.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        
        // For layout group children, we need to set appropriate anchors
        // The layout group will control the position and size
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        // Add LayoutElement component if size is specified
        if (data.size != Vector2.zero)
        {
            LayoutElement layoutElement = element.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = element.AddComponent<LayoutElement>();
            }
            
            layoutElement.preferredWidth = data.size.x;
            layoutElement.preferredHeight = data.size.y;
        }
    }

    public GameObject GetElement(string id)
    {
        return createdElements.ContainsKey(id) ? createdElements[id] : null;
    }

    public void ClearAll()
    {
        foreach (var element in createdElements.Values)
        {
            if (element != null)
            {
                UnityEngine.Object.DestroyImmediate(element);
            }
        }
        createdElements.Clear();
    }
} 