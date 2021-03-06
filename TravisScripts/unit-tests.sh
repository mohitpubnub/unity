#! /bin/sh

# NOTE the command args below make the assumption that your Unity project folder is
#  a subdirectory of the repo root directory, e.g. for this repo "unity-ci-test" 
#  the project folder is "UnityProject". If this is not true then adjust the 
#  -projectPath argument to point to the right location.

## Run the editor unit tests
echo "Running editor unit tests for ${UNITYCI_PROJECT_NAME} editmode"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-logFile $(pwd)/editor.log \
	-projectPath "$(pwd)/${UNITYCI_PROJECT_NAME}" \
	-runEditorTests \
	-testResults $(pwd)/test.xml \
	-editorTestsResultFile $(pwd)/test1.xml \
	-testPlatform editmode \
	-username "${UNITYCI_NEW_USER}" \
	-password "${UNITYCI_NEW_PASS}" \
	-serial "${UNITYCI_NEW_SERIAL}" 

rc0=$?
echo "Unit test logs"
#cat $(pwd)/editor1.log
cat $(pwd)/test1.xml

# returning license
echo "returning license"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense

#exit if tests failed
#if [ $rc0 -ne 0 ]; then { echo "Failed unit tests editmode"; /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense; exit $rc0; } fi	
if [ $rc0 -ne 0 ]; then { echo "Failed unit tests editmode"; exit $rc0; } fi	
