using UnityEngine;

namespace BlackSheeps
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Horde _gridArea;

        private void Update()
        {
            _gridArea.RefreshPosition();
        }
    }
}