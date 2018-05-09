using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class FieldDefComponent : ECSEntity, IECSSerializeable 
{
	public static int ComponentGuid = ECSManager.GetNextGuid();

	public override int Guid { get { return ComponentGuid; } }

	private List<PieceSaveDef> fieldSave;

	[JsonProperty]
	public List<PieceSaveDef> FieldSave
	{
		get { return fieldSave; }
		set { fieldSave = value; }
	}

	[OnSerializing]
	internal void OnSerialization(StreamingContext context)
	{
		fieldSave = new List<PieceSaveDef>
		{
			new PieceSaveDef{Pos = BoardPosition.Default(), Id = PieceType.A1.Id},
			new PieceSaveDef{Pos = BoardPosition.Default().Right, Id = PieceType.A2.Id}
		};
	}
}

public class PieceSaveDef
{
	private BoardPosition pos;
	private int id;

	public BoardPosition Pos
	{
		get { return pos; }
		set { pos = value; }
	}

	public int Id
	{
		get { return id; }
		set { id = value; }
	}
}
