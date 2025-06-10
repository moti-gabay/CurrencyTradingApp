namespace BusinessLayer
{
    public interface ISimulationService
    {
        /// <summary>
        /// מתחיל את סימולציית המסחר במטבעות.
        /// </summary>
        void StartSimulation();

        /// <summary>
        /// עוצר את סימולציית המסחר במטבעות.
        /// </summary>
        void StopSimulation();
    }
}