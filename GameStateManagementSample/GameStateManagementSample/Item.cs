using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagementSample
{
    public class Item : Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Texture2D Sprite { get; set; }
        public int AttackBonus { get; set; }
        public int ArmorClass { get; set; }
        public int HealthBonus { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string Name { get; set; }
    }
}
