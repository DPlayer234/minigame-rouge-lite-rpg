﻿namespace SAE.RoguePG.Main
{
    using SAE.RoguePG.Main.UI;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Stores static references to generic prefabs.
    ///     Keep in mind, that this behaviour will destroy the GameObject it is attached to.
    /// </summary>
    public class GenericPrefab : MonoBehaviour
    {
        /// <summary> Prefab for player health bars </summary>
        [SerializeField]
        private GameObject statusDisplay = null;

        /// <summary> Prefab for an empty panel </summary>
        [SerializeField]
        private GameObject panel = null;

        /// <summary> Prefab for buttons </summary>
        [SerializeField]
        private Button button = null;

        /// <summary> Prefab for world buttons </summary>
        [SerializeField]
        private ButtonController worldButton = null;

        /// <summary> Prefab for text </summary>
        [SerializeField]
        private Text text = null;

        /// <summary> Prefab for 3D Text </summary>
        [SerializeField]
        private Text3DController text3D = null;

        /// <summary> Prefab for player health bars </summary>
        public static GameObject StatusDisplay { get; private set; }

        /// <summary> Prefab for an empty panel </summary>
        public static GameObject Panel { get; private set; }

        /// <summary> Prefab for buttons </summary>
        public static Button Button { get; private set; }

        /// <summary> Prefab for world buttons </summary>
        public static ButtonController WorldButton { get; private set; }

        /// <summary> Prefab for text </summary>
        public static Text Text { get; private set; }

        /// <summary> Prefab for 3D Text </summary>
        public static Text3DController Text3D { get; private set; }

        /// <summary>
        ///     Called by Unity to initialize the <seealso cref="GenericPrefab"/> whether it is active or not
        /// </summary>
        private void Awake()
        {
            GenericPrefab.StatusDisplay = this.statusDisplay;
            GenericPrefab.Panel = this.panel;
            GenericPrefab.Button = this.button;
            GenericPrefab.WorldButton = this.worldButton;
            GenericPrefab.Text = this.text;
            GenericPrefab.Text3D = this.text3D;

            MonoBehaviour.Destroy(this.gameObject);
        }
    }
}
