using UnityEngine;

public class ColliderInputReciever : InputReciever
{
    [SerializeField] private Camera mainCamera;

    public override void RecieveInput(ChessGameController gameController)
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitPoint;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitPoint))
            {
                foreach (var reader in inputReaders)
                    reader.ReadInput(hitPoint.point, gameController);
            }
        }
    }
}