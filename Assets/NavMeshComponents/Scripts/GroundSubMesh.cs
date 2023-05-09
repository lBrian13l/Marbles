namespace UnityEngine.AI
{
    public class GroundSubMesh : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private bool _isRight;

        private float _defaultWidth = 10f;
        private float _additionalLength = 9f;

        public void SetupMesh(Vector3 rampScale)
        {
            _renderer.enabled = true;
            transform.localScale = new Vector3(_defaultWidth / rampScale.x, transform.localScale.y, 0.5f + _additionalLength / rampScale.z);

            if (_isRight)
            {
                transform.localPosition = new Vector3(0.5f + transform.localScale.x / 2, transform.localPosition.y, transform.localPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(-0.5f - transform.localScale.x / 2, transform.localPosition.y, transform.localPosition.z);
            }
        }

        public void HideMesh()
        {
            _renderer.enabled = false;
        }
    }
}
