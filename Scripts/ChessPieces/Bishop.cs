using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Rook
{
    protected override int[,] possibleDirections { get; set; } = { { 1, 1 }, { -1, 1 }, { -1, -1 }, { 1, -1 } };
}