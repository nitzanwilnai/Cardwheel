using CommonTools;
using TMPro;

namespace Cardwheel
{
    public static class CommonChipsVisual
    {
        public static void InitChipsInfo(GUIRef guiRef, TextMeshProUGUI[] baseChipsText, ref TextMeshProUGUI mostFrequent, ref TextMeshProUGUI leastFrequent)
        {
            GUIRef chipsGUIRef = guiRef.GetGameObject("Chips").GetComponent<GUIRef>();
            for (int i = 0; i < baseChipsText.Length; i++)
                baseChipsText[i] = chipsGUIRef.GetTextGUI("Chips" + (i + 1));

            mostFrequent = chipsGUIRef.GetTextGUI("Most");
            leastFrequent = chipsGUIRef.GetTextGUI("Least");

        }

        public static void Show(RunData runData, Balance balance, TextMeshProUGUI[] baseChipsText, TextMeshProUGUI mostFrequent, TextMeshProUGUI leastFrquent)
        {
            for (int i = 0; i < baseChipsText.Length; i++)
                baseChipsText[i].text = "+" + runData.BaseChips[i].ToString("N0");

            SLOT_TYPE mostPlayedType = Logic.GetMostPlayedSlotType(runData);
            mostFrequent.text = mostPlayedType.ToString();
            mostFrequent.color = balance.SlotColors[(int)mostPlayedType];

            SLOT_TYPE leastPlayedType = Logic.GetLeastPlayedSlotType(runData);
            leastFrquent.text = leastPlayedType.ToString();
            leastFrquent.color = balance.SlotColors[(int)leastPlayedType];
        }
    }
}