namespace csharp de.dfki.events

enum MsgType{
	TANGO,
	GEARVR,
	VIVE
}

struct Direction{
	/** X-Coordinate **/
	1:required double x;
	/** Y-Coordinate **/
	2:required double y;
	/** Z-Coordinate **/
	3:required double z;
	/** W-Coordinate **/
	4:required double w;
}

struct Position{
	/** X-Coordinate **/
	1:required double x;
	/** Y-Coordinate **/
	2:required double y;
	/** Z-Coordinate **/
	3:required double z;
}

struct PositionEvent{
	/** Type **/
	1:required MsgType type;
	/** Position **/
	2:required Position position;
}

struct DirectionEvent{
	/** Type **/
	1:required MsgType type;
	/** Direction **/
	2:required Direction direction;
}
