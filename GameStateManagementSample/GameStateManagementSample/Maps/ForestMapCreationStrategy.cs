using RogueSharp;
using RogueSharp.MapCreation;
using System;
using System.Collections.Generic;

namespace GameStateManagementSample.Maps {
    class CityMapCreationStrategy<T> : IMapCreationStrategy<T> where T : IMap, new() {

        private int width;
        private int height;

        public Dictionary<Tuple<int, int>, string> specialCells;

        public CityMapCreationStrategy(int lWidth, int lHeight) {
            width = lWidth;
            height = lHeight;
            specialCells = new Dictionary<Tuple<int, int>, string>();
        }

        public T CreateMap() {
            T map = new T();

            map.Initialize(width, height);
            map.Clear(true, true);

            foreach (Cell cell in map.GetCellsInRows(0, height - 1)) {
                makeWall(cell.X, cell.Y, map);
            }

            foreach (Cell cell in map.GetCellsInColumns(0, width - 1)) {
                makeWall(cell.X, cell.Y, map);
            }

            for (int i = 5; i < 42; i = i + 7) {
                for (int j = 3; j < 20; j = j + 8) {
                    makeHouse(i, j, 5, 6, map);
                }
            }

            return map;
        }

        private void makeHouse(int x, int y, int width, int height, T map) {
            //specialCells.Add(Tuple.Create(x, y), "city_roofcenter"); 
            //map.SetCellProperties(x, y, true, false); 
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if (j == height - 1) {
                        if (i < 1) {
                            specialCells.Add(Tuple.Create(x + i, y + j), "city_rooffrontleft");
                        } else if (i > 3) {
                            specialCells.Add(Tuple.Create(x + i, y + j), "city_rooffrontright");
                        } else {
                            specialCells.Add(Tuple.Create(x + i, y + j), "city_rooffrontcenter");
                        }
                    } else {
                        if (i < 1) {
                            specialCells.Add(Tuple.Create(x + i, y + j), "city_roofleft");
                        } else if (i > 3) {
                            specialCells.Add(Tuple.Create(x + i, y + j), "city_roofright");
                        } else {
                            specialCells.Add(Tuple.Create(x + i, y + j), "city_roofcenter");
                        }
                    }
                    map.SetCellProperties(x + i, y + j, true, false);
                }
            }
        }

        private void makeWall(int x, int y, T map) {
            map.SetCellProperties(x, y, false, false);
        }
    }
}
