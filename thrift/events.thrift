namespace csharp de.dfki.events

enum Device{
	TANGO,
	GEARVR,
	VIVE,
	PC,
	VUFORIA
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



service Scene{

		bool Annotate(1:Annotation an),
		list<Annotation> GetAnnotations(1:i32 objectID),
		Annotation GetAnnotationById(1:i32 id),

		bool Can_Interact(1:i32 id),
		bool LockGameObject(1:i32 id),
		bool Move(1:PositionEvent e),
		bool Move_And_Rotate(1:PositionEvent e,2:DirectionEvent d),
		ObjType getObjType(1:i32 id)

}

struct Annotation{
	1:required i32 Id;
	2:required Device type;
	3:Position position;
	4:i32 objectId;
	5:string information;
}
