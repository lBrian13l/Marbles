namespace UnityEngine.AI
{
    public class WalkableRamp : WalkableFigure
    {
        [SerializeField] private GroundSubMesh[] _groundMeshes;
        [SerializeField] private NavMeshSurface _subSurface;

        private float _defaultMeshWidth = 4f;
        private float _startPointStep = 0.025f;

        public override void SetupLinks()
        {
            for (int i = 0; i < _links.Length && i < _meshes.Length; i++)
            {
                NavMeshLink link = _links[i];
                Transform linkTransform = _links[i].transform;
                Transform meshTransform = _meshes[i].transform;
                float scaleX = transform.lossyScale.x;
                float scaleY = transform.lossyScale.y;
                float scaleZ = transform.lossyScale.z;
                Vector3 linkRotation = linkTransform.localRotation.eulerAngles;

                if (i % 2 == 0)
                {
                    meshTransform.localPosition = new Vector3(-0.5f - meshTransform.localScale.x / 2, meshTransform.localPosition.y, meshTransform.localPosition.z);
                    linkRotation = new Vector3(linkRotation.x, linkRotation.y, -45 + Mathf.Rad2Deg * Mathf.Atan2(scaleY, scaleZ));
                }
                else
                {
                    meshTransform.localPosition = new Vector3(0.5f + meshTransform.localScale.x / 2, meshTransform.localPosition.y, meshTransform.localPosition.z);
                    linkRotation = new Vector3(linkRotation.x, linkRotation.y, 45 - Mathf.Rad2Deg * Mathf.Atan2(scaleY, scaleZ));
                }

                meshTransform.localScale = new Vector3(_defaultMeshWidth / scaleX, meshTransform.localScale.y, meshTransform.localScale.z);
                linkTransform.localRotation = Quaternion.Euler(linkRotation);
                link.startPoint = new Vector3(link.startPoint.x, scaleY * _startPointStep + scaleY * 0.5f * (scaleZ - scaleY) * _startPointStep / scaleZ, link.startPoint.z);
                link.endPoint = new Vector3(link.endPoint.x, -scaleY, link.endPoint.z);
                link.width = scaleZ;
            }

            SetupGroundMeshes();
        }

        private void SetupGroundMeshes()
        {
            foreach (GroundSubMesh groundMesh in _groundMeshes)
            {
                groundMesh.SetupMesh(transform.lossyScale);
            }

            _subSurface.BuildNavMesh();

            foreach (GroundSubMesh groundMesh in _groundMeshes)
            {
                groundMesh.HideMesh();
            }
        }
    }
}
