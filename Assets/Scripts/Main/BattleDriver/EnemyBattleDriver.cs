//-----------------------------------------------------------------------
// <copyright file="EnemyBattleDriver.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.BattleDriver
{
    using DPlay.RoguePG.Extension;
    using UnityEngine;

    /// <summary>
    ///     Makes battles work.
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyBattleDriver : BaseBattleDriver
    {
        /// <summary> Whether this enemy is a boss mob </summary>
        [SerializeField]
        private bool isBoss = false;

        /// <summary> How long to wait before taking the next action </summary>
        private float waitTime = 0.0f;

        /// <summary> Current Health Value </summary>
        public override int CurrentHealth
        {
            get
            {
                return this.currentHealth;
            }

            set
            {
                this.currentHealth = Mathf.Clamp(value, 0, this.MaximumHealth);

                if (this.currentHealth == 0 && this.Allies.Contains(this))
                {
                    foreach (BaseBattleDriver battleDriver in this.Opponents)
                    {
                        ++battleDriver.Level;
                    }
                    
                    this.LeaveParty();
                }
            }
        }

        /// <summary>
        ///     To be called when a battle starts
        /// </summary>
        public override void OnBattleStart()
        {
            base.OnBattleStart();

            if (this.isBoss)
            {
                // Boss theme
                MusicManager.PlayMusic("NoFightButCool");
            }
        }

        /// <summary>
        ///     To be called when a battle ends
        /// </summary>
        public override void OnBattleEnd()
        {
            base.OnBattleEnd();

            if (this.isBoss)
            {
                // Return to regular song
                MusicManager.PlayMusic(Dungeon.DungeonGenerator.Design.backgroundMusic);
            }

            if (!this.CanStillFight)
            {
                if (this.isBoss)
                {
                    this.OpenNextFloorEntrance();
                }

                MonoBehaviour.Destroy(this.gameObject);
            }
        }

        /// <summary>
        ///     Sets up everything needed for the Enemy's turn
        /// </summary>
        public override void StartTurn()
        {
            base.StartTurn();

            this.waitTime = 1.0f;
        }

        /// <summary>
        ///     Ends the Enemy's turn
        /// </summary>
        public override void EndTurn()
        {
            base.EndTurn();
        }

        /// <summary>
        ///     Updates the Enemy's turn once a frame
        /// </summary>
        public override void UpdateTurn()
        {
            base.UpdateTurn();

            this.waitTime -= Time.deltaTime;

            if (this.waitTime < 0.0f && !this.IsWaitingOnAnimation)
            {
                // Random moves for now
                if (this.AttackPoints > 0.0f)
                {
                    var action = this.actions.GetRandomItem();

                    var targets = action.GetTargets();

                    action.Use(targets.GetRandomItem());

                    this.waitTime = 1.0f;
                }
                else
                {
                    this.TakingTurn = false;
                }
            }
        }

        /// <summary>
        ///     Leaves the party.
        /// </summary>
        /// <returns>Whether the party was left with the call</returns>
        public override bool LeaveParty()
        {
            if (base.LeaveParty())
            {
                this.gameObject.SetActive(false);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Opens the entrance to the next floor
        /// </summary>
        private void OpenNextFloorEntrance()
        {
            Dungeon.DungeonGenerator.CreateFloorTransition();
        }
    }
}
