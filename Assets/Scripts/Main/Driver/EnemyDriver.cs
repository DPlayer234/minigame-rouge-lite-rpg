﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAE.RoguePG.Dev;
using SAE.RoguePG.Main.Sprite3D;
using SAE.RoguePG.Main.BattleDriver;

namespace SAE.RoguePG.Main.Driver
{
    /// <summary>
    ///     Makes Enemies work.
    /// </summary>
    [RequireComponent(typeof(EnemyBattleDriver))]
    [DisallowMultipleComponent]
    public class EnemyDriver : BaseDriver
    {
        /// <summary> How fast the enemy can move </summary>
        public float movementSpeed = 1.0f;

        /// <summary> How far away it will detect a player </summary>
        public float targetRange = 4.0f;

        /// <summary> Whether this enemy was defeated </summary>
        [HideInInspector]
        public bool defeated;

        /// <summary> The player this enemy is currently chasing. If null, just walks around aimlessly. </summary>
        private PlayerDriver targetPlayer;

        /// <summary> The delay in seconds between checks for the target </summary>
        private const float TargetUpdateDelay = 0.2f;

        /// <summary> The maximum distance between a player and this entity to trigger a battle </summary>
        private const float BattleTriggerRange = 0.5f;

        /// <summary>
        ///     Finds an appropriate target
        /// </summary>
        private void LookForTarget()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(StateManager.PlayerEntityTag);

            foreach (GameObject player in players)
            {
                PlayerDriver playerDriver = player.GetComponent<PlayerDriver>();

                if (playerDriver != null && playerDriver.IsLeader && this.CanSee(player))
                {
                    this.targetPlayer = playerDriver;
                    break;
                }
            }
        }

        /// <summary>
        ///     Returns whether this enemy can see the specified <seealso cref="GameObject"/>
        /// </summary>
        /// <param name="gameObject">The <seealso cref="GameObject"/> to check</param>
        /// <returns>Whether it's visible.</returns>
        private bool CanSee(GameObject gameObject)
        {
            Vector3 positionDifference = gameObject.transform.position - this.transform.position;

            if (positionDifference.sqrMagnitude < this.targetRange * this.targetRange)
            {
                RaycastHit raycastHit;

                return Physics.Raycast(this.transform.position, positionDifference, out raycastHit, this.targetRange) && raycastHit.transform.gameObject == gameObject;
            }

            return false;
        }

        /// <summary>
        ///     Started as a coroutine to update the target
        /// </summary>
        /// <returns>An iterator</returns>
        private IEnumerator UpdateTarget()
        {
            do
            {
                if (this.targetPlayer == null)
                {
                    if (Random.value < 0.2f)
                    {
                        this.spriteManager.FlipToDirection(!this.spriteManager.IsFacingRight);
                    }

                    this.LookForTarget();
                }
                else if (!this.CanSee(this.targetPlayer.gameObject))
                {
                    this.targetPlayer = null;
                }

                yield return new WaitForSeconds(TargetUpdateDelay);
            } while (!this.defeated);
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyDriver"/> whether it is or is not active.
        /// </summary>
        new private void Awake()
        {
            base.Awake();

            this.defeated = false;
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="EnemyDriver"/> when it first becomes active
        /// </summary>
        new private void Start()
        {
            base.Start();

            StartCoroutine(this.UpdateTarget());
        }

        /// <summary>
        ///     Called by Unity for every physics update to update the <see cref="EnemyDriver"/>
        /// </summary>
        new private void FixedUpdate()
        {
            Vector2 movement = (
                // Has target
                this.targetPlayer != null ?
                new Vector2(
                    this.targetPlayer.transform.position.x - this.transform.position.x,
                    this.targetPlayer.transform.position.z - this.transform.position.z) :

                // Has no target
                Vector2.zero).normalized * movementSpeed;

            Vector3 velocity = this.rigidbody.velocity;

            this.rigidbody.velocity = new Vector3(
                VariousCommon.ExponentialLerp(velocity.x, movement.x, 0.01f, Time.deltaTime),
                velocity.y,
                VariousCommon.ExponentialLerp(velocity.z, movement.y, 0.01f, Time.deltaTime));

            // Start a battle if close enough
            if (this.targetPlayer != null && (this.targetPlayer.transform.position - this.transform.position).sqrMagnitude < BattleTriggerRange * BattleTriggerRange)
            {
                this.LogThisAndFormat("{0}; {1}", this.targetPlayer.battleDriver, this.targetPlayer.battleDriver as PlayerBattleDriver);
                this.LogThisAndFormat("{0}; {1}", this.battleDriver, this.battleDriver as EnemyBattleDriver);

                StateManager.StartBattleMode(this.targetPlayer.battleDriver as PlayerBattleDriver, this.battleDriver as EnemyBattleDriver);
            }

            base.FixedUpdate();
        }
    }
}