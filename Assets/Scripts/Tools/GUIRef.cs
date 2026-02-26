/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonTools
{
    [Serializable]
    public struct GUIRefGameObject
    {
        public string Name;
        public GameObject Value;
    }

    [Serializable]
    public struct GUIRefTextGUI
    {
        public string Name;
        public TextMeshProUGUI Value;
    }

    [Serializable]
    public struct GUIRefImage
    {
        public string Name;
        public Image Value;
    }

    [Serializable]
    public struct GUIRefButton
    {
        public string Name;
        public Button Value;
    }

    [Serializable]
    public struct GUIRefAnimation
    {
        public string Name;
        public Animation Value;
    }

    [Serializable]
    public struct GUIRefParticleSystem
    {
        public string Name;
        public ParticleSystem Value;
    }

    public class GUIRef : MonoBehaviour
    {
        public GUIRefGameObject[] GUIRefGameObjects;
        public GUIRefTextGUI[] GUIRefTextGUI;
        public GUIRefImage[] GUIRefImages;
        public GUIRefButton[] GUIRefButtons;
        public GUIRefAnimation[] GUIRefAnimations;
        public GUIRefParticleSystem[] GUIRefParticleSystems;

        public GameObject GetGameObject(string name)
        {
            int numObjects = GUIRefGameObjects.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefGameObjects[i].Name == name)
                    return GUIRefGameObjects[i].Value;

            Debug.LogErrorFormat(
                "GUIRef " + gameObject.name + " GetGameObject(" + name + ") does not exist!"
            );
            return null;
        }

        public TextMeshProUGUI GetTextGUI(string name)
        {
            int numObjects = GUIRefTextGUI.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefTextGUI[i].Name == name)
                    return GUIRefTextGUI[i].Value;

            Debug.LogErrorFormat(
                "GUIRef " + gameObject.name + " GetTextGUI(" + name + ") does not exist!"
            );
            return null;
        }

        public Image GetImage(string name)
        {
            int numObjects = GUIRefImages.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefImages[i].Name == name)
                    return GUIRefImages[i].Value;

            Debug.LogErrorFormat(
                "GUIRef " + gameObject.name + " GetImage(" + name + ") does not exist!"
            );
            return null;
        }

        public Button GetButton(string name)
        {
            int numObjects = GUIRefButtons.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefButtons[i].Name == name)
                    return GUIRefButtons[i].Value;

            Debug.LogErrorFormat(
                "GUIRef " + gameObject.name + " GetButton(" + name + ") does not exist!"
            );
            return null;
        }

        public Animation GetAnimation(string name)
        {
            int numObjects = GUIRefAnimations.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefAnimations[i].Name == name)
                    return GUIRefAnimations[i].Value;

            Debug.LogErrorFormat(
                "GUIRef " + gameObject.name + " GetAnimation(" + name + ") does not exist!"
            );
            return null;
        }

        public ParticleSystem GetParticleSystem(string name)
        {
            int numObjects = GUIRefParticleSystems.Length;
            for (int i = 0; i < numObjects; i++)
                if (GUIRefParticleSystems[i].Name == name)
                    return GUIRefParticleSystems[i].Value;

            Debug.LogErrorFormat(
                "GUIRef " + gameObject.name + " GetParticleSystem(" + name + ") does not exist!"
            );
            return null;
        }
    }
}
