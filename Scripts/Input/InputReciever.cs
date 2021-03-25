using System.Collections.Generic;
using UnityEngine;

public abstract class InputReciever : MonoBehaviour
{
    protected List<IInputReader> inputReaders = new List<IInputReader>();

    public void AddInputReader(IInputReader reader)
    {
        if(reader != null)
            inputReaders.Add(reader);
    }
       
    public abstract void RecieveInput(ChessGameController gameController);
}