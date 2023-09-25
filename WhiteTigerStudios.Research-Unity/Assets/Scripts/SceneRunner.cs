// Copyright (c) 2023 - WhitetigerStudios
using System;
using UnityEngine;

namespace WhiteTigerStudios.Research
{
    /// <summary>
    /// Basic scene running management, including a label
    /// </summary>
    public class SceneRunner : MonoBehaviour
    {
        private static Rect RectAtRow(int index)
        {
            return new Rect((Screen.width - WidthForRow) / 2, (index + 1) * HeightBetweenRows, WidthForRow, HeightForRow);
        }

        private static readonly int HeightForRow = Screen.height / 15;
        private static readonly int HeightForRowSpace = 10;
        private static readonly int HeightBetweenRows = HeightForRow + HeightForRowSpace;
        private static readonly int WidthForRow = Screen.width / 2;

        void OnGUI()
        {
            int nextRow = 0;
            GUI.Label(RectAtRow(nextRow++), "WhiteTigerStudios.Research");

            try
            {
//TODO: Do anything that needs doing
            }
            catch (Exception ex)
            {
                GUI.Label(RectAtRow(nextRow++), ex.ToString());
            }
        }
    }
}
