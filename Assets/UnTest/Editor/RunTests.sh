#! /bin/bash
# builds solution, then runs tests. 
# execute with RunTests.sh path/to/myunity.sln

set -e

/Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool build $1

SLNDIR=$(dirname $1)
mono --debug $SLNDIR/Assets/UnTest/Editor/UnTest-Console/bin/Release/UnTest-Console.exe $SLNDIR/Temp/bin/Debug

