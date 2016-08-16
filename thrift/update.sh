#!/bin/bas
# generate csharp code
thrift -gen csharp test.thrift
# move generated files to Assets
mv gen-csharp/de/dfki/test/TestEvent.cs ../TestProject/Assets/
