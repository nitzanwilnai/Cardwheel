using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class UIBallMoveData
    {
        public float MinDistance = 0.1f;
        public float Velocity = 20.0f;
        public float SelectedY = 0.35f;

        public int NumBalls;
        public Vector3[] BallStartPosition;
        public Vector3[] BallCurrentPosition;
        public Vector3[] BallTargetPosition;
        public int BallIdx;
        public GameObject[] BallGO;
        public Transform BallParent;

        public float MouseDownTime;
        public Vector3 MouseDownPos;
        public Vector3 MouseDiff;
        public bool MouseMoved;
    }

    public class UIBallVisualData
    {
        public Image[] BallImage;
        public TextMeshProUGUI[] BallDescription;

    }


    public static class CommonBallVisual
    {
        public static void InitBallsMoveData(Balance balance, GUIRef guiRef, UIBallMoveData uiBallMoveData)
        {
            uiBallMoveData.NumBalls = balance.MaxBalls;
            uiBallMoveData.BallStartPosition = new Vector3[uiBallMoveData.NumBalls];
            uiBallMoveData.BallCurrentPosition = new Vector3[uiBallMoveData.NumBalls];
            uiBallMoveData.BallTargetPosition = new Vector3[uiBallMoveData.NumBalls];
            uiBallMoveData.BallGO = new GameObject[uiBallMoveData.NumBalls];

            uiBallMoveData.BallParent = guiRef.GetGameObject("BallParent").transform;
            for (int i = 0; i < uiBallMoveData.NumBalls; i++)
            {
                GameObject go = guiRef.GetGameObject("Ball" + (i + 1).ToString());
                uiBallMoveData.BallGO[i] = go;
            }

        }

        public static void InitBallsVisualData(Balance balance, GUIRef guiRef, UIBallVisualData uiBallVisualData)
        {
            uiBallVisualData.BallImage = new Image[balance.MaxBalls];
            uiBallVisualData.BallDescription = new TextMeshProUGUI[balance.MaxBalls];
            for (int i = 0; i < balance.MaxBalls; i++)
            {
                uiBallVisualData.BallImage[i] = guiRef.GetImage("Ball" + (i + 1).ToString());
                uiBallVisualData.BallDescription[i] = guiRef.GetTextGUI("Description" + (i + 1));
            }
        }

        public static void ShowBalls(int[] ballTypes, Balance balance, UIBallVisualData uiBallVisualData)
        {
            for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
            {
                int ballType = ballTypes[ballIdx];
                uiBallVisualData.BallImage[ballIdx].sprite = AssetManager.Instance.LoadBallSprite(balance.BallBalance.BallSprite[ballType]);
                uiBallVisualData.BallDescription[ballIdx].text = balance.BallBalance.BallDescription[ballType];
            }
        }

        public static void PositionBalls(RunData runData, Balance balance, UIBallMoveData uiBallMoveData)
        {
            uiBallMoveData.BallIdx = -1;
            for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
                uiBallMoveData.BallTargetPosition[ballIdx] = uiBallMoveData.BallCurrentPosition[ballIdx] = uiBallMoveData.BallStartPosition[ballIdx] = uiBallMoveData.BallGO[ballIdx].transform.position;
        }

        public static void HideBalls(Balance balance, UIBallMoveData uiBallMoveData)
        {
            for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
                uiBallMoveData.BallGO[ballIdx].transform.position = uiBallMoveData.BallStartPosition[ballIdx];
        }

        public static void TickCheckSwapBalls(RunData runData, UIBallMoveData uiBallMoveData, UIBallVisualData uiBallVisualData, bool allowSelection)
        {
            if (uiBallMoveData.BallIdx > -1 && uiBallMoveData.BallIdx < uiBallMoveData.NumBalls - 1 && uiBallMoveData.BallCurrentPosition[uiBallMoveData.BallIdx].x < uiBallMoveData.BallCurrentPosition[uiBallMoveData.BallIdx + 1].x)
            {
                Debug.Log("swap balls " + uiBallMoveData.BallIdx + " and " + (uiBallMoveData.BallIdx + 1));

                int idx1 = uiBallMoveData.BallIdx;
                int idx2 = uiBallMoveData.BallIdx + 1;
                swapBalls(runData, uiBallMoveData, uiBallVisualData, idx1, idx2);

                uiBallMoveData.BallTargetPosition[idx1] = getBallStartPosWithSelection(runData, uiBallMoveData, idx1, allowSelection);

            }

            if (uiBallMoveData.BallIdx > 0 && uiBallMoveData.BallIdx < uiBallMoveData.NumBalls && uiBallMoveData.BallCurrentPosition[uiBallMoveData.BallIdx].x > uiBallMoveData.BallCurrentPosition[uiBallMoveData.BallIdx - 1].x)
            {
                int idx1 = uiBallMoveData.BallIdx;
                int idx2 = uiBallMoveData.BallIdx - 1;

                swapBalls(runData, uiBallMoveData, uiBallVisualData, idx1, idx2);

                uiBallMoveData.BallTargetPosition[idx1] = getBallStartPosWithSelection(runData, uiBallMoveData, idx1, allowSelection);
            }
        }

        private static void swapBalls(RunData runData, UIBallMoveData uiBallMoveData, UIBallVisualData uiBallVisualData, int idx1, int idx2)
        {
            Logic.SwapBalls(runData, idx1, idx2);

            Vector3 pos1 = uiBallMoveData.BallCurrentPosition[idx1];
            Vector3 pos2 = uiBallMoveData.BallCurrentPosition[idx2];
            uiBallMoveData.BallGO[idx1].transform.position = uiBallMoveData.BallCurrentPosition[idx1] = pos2;
            uiBallMoveData.BallGO[idx2].transform.position = uiBallMoveData.BallCurrentPosition[idx2] = pos1;

            Sprite sprite1 = uiBallVisualData.BallImage[idx1].sprite;
            Sprite sprite2 = uiBallVisualData.BallImage[idx2].sprite;
            uiBallVisualData.BallImage[idx1].sprite = sprite2;
            uiBallVisualData.BallImage[idx2].sprite = sprite1;

            string text1 = uiBallVisualData.BallDescription[idx1].text;
            string text2 = uiBallVisualData.BallDescription[idx2].text;
            uiBallVisualData.BallDescription[idx1].text = text2;
            uiBallVisualData.BallDescription[idx2].text = text1;

            uiBallMoveData.BallIdx = idx2;
        }


        public static void HanleInput(RunData runData, UIBallMoveData uiBallMoveData, Camera camera, bool allowSelection)
        {
#if UNITY_EDITOR
            bool mouseDown = Input.GetMouseButtonDown(0);
            bool mouseMove = Input.GetMouseButton(0);
            bool mouseUp = Input.GetMouseButtonUp(0);
            Vector3 mousePosition = Input.mousePosition;
#else
            bool mouseDown = (Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Began;
            bool mouseMove = (Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Moved;
            bool mouseUp = (Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled);
            Vector3 mousePosition = Vector3.zero;
            if (Input.touchCount > 0)
                mousePosition = Input.GetTouch(0).position;
#endif

            Vector3 worldPosition = camera.ScreenToWorldPoint(mousePosition);
            Vector3 localPosition = uiBallMoveData.BallParent.InverseTransformPoint(worldPosition);
            worldPosition.z = uiBallMoveData.BallGO[0].transform.position.z;
            localPosition.z = uiBallMoveData.BallGO[0].transform.localPosition.z;

            if (mouseDown)
            {
                //Debug.Log("mouseDown localPos (" + localPos.x + "," + localPos.y + ") tileX " + tileX + " tileY " + tileY + " closestIdx " + tileIdx + " insideIndex " + insideIdx + " itemIdx " + itemIdx);

                uiBallMoveData.MouseDownPos = worldPosition;
                uiBallMoveData.MouseDownTime = Time.realtimeSinceStartup;
                uiBallMoveData.MouseMoved = false;

                float worldDistance = float.MaxValue;
                int worldIdx = -1;
                for (int i = 0; i < uiBallMoveData.NumBalls; i++)
                {
                    Vector3 ballWorldPos = uiBallMoveData.BallGO[i].transform.position;
                    Vector3 ballLocalPos = uiBallMoveData.BallGO[i].transform.localPosition;

                    if (Vector2.Distance(worldPosition, ballWorldPos) < worldDistance)
                    {
                        worldDistance = Vector2.Distance(worldPosition, ballWorldPos);
                        worldIdx = i;
                    }
                }

                if (worldDistance < 1.0f)
                {
                    uiBallMoveData.BallIdx = worldIdx;
                    uiBallMoveData.MouseDiff = worldPosition - uiBallMoveData.BallCurrentPosition[uiBallMoveData.BallIdx];
                }

                uiBallMoveData.MouseMoved = false;
            }

            if (mouseMove)
            {
                if (Vector3.Distance(worldPosition, uiBallMoveData.MouseDownPos) > 0.5f)
                    uiBallMoveData.MouseMoved = true;

                if (uiBallMoveData.BallIdx > -1)
                    uiBallMoveData.BallTargetPosition[uiBallMoveData.BallIdx] = worldPosition - uiBallMoveData.MouseDiff;

                // Debug.Log("diff " + diff + " m_ballTargetPosition[" + m_ballIdx + "] " + m_ballTargetPosition[m_ballIdx]);
            }

            if (mouseUp)
            {
                if (uiBallMoveData.BallIdx > -1)
                {
                    Debug.Log("mouse down time " + (Time.realtimeSinceStartup - uiBallMoveData.MouseDownTime));
                    if (allowSelection && !uiBallMoveData.MouseMoved && Time.realtimeSinceStartup - uiBallMoveData.MouseDownTime < 0.5f)
                    {
                        Logic.ToggleCardPackBallSelection(runData, uiBallMoveData.BallIdx);
                        for (int ballIdx = 0; ballIdx < uiBallMoveData.BallTargetPosition.Length; ballIdx++)
                            uiBallMoveData.BallTargetPosition[ballIdx] = getBallStartPosWithSelection(runData, uiBallMoveData, ballIdx, allowSelection);
                    }
                    else
                        uiBallMoveData.BallTargetPosition[uiBallMoveData.BallIdx] = getBallStartPosWithSelection(runData, uiBallMoveData, uiBallMoveData.BallIdx, allowSelection);
                }

                uiBallMoveData.BallIdx = -1;
                uiBallMoveData.MouseMoved = false;
            }
        }

        static Vector3 getBallStartPosWithSelection(RunData runData, UIBallMoveData uiBallMoveData, int ballIdx, bool allowSelection)
        {
            Vector3 startPos = uiBallMoveData.BallStartPosition[ballIdx];
            if (allowSelection && runData.CardPackBallSelected[ballIdx])
                startPos.y += uiBallMoveData.SelectedY;
            return startPos;
        }

        public static void TickMoveBalls(float dt, UIBallMoveData uiBallMoveData)
        {
            for (int i = 0; i < uiBallMoveData.NumBalls; i++)
            {
                Vector3 diff = uiBallMoveData.BallTargetPosition[i] - uiBallMoveData.BallCurrentPosition[i];
                if (diff.magnitude > uiBallMoveData.MinDistance || diff.magnitude > (dt * uiBallMoveData.Velocity))
                    uiBallMoveData.BallCurrentPosition[i] += diff * dt * uiBallMoveData.Velocity;
                else
                    uiBallMoveData.BallCurrentPosition[i] = uiBallMoveData.BallTargetPosition[i];

                uiBallMoveData.BallGO[i].transform.position = uiBallMoveData.BallCurrentPosition[i];
            }
        }

    }
}