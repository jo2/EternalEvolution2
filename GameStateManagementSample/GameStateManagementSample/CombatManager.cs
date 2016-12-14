﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStateManagementSample
{
    public class CombatManager
    {
        private readonly Player player;
        private readonly List<Mob> mobs;

        // When we construct the CombatManager class we want to pass in references
        // to the player and the list of enemies.
        public CombatManager(Player lPlayer, List<Mob> lMobs)
        {
            player = lPlayer;
            mobs = lMobs;
        }

        // Use this method to resolve attacks between Figures
        public void Attack(Entity attacker, Entity defender)
        {
            // First create a twenty-sided die
            // Roll the die, add the attack bonus, and compare to the defender's armor class
            if (Global.Random.Next(20) + attacker.AttackBonus >= defender.ArmorClass)
            {
                // Roll damage dice and sum them up
                int damage = Global.Random.Next(20);
                // Lower the defender's health by the amount of damage
                defender.Health -= damage;
                // Write a combat message to the debug log.
                // Later we'll add this to the game UI
                Debug.WriteLine("{0} hit {1} for {2} and he has {3} health remaining.", attacker.Name, defender.Name, damage, defender.Health);
                if (defender.Health <= 0)
                {
                    if (defender is Mob)
                    {
                        var enemy = defender as Mob;
                        // When an enemies health dropped below 0 they died
                        // Remove that enemy from the game
                        mobs.Remove(enemy);
                    }
                    // Later we'll want to display this kill message in the UI
                    Debug.WriteLine("{0} killed {1}", attacker.Name, defender.Name);
                }
            }
            else
            {
                // Show the miss message in the Debug log for now
                Debug.WriteLine("{0} missed {1}", attacker.Name, defender.Name);
            }
        }

        // Helper method which returns the figure at a certain map cell
        public Entity FigureAt(int x, int y)
        {
            if (IsPlayerAt(x, y))
            {
                return player;
            }
            return EnemyAt(x, y);
        }

        // Helper method for checking if the player is at a map cell
        public bool IsPlayerAt(int x, int y)
        {
            return (player.X == x && player.Y == y);
        }

        // Helper method for getting an enemy at a map cell
        public Mob EnemyAt(int x, int y)
        {
            foreach (var enemy in mobs)
            {
                if (enemy.X == x && enemy.Y == y)
                {
                    return enemy;
                }
            }
            return null;
        }

        // Helper method for checking if an enemy is at a map cell
        public bool IsEnemyAt(int x, int y)
        {
            return EnemyAt(x, y) != null;
        }
    }
}