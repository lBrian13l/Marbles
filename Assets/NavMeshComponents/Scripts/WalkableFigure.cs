namespace UnityEngine.AI
{
    public abstract class WalkableFigure : MonoBehaviour
    {
        [SerializeField] protected NavMeshLink[] _links;
        [SerializeField] protected MeshRenderer[] _meshes;

        public abstract void SetupLinks();

        public void ShowMeshes()
        {
            foreach (MeshRenderer mesh in _meshes)
            {
                mesh.enabled = true;
            }
        }

        public void HideMeshes()
        {
            foreach (MeshRenderer mesh in _meshes)
            {
                mesh.enabled = false;
            }
        }
    }
}
