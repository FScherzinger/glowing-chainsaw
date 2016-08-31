rm -R target
rm -R gencsharp/*
mkdir -p target
cp ../../../../libtecs/csharp/Thrift.dll target/Thrift.dll || { echo "could not copy Thrift.dll"; exit 1; }
cp ../../../../libtecs/csharp/libtecs.dll target/libtecs.dll || { echo "could not copy libtecs.dll"; exit 1; }

thrift -out gencsharp --gen csharp ../types/types.thrift || { echo "could not build types.thrift for csharp"; exit 1; }

dmcs src/move_arm.cs \
     gencsharp/de/dfki/tecs/robot/baxter/* \
	-r:target/libtecs.dll \
	-r:target/Thrift.dll \
	-target:exe \
	-out:target/tecs-example-move-arm-csharp.exe\
	|| { echo "Could not compile csharp main example"; exit 1; }

dmcs src/baxter_dummy.cs \
     gencsharp/de/dfki/tecs/robot/baxter/* \
	-r:target/libtecs.dll \
	-r:target/Thrift.dll \
	-target:exe \
	-out:target/tecs-example-baxter-dummy-csharp.exe\
	|| { echo "Could not compile csharp main example"; exit 1; }