using System.Collections.Generic;
using UnityEngine;

public class FiledLayoutDef
{
    public int Width;
    public int Height;
    public string Data;
}

public class FieldDataManager : IECSComponent, IDataManager
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public int Guid => ComponentGuid;

	public Dictionary<int, List<BoardPosition>> Pieces = new Dictionary<int, List<BoardPosition>>();

    public int LayoutW;
    public int LayoutH;
    public byte[] LayoutData;
	
	public void OnRegisterEntity(ECSEntity entity)
	{
		Reload();
	}
	
	public void OnUnRegisterEntity(ECSEntity entity)
	{
	}

	public void Reload()
	{
		Pieces = new Dictionary<int, List<BoardPosition>>();
		LoadContentData(new ResourceConfigDataMapper<Dictionary<string, List<BoardPosition>>>("configs/field.data", NSConfigsSettings.Instance.IsUseEncryption));
		LoadLayoutData(new ResourceConfigDataMapper<FiledLayoutDef>("configs/layout.data", NSConfigsSettings.Instance.IsUseEncryption));
	}
	
	public void LoadContentData(IDataMapper<Dictionary<string, List<BoardPosition>>> dataMapper)
	{
		dataMapper.LoadData((data, error)=> 
		{
			if (string.IsNullOrEmpty(error))
			{
				foreach (var pair in data)
				{
					Pieces.Add(PieceType.Parse(pair.Key), pair.Value);
				}
			}
			else
			{
				Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
			}
		});
	}
	
    public void LoadLayoutData(IDataMapper<FiledLayoutDef> dataMapper)
    {
        dataMapper.LoadData((data, error)=> 
        {
            if (string.IsNullOrEmpty(error))
            {
                LayoutW = data.Width;
                LayoutH = data.Height;

                int size = LayoutW * LayoutH;
                if (size != data.Data.Length)
                {
                    Debug.LogError("[FieldDataManager] => LoadLayoutData: LayoutW * LayoutH != Data.Length"); 
                    return;
                }
                
                LayoutData = new byte[size];

                int index = 0;
                foreach (char c in data.Data)
                {
                    byte value = (byte) (c - '0');
                    LayoutData[index++] = value;
                }
            }
            else
            {
                Debug.LogError("[FieldDataManager] => LoadLayoutData: config not loaded");
            }
        });
    }
}