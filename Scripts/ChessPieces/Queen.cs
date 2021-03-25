using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Rook
{
    protected override int[,] possibleDirections { get; set; } = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, 
        { 0, -1 }, { 1, -1 } };
}