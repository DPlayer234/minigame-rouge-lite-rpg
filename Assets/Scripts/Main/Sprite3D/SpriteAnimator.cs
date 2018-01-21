﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAE.RoguePG.Main.Sprite3D
{
    /// <summary>
    ///     Allows for animating sprites using a <see cref="SpriteManager"/>
    /// </summary>
    [RequireComponent(typeof(SpriteManager))]
    public class SpriteAnimator : MonoBehaviour
    {
        /// <summary> How far the current animation state has progressed </summary>
        private float progress;
        
        /// <summary> The current animation information </summary>
        private SpriteAnimationStatus endStatus;
        
        /// <summary> Reference information for interpolation </summary>
        private SpriteAnimationStatus startStatus;

        /// <summary> The <seealso cref="SpriteManager"/> also attached to this <seealso cref="GameObject"/> </summary>
        private SpriteManager spriteManager;

        /// <summary> The active animation coroutine </summary>
        private Coroutine animationCoroutine;

        /// <summary> [ Use Field <see cref="Animation"/> ] </summary>
        new private SpriteAnimation animation;

        /// <summary> Set or get the Animation delegate </summary>
        public SpriteAnimation Animation
        {
            set
            {
                if (this.animationCoroutine != null)
                {
                    StopCoroutine(this.animationCoroutine);
                }

                if (value != null)
                {
                    this.animationCoroutine = StartCoroutine(value(this.SetAnimationTarget));
                }

                this.animation = value;
            }

            get
            {
                return this.animation;
            }
        }

        /// <summary>
        ///     Sets the <seealso cref="SpriteAnimationStatus"/>.
        /// </summary>
        /// <param name="status">The new status to use</param>
        private void SetAnimationTarget(SpriteAnimationStatus status)
        {
            this.progress = 0.0f;
            this.endStatus = status;

            float[] currentRotations = new float[this.spriteManager.animatedTransforms.Length];
            for (int i = 0; i < this.spriteManager.animatedTransforms.Length; i++)
            {
                currentRotations[i] = VariousCommon.WrapDegrees(this.spriteManager.animatedTransforms[i].localEulerAngles.z);
            }

            this.startStatus = new SpriteAnimationStatus(
                0.0f,
                this.spriteManager.bodyTransform.localPosition,
                currentRotations);
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="SpriteAnimator"/> whether it is or is not active.
        /// </summary>
        private void Awake()
        {
            this.spriteManager = this.GetComponent<SpriteManager>();
        }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="SpriteAnimator"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            // Might need this later
        }

        /// <summary>
        ///     Called by Unity every frame to update the <see cref="SpriteAnimator"/>
        /// </summary>
        private void Update()
        {
            this.UpdateAnimation();
        }

        /// <summary>
        ///     Updates the current animation if there is one.
        /// </summary>
        private void UpdateAnimation()
        {
            if (this.endStatus != null)
            {
                this.progress += Time.deltaTime * this.endStatus.speed;

#if UNITY_EDITOR
                if (this.endStatus.rotations.Length != this.startStatus.rotations.Length)
                    Debug.LogWarning("The Amount of Sprites and Rotations does not match up.");
#endif

                // Move Body
                this.spriteManager.bodyTransform.localPosition =
                    VariousCommon.SmootherStep(this.startStatus.position, this.endStatus.position, this.progress);

                // Rotate Absolutely Everything Else
                int length = Mathf.Min(this.endStatus.rotations.Length, this.startStatus.rotations.Length);
                Vector3 localEulerAngles;
                for (int i = 0; i < length; i++)
                {
                    localEulerAngles = this.spriteManager.animatedTransforms[i].localEulerAngles;

                    this.spriteManager.animatedTransforms[i].localEulerAngles = new Vector3(
                        localEulerAngles.x,
                        localEulerAngles.y,
                        VariousCommon.SmootherStep(this.startStatus.rotations[i], this.endStatus.rotations[i], this.progress));
                }
            }
        }

        /// <summary>
        ///     Allows animating a sprite by calling the <seealso cref="StatusSetter"/> with the new <seealso cref="SpriteAnimationStatus"/>.
        ///     It's run as a <see cref="Coroutine"/>, just in case the return type didn't make that obvious enough.
        /// </summary>
        public delegate IEnumerator SpriteAnimation(StatusSetter statusSetter);

        /// <summary>
        ///     Used to set information in a <see cref="SpriteAnimator"/>
        /// </summary>
        public delegate void StatusSetter(SpriteAnimationStatus status);
    }
}