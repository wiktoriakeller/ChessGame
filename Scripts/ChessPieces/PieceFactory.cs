using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class PieceFactory : MonoBehaviour
{
    [Serializable]
    private class ChessPiecePrefabData
    {
        public GameObject Prefab;
        public MovementType MovementType;
    }

    [SerializeField] private ChessPiecePrefabData[] chessPiecePrefabs;
    private Dictionary<string, ChessPiecePrefabData> prefabsByNames;
    private Dictionary<string, Type> piecesByNames;

    private void Awake()
    {
        prefabsByNames = new Dictionary<string, ChessPiecePrefabData>();
        
        foreach(var piecePrefab in chessPiecePrefabs)
        {
            prefabsByNames.Add(piecePrefab.Prefab.GetComponent<PieceEntity>().GetPieceType().ToString(), piecePrefab);
        }

        var pieceTypes = Assembly.GetAssembly(typeof(Piece)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Piece)));
        
        piecesByNames = new Dictionary<string, Type>();

        foreach (var type in pieceTypes)
        {
            piecesByNames.Add(type.ToString(), type);
        }
    }

    public GameObject CreatePiecePrefab(string piecePrefabName)
    {
        if (prefabsByNames.ContainsKey(piecePrefabName))
        {
            GameObject chessPiecePrefab = prefabsByNames[piecePrefabName].Prefab;
            return Instantiate(chessPiecePrefab);
        }   

        return null;
    }

    public Piece CreatePiece(string pieceName)
    {
        if (piecesByNames.ContainsKey(pieceName))
        {
            Type type = piecesByNames[pieceName];
            Piece piece = Activator.CreateInstance(type) as Piece;
            return piece;
        }

        return null;
    }

    public MovementType GetMovementType(string pieceName)
    {
        return prefabsByNames[pieceName].MovementType;
    }
}