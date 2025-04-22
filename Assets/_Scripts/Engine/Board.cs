using System.Collections.Generic;

public class Board
{
    public static int DEFAULT_NUMBER_OF_FIELDS = 24;

    public List<Field> Fields
    {
        get
        {
            return fields;
        }
    }

    private List<Field> fields;
    public Board()
    {
        this.fields = new List<Field>(DEFAULT_NUMBER_OF_FIELDS);
        for(int i = 0; i < DEFAULT_NUMBER_OF_FIELDS; i++)
        {
            this.fields.Add(new Field(i));
        }
    }

    public Board(Board other)
    {
        this.fields = new List<Field>(other.fields.Count);
        for(int i = 0; i < other.fields.Count; i++)
        {
            this.fields.Add(new Field(other.fields[i]));
        }
    }

    public Field GetField(int index)
    {
        return fields[index];
    }

    public List<Field> GetPlayerFields(PlayerNumber playerNumber)
    {
        List<Field> playerFields = new List<Field>();
        foreach(var field in fields)
        {
            if(field.PawnPlayerNumber == playerNumber)
            {
                playerFields.Add(field);
            }
        }
        return playerFields;
    }

    public List<Field> GetEmptyFields()
    {
        return GetPlayerFields(PlayerNumber.None);
    }
}
