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
                for (int j = 4; j < 24; j = j + 8) {
                    if (j > 19) {
                        makeHouse(i, j, 5, 6, map, true);
                    } else {
                        makeHouse(i, j, 5, 6, map, false);
                    }

                }
            }

            return map;
        }

        private void makeHouse(int x, int y, int width, int height, T map, bool doorVisible) {
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if (j == height - 1) {
                        if (doorVisible && (j == height - 1) && (i == 2)) {
                            Console.WriteLine("draw door");
                            specialCells.Add(Tuple.Create(x + i, y + j), "city_door");
                        }
                        if (i < 2) {
                            specialCells.Add(Tuple.Create(x + i, y + j - i - 2), "city2_roofleftfront");
                        } else if (i > 2) {
                            specialCells.Add(Tuple.Create(x + i, y + j - (6 - i)), "city2_roofrightfront");
                        } else {
                            specialCells.Add(Tuple.Create(x + i, y + j - 4), "city2_roofcenterfront");
                        }
                    } else if (j == 0) {
                        if (i < 2) {
                            specialCells.Add(Tuple.Create(x + i, y + j - i - 2), "city2_roofleftback");
                        } else if (i > 2) {
                            specialCells.Add(Tuple.Create(x + i, y + j - (6 - i)), "city2_roofrightback");
                        } else {
                            specialCells.Add(Tuple.Create(x + i, y + j - 4), "city2_roofcenterback");
                        }
                    } else {
                        if (i < 2) {
                            specialCells.Add(Tuple.Create(x + i, y + j - i - 2), "city2_roofleft");
                        } else if (i > 2) {
                            specialCells.Add(Tuple.Create(x + i, y + j - (6 - i)), "city2_roofright");
                        } else {
                            specialCells.Add(Tuple.Create(x + i, y + j - 4), "city2_center");
                        }
                    }
                    if ((j == height - 2 || j == height - 1) && (i == 1 || i == 3)) {
                        specialCells.Add(Tuple.Create(x + i, y + j - 1), "city_window");
                    }
                    map.SetCellProperties(x + i, y + j, false, false);
                }
                if (doorVisible) {
                    //specialCells.Add(Tuple.Create(x + 1, y + height - 1), "city_window");
                    //specialCells.Add(Tuple.Create(x + 3, y + height - 1), "city_window");

                }
            }
        }

        private void makeWall(int x, int y, T map) {
            map.SetCellProperties(x, y, false, false);
        }
    }
}
