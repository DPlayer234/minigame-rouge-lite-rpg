﻿namespace SAE.RoguePG.Main.BattleAction.Actions
{
    using SAE.RoguePG.Main.BattleDriver;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    /// <summary>
    ///     Simple physical attack
    /// </summary>
    public class Smash : ChargeAction
    {
        /// <summary> Action Name </summary>
        public const string ActionName = "Smash";

        /// <summary>
        ///     Initializes a new instance of the <see cref="Smash"/> class
        /// </summary>
        /// <param name="user">The BattleDriver which will use this action</param>
        public Smash(BaseBattleDriver user) : base(user)
        {
            this.name = ActionName;

            this.attackPointCost = 4.0f;
            this.attackPower = 10.0f;
            this.category = ActionCategory.PhysicalAttack;
            this.targetOption = ActionTargetOption.OneOpponent;
        }
    }
}