using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStateManagementSample
{
    public enum GameStates
    {
        None = 0,
        PlayerTurn = 1,
        EnemyTurn = 2,
        Debugging = 3,
        GameOver = 4
    }

    public class Global
    {
        public static readonly IRandom Random = new DotNetRandom();
        public static GameStates GameState { get; set; }
        public static CombatManager CombatManager;
    }
}
