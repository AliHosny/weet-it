REM This script unregisters the documentation from target help
REM system. That means the CHM file will not be accessible from
REM other CHM files without specifying its full path. Moreover,
REM this documentation stops to be context sensitive in 
REM Visual Studio .NET.
REM You should call the following line or this script during
REM uninstallation of your product on the target machine.
REM It is not neccessary to have CHM and XML file in the same
REM directory as EXE file.

HelixoftHelpReg.exe -x"CompPrev.chm" -e"CompPrev_dyn_help.xml"