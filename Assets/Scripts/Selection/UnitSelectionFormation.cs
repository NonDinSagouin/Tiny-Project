using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

using TinyProject.Entities;

namespace TinyProject.Selection
{
    public enum FormationType
    {
        VerticalLine,
        HorizontalLine,
        Square,
        Circle
    }

    public class UnitSelectionFormation : MonoBehaviour
    {
        [SerializeField] private int unitsPerLine = 5;

        private Camera cam;
    
        private void Start()
        {
            cam = Camera.main;
        }

        void Update()
        {
        }

        /// <summary>
        /// Arrange les unités sélectionnées en formation carrée autour de la position de la souris.
        /// </summary>
        /// <param name="mouseWorldPosition">La position du monde de la souris.</param>
        /// <param name="unitesSelected">La liste des unités sélectionnées.</param>
        public void SquareFormation(Vector3 mouseWorldPosition, List<Unit> unitesSelected)
        {
            if (unitesSelected.Count == 0) return;

            // Calculer la taille de la grille (arrondir à la racine carrée supérieure)
            int gridSize = Mathf.CeilToInt(Mathf.Sqrt(unitesSelected.Count));
            
            // Calculer l'offset pour centrer la formation
            float offset = (gridSize - 1) / 2.0f;
            
            List<Vector3> targetPositions = new List<Vector3>();

            // Générer les positions de la formation carrée centrée
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    targetPositions.Add(mouseWorldPosition + new Vector3(x - offset, y - offset, 0));
                }
            }

            int targetIndex = 0;

            foreach (Unit unit in unitesSelected)
            {
                unit.MoveTo(targetPositions[targetIndex]);
                targetIndex++;
                if (targetIndex >= targetPositions.Count)
                    targetIndex = 0;
            }
        }

        /// <summary>
        /// Arrange les unités sélectionnées en formation circulaire autour de la position de la souris.
        /// </summary>
        /// <param name="mouseWorldPosition">La position du monde de la souris.</param>
        /// <param name="unitesSelected">La liste des unités sélectionnées.</param>
        public void CircleFormation(Vector3 mouseWorldPosition, List<Unit> unitesSelected)
        {
            if (unitesSelected.Count == 0) return;

            float radius = Mathf.Max(1.5f, unitesSelected.Count / (2 * Mathf.PI));
            List<Vector3> targetPositions = new List<Vector3>();

            // Générer les positions de la formation circulaire centrée
            for (int i = 0; i < unitesSelected.Count; i++)
            {
                float angle = (i / (float)unitesSelected.Count) * 2 * Mathf.PI;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                targetPositions.Add(mouseWorldPosition + new Vector3(x, y, 0));
            }

            int targetIndex = 0;

            foreach (Unit unit in unitesSelected)
            {
                unit.MoveTo(targetPositions[targetIndex]);
                targetIndex++;
            }
        }

        /// <summary>
        /// Arrange les unités sélectionnées en formation linéaire autour de la position de la souris.
        /// </summary>
        /// <param name="mouseWorldPosition">La position du monde de la souris.</param>
        /// <param name="unitesSelected">La liste des unités sélectionnées.</param>
        /// <param name="isHorizontal">Si true, formation horizontale (lignes). Si false, formation verticale (colonnes).</param>
        public void LineFormation(Vector3 mouseWorldPosition, List<Unit> unitesSelected, bool isHorizontal)
        {
            if (unitesSelected.Count == 0) return;

            List<Vector3> targetPositions = new List<Vector3>();

            // Calculer le nombre de lignes/colonnes
            int numLines = Mathf.CeilToInt(unitesSelected.Count / (float)unitsPerLine);
            
            // Calculer l'offset pour centrer la formation
            float lineOffset = (numLines - 1) / 2.0f;

            int unitIndex = 0;

            // Générer les positions en ligne, centrées
            for (int line = 0; line < numLines; line++)
            {
                int unitsInThisLine = Mathf.Min(unitsPerLine, unitesSelected.Count - unitIndex);
                float currentUnitOffset = (unitsInThisLine - 1) / 2.0f;

                for (int pos = 0; pos < unitsInThisLine; pos++)
                {
                    if (isHorizontal)
                    {
                        // Formation horizontale : lignes de gauche à droite
                        targetPositions.Add(mouseWorldPosition + new Vector3(pos - currentUnitOffset, line - lineOffset, 0));
                    }
                    else
                    {
                        // Formation verticale : colonnes de haut en bas
                        targetPositions.Add(mouseWorldPosition + new Vector3(line - lineOffset, pos - currentUnitOffset, 0));
                    }
                }

                unitIndex += unitsInThisLine;
            }

            int targetIndex = 0;

            foreach (Unit unit in unitesSelected)
            {
                unit.MoveTo(targetPositions[targetIndex]);
                targetIndex++;
            }
        }

        /// <summary>
        /// Déplace une unité sélectionnée vers la position de la souris sans formation spécifique.
        /// </summary>
        /// <param name="mouseWorldPosition">La position du monde de la souris.</param>
        /// <param name="unit">L'unité sélectionnée.</param>
        public void NoFormation(Vector3 mouseWorldPosition, Unit unit)
        {
            if (unit == null) return;
            unit.MoveTo(mouseWorldPosition);
        }
    }
}