using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public static class CommonChipsVisual
    {
        public static void InitChipsInfo(GUIRef guiRef, TextMeshProUGUI[] baseChipsText)
        {
            GUIRef chipsGUIRef = guiRef.GetGameObject("Chips").GetComponent<GUIRef>();
            for (int i = 0; i < baseChipsText.Length; i++)
                baseChipsText[i] = chipsGUIRef.GetTextGUI("Chips" + (i + 1));

        }

        public static void Show(RunData runData, TextMeshProUGUI[] baseChipsText)
        {
            for (int i = 0; i < baseChipsText.Length; i++)
                baseChipsText[i].text = "+" + runData.BaseChips[i].ToString("N0");

        }
    }
}