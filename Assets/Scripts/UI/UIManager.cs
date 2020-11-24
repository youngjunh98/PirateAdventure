using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<string, UIElement> m_elementMap;

    private Dictionary<string, UIElement> ElementMap
    {
        get
        {
            if (m_elementMap == null)
            {
                m_elementMap = new Dictionary<string, UIElement> ();
            }

            return m_elementMap;
        }
    }

    public void RegisterElement (UIElement element)
    {
        if (ElementMap.ContainsKey (element.name))
        {
            return;
        }

        ElementMap.Add (element.name, element);
    }

    public void UnregisterElement (UIElement element)
    {
        if (ElementMap.ContainsKey (element.name))
        {
            ElementMap.Remove (element.name);
        }
    }

    public UIElement FindElement (string name, bool bFindUnregistered)
    {
        UIElement found = null;

        if (ElementMap.ContainsKey (name))
        {
            found = ElementMap[name];
        }

        if (found == null && bFindUnregistered)
        {
            foreach (var gameObject in gameObject.scene.GetRootGameObjects ())
            {
                foreach (var uiElement in gameObject.GetComponentsInChildren<UIElement> (true))
                {
                    if (uiElement.name == name)
                    {
                        found = gameObject.GetComponent<UIElement> ();
                        break;
                    }
                }
            }
        }

        return found;
    }
}
