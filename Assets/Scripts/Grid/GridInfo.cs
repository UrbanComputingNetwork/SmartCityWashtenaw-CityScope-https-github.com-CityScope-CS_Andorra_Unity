using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridInfo
{
    [Serializable]
    public struct CellData
    {
        public int x;
        public int y;
        public int type;
        public int rotation;
        public int heat;

        public CellData(int x, int y, int type, int rotation, int heat)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.rotation = rotation;
            this.heat = heat;
        }

        public bool Equals(CellData obj)
        {
            return x == obj.x && y == obj.y && type == obj.type && rotation == obj.rotation;
        }

        public override string ToString()
        {
            return string.Format("x,y=({0},{1});type={2};rot={3};heat={4}", x, y, type, rotation, heat);
        }
    }
    public int Slider1;
    public List<int> PopulationInfo;// = new List<int>();
    public List<int> BuildingDensity;// = new List<int>();

    public CellData[,] gridData;

    public GridInfo(int width, int height)
    {
        gridData = new CellData[height, width];
        for (int i = 0; i < gridData.GetLength(0); i++)
        {
            for (int j = 0; j < gridData.GetLength(1); j++)
            {
                gridData[i, j] = new CellData(j, i, -2, 0, 0);
            }
        }
    }
    
    public void UpdateCell(CellData data)
    {
        gridData[data.y, data.x] = data;
    }
}
