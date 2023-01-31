#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PortalDebug : EditorWindow
{
    public static readonly List<Portal> RenderedPortals = new List<Portal>();

    Vector2 scrollPos;
    bool showRecursion;
    
    [MenuItem("Tools/Debug portals")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PortalDebug), false, "Portal Debug");
    }

    void ListPortalsRecursive(Portal portal, int depth, int maxDepth)
    {
        for (int i = 0; i < Math.Min(maxDepth - depth, portal.VisiblePortals.Length); ++i)
        {
            var subPortal = portal.VisiblePortals[i];
            var indents = new string('┼', depth - 1); // ─
            var text = $"{(depth == maxDepth - 1 ? '└' : '├')}{indents} {(i + 1)}. {subPortal.name}";
            GUILayout.Label(text);
            ListPortalsRecursive(subPortal, depth + 1, maxDepth);
        }
    }

    void OnGUI()
    {
        GUILayout.Label((RenderedPortals.Count == 0 ? "No " : "") + "Rendering Portals", EditorStyles.largeLabel);

        showRecursion = GUILayout.Toggle(showRecursion, "List recursion");

        using (var scrollbox = new GUILayout.ScrollViewScope(scrollPos))
        {
            scrollPos = scrollbox.scrollPosition;

            for (int i = 0; i < RenderedPortals.Count; ++i)
            {
                var portal = RenderedPortals[i];
                if (portal == null)
                    continue; // can happen if portal destroyed

                var text = $"{(i + 1)}. {portal.name}; see: {portal.VisiblePortals.Length}; recurs:{portal.SubRenderCount - 1}";
                GUILayout.Label(text, EditorStyles.boldLabel);

                if (showRecursion)
                    ListPortalsRecursive(portal, 1, portal.MaxRecursion);
            }
        }
    }
}

#endif