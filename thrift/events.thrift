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

		Information GetInformation(1:Position pos),
		Information GetInformationById(1:i32 id),
		bool Informate(1:Information info),

		list<Node> GetNodes(1:Position pos),
		Node GetNodeById(1:i32 id),
		bool Node(1:Node node),

		bool Can_Interact(1:i32 id),
		bool LockGameObject(1:i32 id),
		bool Move(1:PositionEvent e),
		bool Move_And_Rotate(1:PositionEvent e,2:DirectionEvent d),
		ObjType getObjType(1:i32 id)

}


struct Information{
	1:required Device type;
	2:required Position inspect_pos;
	3:required i32 Id;
	4:string informtion;
}

struct Annotation{
	1:required Device type;
	2:required i32 ObjectID;
	4:required i32 Id;
	3:required string information;
}

struct Node{
	1:required Device type;
	2:required Position position;
	3:required i32 Id;
	4:required string information;
}
