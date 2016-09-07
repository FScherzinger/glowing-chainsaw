namespace csharp de.dfki.events

enum Device{
	TANGO,
	GEARVR,
	VIVE,
	PC,
	BAXTER
}

enum ObjType{
	CUBE,
	CAMERA
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
	1:required Device device;
	2:required ObjType objtype;
	3:required Position position;
	4:required i32 Id;
}

struct InformationEvent{
	1:required Device type;
	2:required Position inspect_pos;
	3:required i32 Id;
	4:string informtion;
}

struct DirectionEvent{
	1:required Device device;
	2:required ObjType objtype;
	3:required Direction direction;
	4:required i32 Id;
}

struct PointerEvent{
	1:required Device type;
	/** direction vector **/
	2:required Direction direction;
		/** base point **/
	3:required Position position;
	4:required i32 Id;
}

struct AnnotateEvent{
	1:required Device type;
	2:required i32 ObjectID;
	4:required i32 Id;
	3:required string information;
}

struct NodeEvent{
	1:required Device type;
	2:required Position position;
	3:required i32 Id;
	4:required string information;
}
