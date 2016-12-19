using Microsoft.Xna.Framework.Graphics;
using RogueSharp.DiceNotation;
using System.Collections.Generic;

namespace GameStateManagementSample
{
    public class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Texture2D Sprite { get; set; }
        public int AttackBonus { get; set; }
        public int ArmorClass { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string Name { get; set; }
        public LinkedList<Item> Equipment = new LinkedList<Item>();

    }
}
