//-----------------------------------------------------------------------
// <copyright file="CameraController.cs" company="COMPANYPLACEHOLDER">
//     Copyright (c) Darius Kinstler. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DPlay.RoguePG.Main.Camera
{
    using DPlay.RoguePG.Extension;
    using UnityEngine;

    /// <summary>
    ///     Will make the Camera that this is attached to follow the referenced GameObject
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        ///     The <seealso cref="Transform"/> of the GameObject to follow
        /// </summary>
        [HideInInspector]
        public Transform following;

        /// <summary> How fast the camera approaches the target position. Lower values increase the speed. </summary>
        [Range(0.0f, 1.0f)]
        public float movementSpeedBase = 0.1f;

        /// <summary> How fast the camera approaches the target rotation. Lower values increase the speed. </summary>
        [Range(0.0f, 1.0f)]
        public float rotationSpeedBase = 0.01f;

        /// <summary> How fast is the manual camera control </summary>
        public float manualControlSpeed = 5.0f;

        /// <summary>
        ///     How far away should the camera be from <see cref="following"/> out of battle
        /// </summary>
        [SerializeField]
        private float preferredDistance = 4.0f;

        /// <summary>
        ///     How much higher should the camera be than the pivot of <see cref="following"/> out of battle
        /// </summary>
        [SerializeField]
        private float preferredHeight = 1.0f;

        /// <summary>
        ///     How far away should the camera be from <see cref="following"/> during a battle
        /// </summary>
        [SerializeField]
        private float preferredDistanceBattle = 4.0f;

        /// <summary>
        ///     How much higher should the camera be than the pivot of <see cref="following"/> during a battle
        /// </summary>
        [SerializeField]
        private float preferredHeightBattle = 1.0f;

        /// <summary> Layer used by entity collision </summary>
        private int opaqueLayerMask;

        /// <summary>
        ///     How far away should the camera be from <see cref="following"/>
        /// </summary>
        public float PreferedDistance
        {
            get
            {
                return BattleManager.IsBattleActive ? this.preferredDistanceBattle : this.preferredDistance;
            }
        }

        /// <summary>
        ///     How much higher should the camera be than the pivot of <see cref="following"/>
        /// </summary>
        public float PreferedHeight
        {
            get
            {
                return BattleManager.IsBattleActive ? this.preferredHeightBattle : this.preferredHeight;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="CameraController"/> whether it is enabled or not.
        /// </summary>
        private void Awake()
        {
            this.opaqueLayerMask = LayerMask.GetMask("Default");
        }

        /// <summary>
        ///     Called by Unity once every frame after all Updates and FixedUpdates have been executed.
        /// </summary>
        private void FixedUpdate()
        {
            if (this.following != null)
            {
                float distance = this.PreferedDistance;

                RaycastHit raycastHit;
                Vector3 rayCastDirection = this.transform.position - this.following.position;
                rayCastDirection.y = 0.0f;

                if (Physics.Raycast(
                    this.following.position,
                    rayCastDirection,
                    out raycastHit,
                    distance,
                    this.opaqueLayerMask))
                {
                    distance = (raycastHit.point - this.following.position).magnitude;
                }

                Vector3 currentRotation = RotationExtension.WrapDegrees(this.transform.eulerAngles);
                this.transform.LookAt(this.following);
                this.transform.position += this.transform.right * Input.GetAxis("CameraHorizontal") * Time.fixedDeltaTime * this.manualControlSpeed;
                Vector3 targetRotation = RotationExtension.WrapDegrees(this.transform.eulerAngles);

                Vector3 thisToFollowing = this.transform.forward;
                thisToFollowing.y = 0.0f;
                thisToFollowing.Normalize();

                Vector3 newPosition = MathExtension.ExponentialLerp(
                    this.transform.position,
                    this.following.position - thisToFollowing * distance,
                    this.movementSpeedBase,
                    Time.fixedDeltaTime);

                newPosition.y = this.following.position.y + this.PreferedHeight;
                this.transform.position = newPosition;

                this.transform.eulerAngles = RotationExtension.ExponentialLerpRotation(
                    currentRotation,
                    targetRotation,
                    this.rotationSpeedBase,
                    Time.fixedDeltaTime);
            }
        }
    }
}