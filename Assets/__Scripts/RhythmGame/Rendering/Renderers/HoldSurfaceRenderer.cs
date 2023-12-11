using System.Collections.Generic;
using SaturnGame.RhythmGame;
using Unity.VisualScripting;
using UnityEngine;

namespace SaturnGame.Rendering
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [AddComponentMenu("SaturnGame/Rendering/Hold Surface Renderer")]
    public class HoldSurfaceRenderer : MonoBehaviour
    {
        private const float tunnelRadius = 1.72f; // 1.75 by default.
        private const float tunnelLength = -6f;

        // ==== MESH ====
        [SerializeField] private Material materialTemplate;
        private Material materialInstance;

        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        [SerializeField] private Mesh holdMesh;
        private List<Vector3> vertList = new();
        private List<Vector2> uvList = new();
        private int[] triangles;

        private int ColorID;
        public HoldNote holdNote;

        public bool reverse;

        void Awake()
        {
            materialInstance = new(materialTemplate);
            holdMesh = new();
            meshFilter.mesh = holdMesh;
        }

        public void SetRenderer(HoldNote hold)
        {
            ColorID = NoteColors.GetColorID(hold.Start.NoteType);
            holdNote = hold;

            if (materialInstance.HasFloat("_ColorID"))
                materialInstance.SetFloat("_ColorID", ColorID);

            meshRenderer.material = materialInstance;
            meshFilter.mesh = holdMesh;
        }

        /// <summary>
        /// Generates a Hold Note Mesh.
        /// This still needs A LOT of optimizing/cleanup!!!
        /// Note to self: Reverse doesn't work because scaledVisualTime of reversed holds is reverse order.
        /// </summary>
        public void GenerateMesh(float scrollDuration)
        {
            float holdStartTime = holdNote.Start.ScaledVisualTime;

            int holdWidth = holdNote.MaxSize;
            int holdLength = holdNote.RenderedNotes.Length;
            int trueLength = 0; // To keep track of all sub-segments as well

            int vertexID = 0;

            vertList.Clear();
            uvList.Clear();

            // For every RenderedNote
            for (int y = 0; y < holdLength; y++)
            {
                Note startNote = holdNote.RenderedNotes[y];
                int startNoteSize = startNote.Size;
                int startNotePos = startNote.Position;

                int endNoteSize = startNoteSize;
                int endNotePos = startNotePos;

                float start = startNote.ScaledVisualTime;
                float end = start + 1;
                float interval = 20;

                if (y != holdLength - 1)
                {
                    Note endNote = holdNote.RenderedNotes[y + 1];
                    endNoteSize = endNote.Size;
                    endNotePos = endNote.Position;
                    end = endNote.ScaledVisualTime;

                    // Hold is straight and doesn't need sub-segments.
                    // Set interval to the whole distance from start to end.
                    if (startNoteSize == endNoteSize && startNotePos == endNotePos)
                        interval = endNote.ScaledVisualTime - startNote.ScaledVisualTime;
                }

                // For every sub-segment between RenderedNotes.
                for (float i = start; i < end; i += interval)
                {
                    float progress = Mathf.InverseLerp(start, end, i);

                    float noteSize = Mathf.Lerp(startNoteSize, endNoteSize, progress);
                    float notePos = SaturnMath.LerpRound(startNotePos, endNotePos, progress, 60);

                    // Shrink hold sizes to fit note mesh
                    if (noteSize < 60)
                    {
                        noteSize -= 1.6f;
                        notePos += 0.8f;
                    }

                    float sizeMultiplier = GetAngleInterval(noteSize, holdWidth);
                    float depth = SaturnMath.InverseLerp(0, scrollDuration, i - holdStartTime);

                    // Generate an arc of verts
                    for (int x = 0; x <= holdWidth; x++)
                    {
                        float currentAngle = (sizeMultiplier * x + notePos) * 6;

                        vertList.Add(GetPointOnCylinder(Vector2.zero, tunnelRadius, tunnelLength, currentAngle, depth));
                        uvList.Add(GetUV(x, holdWidth, y + progress, holdLength));
                        vertexID++;
                    }

                    // We've generated one segment. Increment for triangle gen.
                    trueLength++;
                }
            }

            triangles = new int[holdWidth * trueLength * 6]; 

            int vert = 0;
            int tris = 0;

            for (int y = 0; y < (trueLength - 1); y++)
            {
                for (int x = 0; x < holdWidth; x++)
                {
                    // Draw triangles counterclockwise to flip normals
                    triangles[tris + 2] = vert;
                    triangles[tris + 1] = vert + 1;
                    triangles[tris + 0] = vert + holdWidth + 1;

                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + holdWidth + 1;
                    triangles[tris + 5] = vert + holdWidth + 2;

                    vert ++;
                    tris += 6;
                }

                vert++;
            }

            holdMesh.Clear();

            holdMesh.vertices = vertList.ToArray();
            holdMesh.uv = uvList.ToArray();
            holdMesh.triangles = triangles;
        }

        Vector3 GetPointOnCylinder(Vector2 centerPoint, float coneRadius, float coneLength, float angle, float depth)
        {
            angle = 180 - angle;
            
            //if (reverse) depth = depth;

            float x = coneRadius * Mathf.Cos(Mathf.Deg2Rad * angle) + centerPoint.x;
            float y = coneRadius * Mathf.Sin(Mathf.Deg2Rad * angle) + centerPoint.y;
            float z = coneLength * depth;

            return new Vector3 (x, y, z);
        }

        Vector2 GetUV(float x, float noteSize, float y, float depth)
        {
            float u = Mathf.InverseLerp(0, noteSize, x);
            float v = Mathf.InverseLerp(0, depth, y);
            return new Vector2 (u,v);
        }

        float GetAngleInterval(float currentNoteSize, int maxNoteSize)
        {
            return (float) currentNoteSize / maxNoteSize;
        }
    

        [SerializeField] private int debugGizmos = 0;
        void OnDrawGizmos()
        {
            for (int i = 0; i < debugGizmos; i++)
            {
                Gizmos.DrawSphere(holdMesh.vertices[i], 0.1f);
            }
        }
    }
}
