using System.Collections.Generic;
using SaturnGame.RhythmGame;
using UnityEngine;

namespace SaturnGame.Rendering
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [AddComponentMenu("SaturnGame/Rendering/Swipe Renderer")]
    public class SwipeRenderer : MonoBehaviour
    {
        // ==== MESH ====
        [SerializeField] private List<Mesh> meshes;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material materialTemplate;
        private Material materialInstance;

        // ==== NOTE INFO ====
        public int Size { get; private set; }
        public int Position { get; private set; }

        public Color Color { get; private set; }
        public string Direction { get; private set; } = "_COUNTERCLOCKWISE";

        void Awake()
        {
            materialInstance = new(materialTemplate);
        }

        public void SetRendererProperties(Note note)
        {
            Size = note.Size;
            Position = note.Position;

            int colorID = NoteColors.GetColorID(note.NoteType);
            Color = NoteColors.GetSwipeColor(colorID);

            bool dir = note.NoteType is ObjectEnums.NoteType.SwipeCounterclockwise;
            Direction = dir ? "_COUNTERCLOCKWISE" : "_CLOCKWISE";
        }

        public void UpdateRenderer()
        {
            if (materialInstance.HasColor("_NoteColor"))
                materialInstance.SetColor("_NoteColor", Color);

            materialInstance.DisableKeyword("_DIRECTION_COUNTERCLOCKWISE");
            materialInstance.EnableKeyword("_DIRECTION" + Direction);

            meshFilter.mesh = meshes[Size - 1];
            meshRenderer.material = materialInstance;

            transform.eulerAngles = new Vector3 (0, 0, Position * -6);
        }
    }
}
