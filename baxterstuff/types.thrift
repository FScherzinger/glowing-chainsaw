namespace java de.dfki.tecs.robot.baxter
namespace cpp de.dfki.tecs.robot.baxter
namespace csharp de.dfki.tecs.robot.baxter
namespace py de.dfki.tecs.robot.baxter
namespace rb de.dfki.tecs.robot.baxter

enum Limb {
	BOTH = 0,	// use both arms
	LEFT = 1,	// use left arm
	RIGHT = 2	// use right arm
}

enum Reference_sys{
	ABSOLUTE = 0,
	RELATIVE = 1
}

enum Kinematics {
	INVERSE = 0,
	FORWARD =1
}

enum Gripper_state{
	OPEN=1
	CLOSE=0
}

enum ImageType{
	DFKI_LOGO = 0,
	ACTION_TRIGGERED = 1,
	SELECT_SURFACE = 2,
	SYSTEM_READY = 3,
	SYSTEM_SHUT_DOWN = 4,
	PICK_ITEM = 5,
	PICK_RESET = 6
}

struct Angles {
	1: string left_s0 = "",
	2: string left_s1 = "",
	3: string left_e0 = "",
	4: string left_e1 = "",
	5: string left_w0 = "",
	6: string left_w1 = "",
	7: string left_w2 = "",
	8: string right_s0 = "",
	9: string right_s1 = "",
	10: string right_e0 = "",
	11: string right_e1 = "",
	12: string right_w0 = "",
	13: string right_w1 = "",
	14: string right_w2 = "",
}

struct Position {
	1: string X_left = "",
	2: string Y_left = "",
	3: string Z_left = "",
	4: string X_right = "",
	5: string Y_right = "",
	6: string Z_right = ""
}

struct Orientation {
	1: string Yaw_left = "",
	2: string Pitch_left = "",
	3: string Roll_left = "",
	4: string Yaw_right = "",
	5: string Pitch_right = "",
	6: string Roll_right = ""
}

struct Speed{
	1: string Speed_left="0.2",
	2: string Speed_right="0.2"
}

struct DoneEvent {
	1:required string message = "Done!",
	2:required bool error = 0
}

// ------------------------------------
// Arm events
// ------------------------------------

struct PickEvent {
	1:required Limb limb = Limb.LEFT,	
	2:required Position pos = {}, 
	3:required Orientation ori = {},
	4:required Speed speed = {},
	5:required Angles angls = {},
	6:required Reference_sys mode = Reference_sys.ABSOLUTE
	7:required Kinematics kin = Kinematics.INVERSE
}

struct PlaceEvent {
	1:required Limb limb = Limb.LEFT,	
	2:required Position pos = {}, 
	3:required Orientation ori = {},
	4:required Speed speed = {},
	5:required Angles angls = {},
	6:required Reference_sys mode = Reference_sys.ABSOLUTE
	7:required Kinematics kin = Kinematics.INVERSE
}

struct PointEvent {
	1:required Limb limb = Limb.LEFT,	
	2:required Position pos = {}, 
	3:required Orientation ori = {},
	4:required Speed speed = {},
	5:required Angles angls = {},
	6:required Reference_sys mode = Reference_sys.ABSOLUTE
	7:required Kinematics kin = Kinematics.INVERSE
}

struct MoveArmEvent {
	1:required Limb limb = Limb.LEFT,
	2:required Position pos = {}, 
	3:required Orientation ori = {},
	4:required Speed speed = {},
	5:required Angles angls = {},
	6:required Reference_sys mode = Reference_sys.ABSOLUTE
	7:required Kinematics kin = Kinematics.INVERSE
}

struct PickAndPlaceEvent {
	1:required Limb limb = Limb.LEFT,
	2:required Position initial_pos = {},
	3:required Position final_pos = {},
	4:required Orientation initial_ori = {},
	5:required Orientation final_ori = {},
	6:required Speed speed = {},
	7:required Angles angls = {},
	8:required Reference_sys mode = Reference_sys.ABSOLUTE
	9:required Kinematics kin = Kinematics.INVERSE
}

struct RetrievePoseEvent {
	1:required Position pos = {},
	2:required Orientation ori = {}
}

struct RetrieveAnglesEvent {
	1:required Angles angles = {},
}

struct EstimateWorkingSurface {
	1:required Limb limb = Limb.LEFT,
}

// ------------------------------------
// Gripper events
// ------------------------------------

struct GripperEvent {
	1:required Limb limb = Limb.LEFT,	
	2:required Gripper_state action = Gripper_state.OPEN,
}

struct RetrieveApertureEvent{
	1:required string right_aperture="100"
	2:required string left_aperture="100"
}

// ------------------------------------
// Head events
// ------------------------------------

struct MoveHeadEvent {
	1:required double angle = 0.0,		// Desired pan angle
	2:required i32 speed = 100,			// Desired speed to pan at, range is 0-100
	3:required double timeout = 0.0 	// Seconds to wait for the head to pan to the specified angle. If 0, just command once and return.
}

struct RetrieveHeadPanEvent {
	1:required double angle = 0.0,		// pan angle
}

struct ShowImageEvent{
	1:required ImageType type = ImageType.DFKI_LOGO,
	2:required i32 surfaceId = -1,
}

// ------------------------------------
// Talk events
// ------------------------------------

struct TalkEvent {
	1:required string message = "Hello!";
}
