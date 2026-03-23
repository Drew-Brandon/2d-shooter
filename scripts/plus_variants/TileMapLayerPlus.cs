using Godot;

public partial class TileMapLayerPlus : TileMapLayer
{
	public Vector2I GetTilePos(Vector2 pos)
	{
		return (Vector2I)(pos / TileSet.TileSize).Floor();
	}

	public Vector2 TileSnap(Vector2 pos)
	{
		Vector2I tileSize = TileSet.TileSize;
		return (pos / tileSize).Floor() * tileSize + tileSize / 2;
	}

	public TileData GetCellTileDataGlobal(Vector2 pos)
	{
		return GetCellTileData(GetTilePos(pos));
	}
}
