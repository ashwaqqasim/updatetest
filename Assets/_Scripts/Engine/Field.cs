using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerNumber
{
    FirstPlayer,
    SecondPlayer,
    None
}

public class Field
{
    public static int FIELD_INDEX_UNSET = -1;

    public PlayerNumber PawnPlayerNumber { get; set; }
    public int FieldIndex { get; private set; }
    public int LastFieldIndex { get; set; }
    public bool Empty {
        get
        {
            return this.PawnPlayerNumber == PlayerNumber.None;
        }
    }

    public Field(int fieldIndex)
    {
        Reset();
        FieldIndex = fieldIndex;
    }

    public Field(Field other)
    {
        PawnPlayerNumber = other.PawnPlayerNumber;
        LastFieldIndex = other.LastFieldIndex;
        FieldIndex = other.FieldIndex;
    }

    public void Reset()
    {
        PawnPlayerNumber = PlayerNumber.None;
        LastFieldIndex = FIELD_INDEX_UNSET;
    }

    public void MoveTo(Field other)
    {
        other.PawnPlayerNumber = PawnPlayerNumber;
        other.LastFieldIndex = FieldIndex;
        this.Reset();
    }

    public bool CanMoveTo(Field other)
    {
        return other.PawnPlayerNumber == PlayerNumber.None; // 🔹 الآن يمكن العودة إلى المواضع السابقة
    }


    public bool BelongsTo(PlayerNumber player)
    {
        return PawnPlayerNumber == player;
    }
}
