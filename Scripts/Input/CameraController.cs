using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform boardEdgeFirst;
    [SerializeField] private Transform boardEdgeSecond;
    [SerializeField] private Transform boardCenter;
    [SerializeField] private Transform startPosition;

    [SerializeField] private GameObject firstPlayer;
    [SerializeField] private GameObject secondPlayer;

    [SerializeField] private float zoomSpeed;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxRotation;
    [SerializeField] private float minRotation;

    private float localRotationX;
    private Vector3 initialPosition;
    private GameObject currentPlayer;
    private Transform boardEdge;
    private bool disableCam;
    private bool disableRotationToNextPlayer;

    private void Awake()
    {
        DisableCam();
    }

    public void ResetCamera()
    {
        InitializePosition();
        localRotationX = mainCamera.transform.eulerAngles.x;
        StopAllCoroutines();
    }

    private void InitializePosition()
    {
        mainCamera.transform.position = firstPlayer.transform.position;
        mainCamera.transform.rotation = Quaternion.Euler(firstPlayer.transform.eulerAngles.x, 0.0f, firstPlayer.transform.eulerAngles.z);
        boardEdge = boardEdgeFirst;
        currentPlayer = firstPlayer;

        if(!ChessGameSettings.HumanPlayerIsWhite && ChessGameSettings.WhitePlayerUsesAi)
        {
            mainCamera.transform.position = secondPlayer.transform.position;
            mainCamera.transform.rotation = Quaternion.Euler(secondPlayer.transform.eulerAngles.x, 180.0f, secondPlayer.transform.eulerAngles.z);
            boardEdge = boardEdgeSecond;
            currentPlayer = secondPlayer;
            zoomSpeed *= -1;
        }

        initialPosition = mainCamera.transform.position;
        disableRotationToNextPlayer = (ChessGameSettings.WhitePlayerUsesAi || ChessGameSettings.BlackPlayerUsesAi) ? true : false;
    }

    private void Update()
    {
        Zoom();
        CameraRotateX();
    }

    private void Zoom()
    {
        if (disableCam)
            return;

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");

        if (zoomInput != 0)
        {
            float z = mainCamera.transform.position.z + zoomInput * zoomSpeed * Time.deltaTime;
            
            if (currentPlayer == firstPlayer)
            {
                z = Mathf.Clamp(z, initialPosition.z, boardEdge.position.z);
            }
            else
            {
                z = Mathf.Clamp(z, boardEdge.position.z, initialPosition.z);
            }

            mainCamera.transform.position = Vector3.right * mainCamera.transform.position.x + Vector3.up * mainCamera.transform.position.y
                + Vector3.forward * z;
        }
    }

    private void CameraRotateX()
    {
        if (disableCam)
            return;

        if (Input.GetMouseButton(1))
        {
            float mouseY = Input.GetAxis("Mouse Y");
            localRotationX += mouseY * -mouseSensitivity;
            localRotationX = Mathf.Clamp(localRotationX, minRotation, maxRotation);
            Quaternion QT = Quaternion.Euler(localRotationX, mainCamera.transform.eulerAngles.y, 0.0f);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, QT, Time.deltaTime * rotationSpeed);
        }
    }

    public IEnumerator RotateToAnotherPlayer(Action callback)
    {
        if (disableRotationToNextPlayer)
        {
            callback?.Invoke();
            yield break;
        }

        DisableCam();

        if (currentPlayer == firstPlayer)
        {
            currentPlayer = secondPlayer;
            boardEdge = boardEdgeSecond;
        }
        else
        {
            currentPlayer = firstPlayer;
            boardEdge = boardEdgeFirst;
        }

        Vector3 boardCenterPosition = boardCenter.transform.position;

        while (Vector3.Distance(mainCamera.transform.position, initialPosition) > 0.01f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, initialPosition, Time.deltaTime * 30.0f);
            yield return null;
        }

        mainCamera.transform.position = initialPosition;

        while (Vector3.Distance(mainCamera.transform.position, currentPlayer.transform.position) > 0.5f)
        {
            mainCamera.transform.RotateAround(boardCenterPosition, Vector3.up, Time.deltaTime * 160.0f);
            yield return null;
        }

        mainCamera.transform.position = currentPlayer.transform.position;
        initialPosition = currentPlayer.transform.position;

        Quaternion QT = Quaternion.Euler(mainCamera.transform.eulerAngles.x, 0.0f, mainCamera.transform.eulerAngles.z);

        if (currentPlayer == secondPlayer)
            QT = Quaternion.Euler(mainCamera.transform.eulerAngles.x, 180.0f, mainCamera.transform.eulerAngles.z);

        mainCamera.transform.rotation = QT;
        zoomSpeed *= -1;

        EnableCam();
        callback?.Invoke();
    }

    private IEnumerator RotateAroundCenter()
    {
        Vector3 boardCenterPosition = boardCenter.transform.position;
        while (true)
        {
            mainCamera.transform.RotateAround(boardCenterPosition, Vector3.up, Time.deltaTime * 10.0f);
            yield return null;
        }
    }
    public void RandomCameraMovement()
    {
        mainCamera.transform.position = startPosition.position;
        mainCamera.transform.rotation = Quaternion.Euler(startPosition.eulerAngles.x, startPosition.eulerAngles.y, 0.0f);
        StartCoroutine(RotateAroundCenter());
    }

    public void DisableCam()
    {
        disableCam = true;
    }

    public void EnableCam()
    {
        disableCam = false;
    }
}